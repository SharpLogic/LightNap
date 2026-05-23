using LightNap.Configuration.Database;
using LightNap.Configuration.Extensions;
using LightNap.Core.Api;
using LightNap.Core.Identity.Models;
using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Configuration.Email;
using LightNap.Core.Data;
using LightNap.Core.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using LightNap.Core.Data.Entities;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Email.Services;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Interfaces;
using LightNap.Core.Identity.Services;
using LightNap.Core.Interfaces;
using LightNap.Core.Notifications.Interfaces;
using LightNap.Core.Notifications.Services;
using LightNap.Core.Profile.Interfaces;
using LightNap.Core.Profile.Services;
using LightNap.Core.Public.Interfaces;
using LightNap.Core.Public.Services;
using LightNap.Core.Services;
using LightNap.Core.StaticContents.Interfaces;
using LightNap.Core.StaticContents.Services;
using LightNap.Core.Users.Interfaces;
using LightNap.Core.Users.Services;
using LightNap.Core.UserSettings.Interfaces;
using LightNap.Core.UserSettings.Services;
using LightNap.DataProviders.Sqlite.Extensions;
using LightNap.DataProviders.SqlServer.Extensions;
using LightNap.WebApi.Authorization;
using LightNap.WebApi.Configuration;
using LightNap.WebApi.Filters;
using LightNap.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace LightNap.WebApi.Extensions;

