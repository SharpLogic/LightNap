using LightNap.Core.Configuration;
using LightNap.Core.Hubs;
using LightNap.WebApi.Configuration;
using LightNap.WebApi.Extensions;
using LightNap.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Hybrid;
using StackExchange.Redis;
using LightNap.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Get and validate required configuration sections so we can confirm them immediately (fail fast) and use them in setup.
AuthenticationSettings appSettings = builder.Configuration.GetRequiredSection<AuthenticationSettings>("Authentication");
JwtSettings jwtSettings = builder.Configuration.GetRequiredSection<JwtSettings>("Jwt");
EmailSettings emailSettings = builder.Configuration.GetRequiredSection<EmailSettings>("Email");
CacheSettings cacheSettings = builder.Configuration.GetRequiredSection<CacheSettings>("Cache");
DatabaseSettings databaseSettings = builder.Configuration.GetRequiredSection<DatabaseSettings>("Database");
RateLimitingSettings rateLimitingSettings = builder.Configuration.GetRequiredSection<RateLimitingSettings>("RateLimiting");

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Add JWT auth to Swagger
    string securityDefinitionName = "Bearer";
    options.AddSecurityDefinition(securityDefinitionName, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header token using the Bearer scheme. Example: \"Bearer {paste this token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Require the Bearer token for all operations (adds the Authorize button in Swagger UI)
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(securityDefinitionName, document)] = []
    });
});

builder.Services.AddDatabaseServices(builder.Configuration, databaseSettings)
    .AddEmailServices(emailSettings)
    .AddApplicationServices()
    .AddIdentityServices(jwtSettings)
    .AddRateLimitingServices(rateLimitingSettings);

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
app.UseRateLimiter();
app.UseAuthorization();

app.UseWebSockets();

app.MapControllers();

// Configure SignalR hubs under /api/hubs/ since this will work with the configured frontend proxy and backend token transfer.
app.MapHub<NotificationsHub>("/api/hubs/notifications");

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
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = fileProvider
    });
    app.MapFallbackToFile("index.html", new StaticFileOptions
    {
        FileProvider = fileProvider,
        RequestPath = ""
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

app.Run();