using LightNap.Core.Api;
using LightNap.Core.Configuration;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

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
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentException">Thrown when the database provider is unsupported.</exception>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            string databaseProvider = configuration.GetRequiredSetting("DatabaseProvider");
            switch (databaseProvider)
            {
                case "InMemory":
                    services.AddLightNapInMemoryDatabase();
                    break;
                case "Sqlite":
                    services.AddLightNapSqlite(configuration);
                    break;
                case "SqlServer":
                    services.AddLightNapSqlServer(configuration);
                    break;
                default: throw new ArgumentException($"Unsupported 'DatabaseProvider' setting: '{databaseProvider}'");
            }
            return services;
        }

        /// <summary>
        /// Adds email services to the service collection based on the configured email provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentException">Thrown when the email provider is unsupported.</exception>
        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            string emailProvider = configuration.GetRequiredSetting("Email:Provider");
            switch (emailProvider)
            {
                case "LogToConsole":
                    services.AddLogToConsoleEmailSender();
                    break;
                case "Smtp":
                    services.AddSmtpEmailSender();
                    break;
                default: throw new ArgumentException($"Unsupported 'Email:Provider' setting: '{emailProvider}'");
            }

            services.AddScoped<IEmailService, DefaultEmailService>();

            return services;
        }

        /// <summary>
        /// Adds identity services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
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
                    ValidIssuer = configuration.GetRequiredSetting("Jwt:Issuer"),
                    ValidAudience = configuration.GetRequiredSetting("Jwt:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetRequiredSetting("Jwt:Key")))
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

            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(ClaimAuthorizationRequirement), policy => policy.Requirements.Add(new ClaimAuthorizationRequirement()));
            });

            return services;
        }


        /// <summary>
        /// Performs database migration and seeding, with locking for multi-instance deployments.
        /// </summary>
        /// <param name="services">The service provider.</param>
        /// <param name="builderServices">The original service collection for seeding.</param>
        /// <param name="useDistributed">True if running in distributed mode.</param>
        /// <param name="logger">The logger for error/warning messages.</param>
        public static async Task InitializeDatabaseAsync(
            this IServiceProvider services,
            IServiceCollection builderServices,
            bool useDistributed,
            ILogger logger)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var applicationSettings = services.GetRequiredService<IOptions<ApplicationSettings>>();

            if (useDistributed)
            {
                await InitializeWithLockAsync(services, builderServices, context, applicationSettings, logger);
            }
            else
            {
                await PerformMigrationAsync(context, applicationSettings);
                await PerformSeedingAsync(builderServices);
            }
        }

        private static async Task InitializeWithLockAsync(
            IServiceProvider services,
            IServiceCollection builderServices,
            ApplicationDbContext context,
            IOptions<ApplicationSettings> applicationSettings,
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
                await PerformMigrationAsync(context, applicationSettings);
                await PerformSeedingAsync(builderServices);
                await db.KeyDeleteAsync(lockKey);
            }
            else
            {
                logger.LogWarning("Could not acquire lock for migration and seeding within timeout. Skipping.");
            }
        }

        private static async Task PerformMigrationAsync(ApplicationDbContext context, IOptions<ApplicationSettings> applicationSettings)
        {
            if (applicationSettings.Value.AutomaticallyApplyEfMigrations && context.Database.IsRelational())
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

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            using var seederServiceProvider = seederServiceCollection.BuildServiceProvider();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            var seeder = seederServiceProvider.GetRequiredService<Seeder>();
            await seeder.SeedAsync();
        }
    }
}