/// <summary>
/// Extension methods for configuring application services.
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Adds application services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="logger">An optional logger used to report what was wired up.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, ILogger? logger = null)
    {
        logger?.LogInformation("Configuring application services");
        services.AddCors();
        services.AddHttpContextAccessor();
        services.AddSingleton<Ganss.Xss.IHtmlSanitizer>(_ => new Ganss.Xss.HtmlSanitizer());
        services.AddSingleton<IAuthorizationHandler, ClaimAuthorizationHandler>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserContext, WebUserContext>();
        services.AddScoped<IUserSettingsService, UserSettingsService>();
        services.AddScoped<ICookieManager, WebCookieManager>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IExternalLoginService, ExternalLoginService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IPublicService, PublicService>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<IRolesService, RolesService>();
        services.AddScoped<IStaticContentService, StaticContentService>();

        return services;
    }

    /// <summary>
    /// Adds database services to the service collection based on the configured database provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="databaseSettings">The database settings.</param>
    /// <param name="logger">An optional logger used to report what was wired up.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentException">Thrown when the database provider is unsupported.</exception>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration, DatabaseSettings databaseSettings, ILogger? logger = null)
    {
        switch (databaseSettings.Provider)
        {
            case DatabaseProvider.InMemory:
                services.AddLightNapInMemoryDatabase(logger: logger);
                break;
            case DatabaseProvider.Sqlite:
                services.AddLightNapSqlite(configuration.GetRequiredConnectionString("DefaultConnection"), logger);
                break;
            case DatabaseProvider.SqlServer:
                services.AddLightNapSqlServer(configuration.GetRequiredConnectionString("DefaultConnection"), logger);
                break;
            default: throw new ArgumentException($"Unsupported 'Database:Provider' setting: '{databaseSettings.Provider}'");
        }
        return services;
    }

    /// <summary>
    /// Adds email services to the service collection based on the configured email provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="emailSettings">The email settings.</param>
    /// <param name="logger">An optional logger used to report what was wired up.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentException">Thrown when the email provider is unsupported.</exception>
    public static IServiceCollection AddEmailServices(this IServiceCollection services, EmailSettings emailSettings, ILogger? logger = null)
    {
        switch (emailSettings.Provider)
        {
            case EmailProvider.LogToConsole:
                services.AddLogToConsoleEmailSender(logger);
                break;
            case EmailProvider.Smtp:
                if (emailSettings.Smtp is null) { throw new ArgumentNullException($"SMTP settings are required if '{emailSettings.Provider}' email option is set"); }
                Validator.ValidateObject(emailSettings.Smtp, new ValidationContext(emailSettings.Smtp), validateAllProperties: true);
                services.AddSmtpEmailSender(emailSettings.Smtp, logger);
                break;
            default: throw new ArgumentException($"Unsupported email provider setting: '{emailSettings.Provider}'");
        }

        services.AddScoped<IEmailService, DefaultEmailService>();

        return services;
    }

    /// <summary>
    /// Adds identity services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="jwtSettings">The JWT settings.</param>
    /// <param name="authSettings">The authentication settings.</param>
    /// <param name="logger">An optional logger used to report what was wired up.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, JwtSettings jwtSettings, WebApiAuthenticationSettings authSettings, ILogger? logger = null)
    {
        logger?.LogInformation("Configuring identity services (JWT issuer: {Issuer})", jwtSettings.Issuer);

        services.AddIdentity<ApplicationUser, ApplicationRole>(
            (options) =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
            };

            // This event is needed to allow the JWT token to be passed in the query string for SignalR hub connections
            // (which cannot set headers).
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // Check if the request path matches a hub endpoint
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        // OAuth providers are wired up through LightNap.Configuration so the WebApi project does not
        // need to reference vendor NuGets directly. Each enabled provider also registers its own
        // SupportedExternalLoginDto, which the controller picks up via IEnumerable<SupportedExternalLoginDto>.
        services.AddLightNapOAuthProviders(authSettings?.OAuth, logger);

        if (authSettings?.WindowsAuth?.Enabled == true)
        {
            services.AddAuthentication()
                .AddNegotiate();
        }

        services.AddAuthorizationBuilder()
            .AddPolicy(nameof(ClaimAuthorizationRequirement), policy => policy.Requirements.Add(new ClaimAuthorizationRequirement()));

        return services;
    }

    /// <summary>
    /// Performs database migration and seeding, with locking for multi-instance deployments.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="builderServices">The original service collection for seeding.</param>
    /// <param name="useDistributed">True if running in distributed mode.</param>
    /// <param name="databaseSettings">The database settings.</param>
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        IServiceCollection builderServices,
        bool useDistributed,
        DatabaseSettings databaseSettings)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<IServiceProvider>>();

        if (useDistributed)
        {
            await InitializeWithLockAsync(services, builderServices, context, databaseSettings, logger);
        }
        else
        {
            await PerformMigrationAsync(context, databaseSettings);
            await PerformSeedingAsync(builderServices);
        }
    }

    private static async Task InitializeWithLockAsync(
        IServiceProvider services,
        IServiceCollection builderServices,
        ApplicationDbContext context,
        DatabaseSettings databaseSettings,
        ILogger logger)
    {
        var multiplexer = services.GetRequiredService<IConnectionMultiplexer>();
        var db = multiplexer.GetDatabase();
        string lockKey = "migration_seeding_lock";
        TimeSpan lockExpiry = TimeSpan.FromMinutes(10);
        TimeSpan waitTimeout = TimeSpan.FromMinutes(5);
        DateTime startTime = DateTime.Now;
        bool locked = false;

        while (!locked && (DateTime.Now - startTime) < waitTimeout)
        {
            locked = db.StringSet(lockKey, "locked", lockExpiry, When.NotExists);
            if (!locked)
            {
                await Task.Delay(1000);
            }
        }

        if (locked)
        {
            await PerformMigrationAsync(context, databaseSettings);
            await PerformSeedingAsync(builderServices);
            await db.KeyDeleteAsync(lockKey);
        }
        else
        {
            logger.LogWarning("Could not acquire lock for migration and seeding within timeout. Skipping.");
        }
    }

    private static async Task PerformMigrationAsync(ApplicationDbContext context, DatabaseSettings databaseSettings)
    {
        if (databaseSettings.AutomaticallyApplyEfMigrations && context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }
    }

    private static async Task PerformSeedingAsync(IServiceCollection services)
    {
        var seederServiceCollection = new ServiceCollection();
        foreach (var descriptor in services.Where(descriptor => descriptor.ServiceType != typeof(IUserContext)))
        {
            seederServiceCollection.Add(descriptor);
        }
        seederServiceCollection.AddScoped<IUserContext, SystemUserContext>();
        seederServiceCollection.AddScoped<Seeder>();

        using var seederServiceProvider = seederServiceCollection.BuildServiceProvider();
        var seeder = seederServiceProvider.GetRequiredService<Seeder>();
        await seeder.SeedAsync();
    }

    /// <summary>
    /// Adds rate limiting services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="rateLimitingSettings">The rate limiting settings.</param>
    /// <param name="logger">An optional logger used to report what was wired up.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, RateLimitingSettings rateLimitingSettings, ILogger? logger = null)
    {
        logger?.LogInformation("Configuring rate limiting (global={Global}, auth={Auth}, content={Content}, registration={Registration})",
            rateLimitingSettings.GlobalPermitLimit,
            rateLimitingSettings.AuthPermitLimit,
            rateLimitingSettings.ContentPermitLimit,
            rateLimitingSettings.RegistrationPermitLimit);
        services.AddRateLimiter(options =>
        {
            // Helper to get partition key (user ID or IP)
            string GetPartitionKey(HttpContext httpContext)
            {
                string? userId = httpContext.User.TryGetUserId();
                return userId ?? (httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            }

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                string key = GetPartitionKey(httpContext);

                return RateLimitPartition.GetFixedWindowLimiter(key, partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitingSettings.GlobalPermitLimit,
                    Window = TimeSpan.FromMinutes(1)
                });
            });

            // Policy for authentication endpoints
            options.AddPolicy(WebConstants.RateLimiting.AuthPolicyName, httpContext =>
            {
                string key = GetPartitionKey(httpContext);

                return RateLimitPartition.GetFixedWindowLimiter(key, partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitingSettings.AuthPermitLimit,
                    Window = TimeSpan.FromMinutes(1)
                });
            });

            // Policy for content-heavy endpoints
            options.AddPolicy(WebConstants.RateLimiting.ContentPolicyName, httpContext =>
            {
                string key = GetPartitionKey(httpContext);

                return RateLimitPartition.GetFixedWindowLimiter(key, partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitingSettings.ContentPermitLimit,
                    Window = TimeSpan.FromMinutes(1)
                });
            });

            // Policy for registration endpoint (strict to prevent spam)
            options.AddPolicy(WebConstants.RateLimiting.RegistrationPolicyName, httpContext =>
            {
                string key = GetPartitionKey(httpContext);

                return RateLimitPartition.GetFixedWindowLimiter(key, partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitingSettings.RegistrationPermitLimit,
                    Window = TimeSpan.FromMinutes(1)
                });
            });

            // On rejection, return 429 with custom message
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
            };
        });

        return services;
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services, ILogger? logger = null)
    {
        logger?.LogInformation("Configuring Swagger services");
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            // Dynamically get Core assembly name from a type
            var coreAssemblyName = typeof(LoginSuccessDto).Assembly.GetName().Name;
            var coreXmlFile = $"{coreAssemblyName}.xml";
            var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFile);
            if (File.Exists(coreXmlPath))
            {
                options.IncludeXmlComments(coreXmlPath);
            }

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

            // Enable support for non-nullable reference types and required properties so everything generated from
            // the spec isn't optional and nullable unless it's supposed to be
            options.SupportNonNullableReferenceTypes();
            options.NonNullableReferenceTypesAsRequired();

            // Include schemas for select base classes so that frontend type generation from tools like Orval give us
            // an easier time working with polymorphic types, such as when searching users as admin vs. regular user
            options.UseOneOfForPolymorphism();
        });

        return services;
    }

    /// <summary>
    /// Wires up the generic <see cref="ITelemetryClient"/> seam. When <paramref name="enabled"/> is
    /// true, also registers the Application Insights collectors so the AI <see cref="TelemetryClient"/>
    /// is available for the production implementation. When false, only a no-op client is registered.
    /// </summary>
    public static IServiceCollection AddLightNapTelemetryServices(this IServiceCollection services, bool enabled, ILogger? logger = null)
    {
        if (enabled)
        {
            logger?.LogInformation("Configuring Application Insights telemetry");
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<ITelemetryClient>(sp =>
                new ApplicationInsightsTelemetryClient(sp.GetRequiredService<TelemetryClient>()));
        }
        else
        {
            logger?.LogInformation("Telemetry disabled — using NoOpTelemetryClient");
            services.AddSingleton<ITelemetryClient, NoOpTelemetryClient>();
        }
        return services;
    }
}