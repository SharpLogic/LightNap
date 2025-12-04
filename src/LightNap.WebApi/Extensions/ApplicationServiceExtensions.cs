using LightNap.Core.Api;
using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Configuration.Database;
using LightNap.Core.Configuration.Email;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Email.Services;
using LightNap.Core.Extensions;
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
using LightNap.WebApi.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.RateLimiting;

namespace LightNap.WebApi.Extensions
{
    /// <summary>
    /// Extension methods for configuring application services.
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Adds application services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddCors();
            services.AddHttpContextAccessor();
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
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentException">Thrown when the database provider is unsupported.</exception>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration, DatabaseSettings databaseSettings)
        {
            switch (databaseSettings.Provider)
            {
                case DatabaseProvider.InMemory:
                    services.AddLightNapInMemoryDatabase();
                    break;
                case DatabaseProvider.Sqlite:
                    services.AddLightNapSqlite(configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentException($"A 'DefaultConnection' connection string is required for '{databaseSettings.Provider}'"));
                    break;
                case DatabaseProvider.SqlServer:
                    services.AddLightNapSqlServer(configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentException($"A 'DefaultConnection' connection string is required for '{databaseSettings.Provider}'"));
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
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentException">Thrown when the email provider is unsupported.</exception>
        public static IServiceCollection AddEmailServices(this IServiceCollection services, EmailSettings emailSettings)
        {
            switch (emailSettings.Provider)
            {
                case EmailProvider.LogToConsole:
                    services.AddLogToConsoleEmailSender();
                    break;
                case EmailProvider.Smtp:
                    if (emailSettings.Smtp is null) { throw new ArgumentNullException($"SMTP settings are required if '{emailSettings.Provider}' email option is set"); }
                    Validator.ValidateObject(emailSettings.Smtp, new ValidationContext(emailSettings.Smtp), validateAllProperties: true);
                    services.AddSmtpEmailSender(emailSettings.Smtp);
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
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, JwtSettings jwtSettings, AuthenticationSettings authSettings)
        {
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

            // Callback URLs to register on partner site will be /signin-{provider} like /signin-google, /signin-microsoft, etc.
            // To add more providers, see https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social.
            var supportedExternalLogins = new List<SupportedExternalLoginDto>();
            var oAuthSettings = authSettings?.OAuth;
            if (oAuthSettings is not null)
            {
                // Add external authentication schemes
                if (oAuthSettings.Google is not null)
                {
                    services.AddAuthentication()
                        .AddGoogle(options =>
                        {
                            options.ClientId = oAuthSettings.Google.ClientId;
                            options.ClientSecret = oAuthSettings.Google.ClientSecret;
                        });
                    supportedExternalLogins.Add(new SupportedExternalLoginDto("Google", GoogleDefaults.DisplayName));
                }

                if (oAuthSettings.Microsoft is not null)
                {
                    services.AddAuthentication()
                        .AddMicrosoftAccount(options =>
                        {
                            options.ClientId = oAuthSettings.Microsoft.ClientId;
                            options.ClientSecret = oAuthSettings.Microsoft.ClientSecret;
                        });
                    supportedExternalLogins.Add(new SupportedExternalLoginDto("Microsoft", MicrosoftAccountDefaults.DisplayName));
                }
            }

            if (authSettings?.WindowsAuth?.Enabled == true)
            {
                services.AddAuthentication()
                    .AddNegotiate();
            }

            services.AddSingleton<IEnumerable<SupportedExternalLoginDto>>(supportedExternalLogins);

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
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, RateLimitingSettings rateLimitingSettings)
        {
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
                options.AddPolicy("Auth", httpContext =>
                {
                    string key = GetPartitionKey(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(key, partition => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitingSettings.AuthPermitLimit,
                        Window = TimeSpan.FromMinutes(1)
                    });
                });

                // Policy for content-heavy endpoints
                options.AddPolicy("Content", httpContext =>
                {
                    string key = GetPartitionKey(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(key, partition => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitingSettings.ContentPermitLimit,
                        Window = TimeSpan.FromMinutes(1)
                    });
                });

                // Policy for registration endpoint (strict to prevent spam)
                options.AddPolicy("Registration", httpContext =>
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
    }
}