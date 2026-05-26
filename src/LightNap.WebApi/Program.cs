using LightNap.Configuration.Cache;
using LightNap.Configuration.Database;
using LightNap.Configuration.DataProtection;
using LightNap.Configuration.Extensions;
using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Configuration.Email;
using LightNap.Core.Extensions;
using LightNap.Core.Hubs;
using LightNap.WebApi.Configuration;
using LightNap.WebApi.Extensions;
using LightNap.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Get and validate required configuration sections so we can confirm them immediately (fail fast) and use them in setup.
WebApiAuthenticationSettings appSettings = builder.Configuration.GetRequiredSection<WebApiAuthenticationSettings>("Authentication");
JwtSettings jwtSettings = builder.Configuration.GetRequiredSection<JwtSettings>("Jwt");
EmailSettings emailSettings = builder.Configuration.GetRequiredSection<EmailSettings>("Email");
CacheSettings cacheSettings = builder.Configuration.GetRequiredSection<CacheSettings>("Cache");
DatabaseSettings databaseSettings = builder.Configuration.GetRequiredSection<DatabaseSettings>("Database");
DataProtectionSettings dataProtectionSettings = builder.Configuration.GetRequiredSection<DataProtectionSettings>("DataProtection");
RateLimitingSettings rateLimitingSettings = builder.Configuration.GetRequiredSection<RateLimitingSettings>("RateLimiting");

// To enable per-browser anonymous visitor tracking (useful for apps with anonymous UGC, public
// submissions, or analytics correlation), uncomment the following, add a matching "AnonymousVisitor"
// section to appsettings.json, and the corresponding app.UseLightNapAnonymousVisitorTracking() call
// below after UseAuthentication and before endpoint mapping.
//
// var anonymousVisitorSettings = builder.Configuration.GetRequiredSection<AnonymousVisitorSettings>("AnonymousVisitor");
// builder.Services.AddLightNapAnonymousVisitorTracking(anonymousVisitorSettings, bootstrapLogger);

// Register configuration sections with validation.
builder.Services.AddOptions<AuthenticationSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Authentication"))
    .ValidateDataAnnotations();
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Jwt"))
    .ValidateDataAnnotations();
builder.Services.AddOptions<EmailSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Email"))
    .ValidateDataAnnotations();
builder.Services.AddOptions<CacheSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Cache"))
    .ValidateDataAnnotations();
builder.Services.AddOptions<RateLimitingSettings>()
    .Bind(builder.Configuration.GetRequiredSection("RateLimiting"))
    .ValidateDataAnnotations();
builder.Services.AddOptions<DatabaseSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Database"))
    .ValidateDataAnnotations();

// Check if the SeededUsers section exists before configuring and validating it
var seededUsersSection = builder.Configuration.GetSection("SeededUsers");
if (seededUsersSection.Exists())
{
    builder.Services.AddOptions<Dictionary<string, List<SeededUserConfiguration>>>()
        .Bind(seededUsersSection)
        .ValidateDataAnnotations();
}

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions((options) =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
var bootstrapLogger = loggerFactory.CreateLogger("Startup");

builder.Services
    .AddSwaggerServices(bootstrapLogger)
    .AddDatabaseServices(builder.Configuration, databaseSettings, bootstrapLogger)
    .AddLightNapDataProtectionServices(dataProtectionSettings, logger: bootstrapLogger)
    .AddEmailServices(emailSettings, bootstrapLogger)
    .AddApplicationServices(bootstrapLogger)
    .AddIdentityServices(jwtSettings, appSettings, bootstrapLogger)
    .AddRateLimitingServices(rateLimitingSettings, bootstrapLogger)
    .AddLightNapTelemetryServices(builder.Configuration.GetValue<bool>("ApplicationInsights:Enabled"), bootstrapLogger);

// Configure HybridCache conditionally
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(cacheSettings.ExpirationMinutes)
    };
});

