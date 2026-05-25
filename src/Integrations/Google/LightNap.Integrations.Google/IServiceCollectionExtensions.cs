using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Identity.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightNap.Integrations.Google;

/// <summary>
/// Service collection extensions for wiring up Google external login.
/// </summary>
public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds Google as an external authentication provider.
        /// </summary>
        /// <param name="oAuthSettings">The OAuth provider settings (ClientId / ClientSecret).</param>
        /// <param name="logger">An optional logger used to report what was wired up.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddGoogleLogin(OAuthProviderSettings oAuthSettings, ILogger? logger = null)
        {
            logger?.LogInformation("Configuring Google external login");

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = oAuthSettings.ClientId;
                    options.ClientSecret = oAuthSettings.ClientSecret;
                });

            services.AddSingleton(new SupportedExternalLoginDto(GoogleDefaults.AuthenticationScheme, GoogleDefaults.DisplayName));

            return services;
        }
    }
}
