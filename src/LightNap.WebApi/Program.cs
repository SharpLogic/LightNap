using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Hubs;
using LightNap.Core.Interfaces;
using LightNap.WebApi.Configuration;
using LightNap.WebApi.Extensions;
using LightNap.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.Configure<Dictionary<string, List<SeededUserConfiguration>>>(builder.Configuration.GetSection("SeededUsers"));

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

builder.Services.AddDatabaseServices(builder.Configuration)
    .AddEmailServices(builder.Configuration)
    .AddApplicationServices()
    .AddIdentityServices(builder.Configuration);

// Configure HybridCache conditionally
var cacheConfig = builder.Configuration.GetSection("Cache");
bool useDistributed = builder.Configuration.GetValue<bool>("UseDistributedMode");
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(cacheConfig.GetValue<int>("LocalExpirationMinutes"))
    };
});

// Configure distributed cache if in distributed mode
if (useDistributed)
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
    });
}

// Configure SignalR with optional backplane
var signalR = builder.Services.AddSignalR()
    .AddJsonProtocol(jsonOptions =>
    {
        jsonOptions.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

if (useDistributed)
{
    signalR.AddStackExchangeRedis(options =>
    {
        options.Configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379");
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
    var context = services.GetRequiredService<ApplicationDbContext>();
    var applicationSettings = services.GetRequiredService<IOptions<ApplicationSettings>>();
    if (applicationSettings.Value.AutomaticallyApplyEfMigrations && context.Database.IsRelational())
    {
        await context.Database.MigrateAsync();
    }

    // We want to use dependency injection for the Seeder class, but we need to replace the IUserContext service with SystemUserContext for seeding purposes.
    var seederServiceCollection = new ServiceCollection();
    foreach (var descriptor in builder.Services.Where(descriptor => descriptor.ServiceType != typeof(IUserContext)))
    {
        seederServiceCollection.Add(descriptor);
    }
    seederServiceCollection.AddScoped<IUserContext, SystemUserContext>();
    seederServiceCollection.AddScoped<Seeder>();

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
    using var seederServiceProvider = seederServiceCollection.BuildServiceProvider();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
    var seeder = seederServiceProvider.GetRequiredService<Seeder>();
    await seeder.SeedAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during migration and/or seeding");
    throw;
}

app.Run();