// Configure distributed services and SignalR if in distributed mode
bool useDistributed = builder.Configuration.GetValue<bool>("UseDistributedMode");
if (useDistributed)
{
    string redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
    });

    builder.Services.AddSignalR()
        .AddJsonProtocol(jsonOptions =>
        {
            jsonOptions.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddStackExchangeRedis(options =>
        {
            options.Configuration = ConfigurationOptions.Parse(redisConnection);
        });
}
else
{
    builder.Services.AddSignalR()
        .AddJsonProtocol(jsonOptions =>
        {
            jsonOptions.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseCors(policy =>
    policy
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());

app.UseAuthentication();

// Uncomment together with the AddLightNapAnonymousVisitorTracking registration above to enable
// the per-browser visitor cookie. Placed after UseAuthentication so HttpContext.Items is populated
// for the rate limiter and downstream services.
// app.UseLightNapAnonymousVisitorTracking();

app.UseRateLimiter();
app.UseAuthorization();

app.UseWebSockets();

// Tell crawlers not to index JSON API responses, even though /api/* remains crawlable
// (so JS-rendering bots like Googlebot can complete client-side hydration).
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        context.Response.Headers.Append("X-Robots-Tag", "noindex");
    }
    await next();
});

app.MapControllers();

// Configure SignalR hubs under /api/hubs/ since this will work with the configured frontend proxy and backend token transfer.
app.MapHub<RealTimeHub>("/api/hubs/realtime");

// We need the wwwroot folder so we can append the "browser" folder the Angular app deploys to. We then need to configure the app to serve the Angular deployment,
// which includes appropriate deep links. However, if you're using a fresh clone then you won't have a wwwroot folder until you build the Angular app and WebRootPath
// will be null. We then need to check if the folder exists before we try to use it. If it doesn't, then we don't need to bother with the configuration.
string wwwRootPath = app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
string angularAppPath = Path.Combine(wwwRootPath, "browser");
if (Directory.Exists(angularAppPath))
{
    var fileProvider = new PhysicalFileProvider(angularAppPath);
    app.UseDefaultFiles(new DefaultFilesOptions
    {
        DefaultFileNames = ["index.html"],
        FileProvider = fileProvider
    });

    // Configure cache headers for Angular assets so PWA updates aren't stalled by stale
    // index.html / ngsw.json and hashed bundles can be cached forever.
    var staticFileOptions = new StaticFileOptions
    {
        FileProvider = fileProvider,
        OnPrepareResponse = context =>
        {
            var path = context.File.Name;

            // Don't cache index.html or ngsw.json (service worker manifest)
            if (path.Equals("index.html", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("ngsw.json", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("ngsw-", StringComparison.OrdinalIgnoreCase))
            {
                context.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
                context.Context.Response.Headers.Pragma = "no-cache";
                context.Context.Response.Headers.Expires = "0";
            }
            // Cache hashed assets (main.abc123.js, etc.) aggressively
            else if (path.Contains('.') &&
                     (path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
                      path.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
                      path.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) ||
                      path.EndsWith(".woff", StringComparison.OrdinalIgnoreCase)))
            {
                context.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
            }
        }
    };

    app.UseStaticFiles(staticFileOptions);
    app.MapFallbackToFile("index.html", new StaticFileOptions
    {
        FileProvider = fileProvider,
        RequestPath = "",
        OnPrepareResponse = context =>
        {
            context.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
            context.Context.Response.Headers.Pragma = "no-cache";
            context.Context.Response.Headers.Expires = "0";
        }
    });
}

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var logger = services.GetService<ILogger<Program>>() ?? throw new Exception($"Logging is not configured, so there may be deeper configuration issues");

try
{
    await services.InitializeDatabaseAsync(builder.Services, useDistributed, databaseSettings);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during database initialization");
    throw;
}

logger.LogInformation("Everything done, running app");

try
{
    app.Run();
}
catch (Exception ex)
{
    logger.LogError(ex, "Application terminated unexpectedly");
    throw;
}