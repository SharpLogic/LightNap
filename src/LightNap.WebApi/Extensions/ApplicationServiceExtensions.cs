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
using LightNap.Core.Users.Interfaces;
using LightNap.Core.Users.Services;
using LightNap.Core.UserSettings.Interfaces;
using LightNap.Core.UserSettings.Services;
using LightNap.DataProviders.Sqlite.Extensions;
using LightNap.DataProviders.SqlServer.Extensions;
using LightNap.WebApi.Authorization;
using LightNap.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(ClaimAuthorizationRequirement), policy => policy.Requirements.Add(new ClaimAuthorizationRequirement()));
            });

            return services;
        }
    }
}