using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Identity.Models;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightNap.Integrations.Microsoft;

/// <summary>
/// Service collection extensions for wiring up Microsoft external login.
/// </summary>
public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds Microsoft as an external authentication provider.
        /// </summary>
        /// <param name="oAuthSettings">The OAuth provider settings (ClientId / ClientSecret).</param>
        /// <param name="logger">An optional logger used to report what was wired up.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddMicrosoftLogin(OAuthProviderSettings oAuthSettings, ILogger? logger = null)
        {
            logger?.LogInformation("Configuring Microsoft external login");

            services.AddAuthentication()
                .AddMicrosoftAccount(options =>
                {
                    options.ClientId = oAuthSettings.ClientId;
                    options.ClientSecret = oAuthSettings.ClientSecret;
                });

            services.AddSingleton(new SupportedExternalLoginDto(MicrosoftAccountDefaults.AuthenticationScheme, MicrosoftAccountDefaults.DisplayName));

            return services;
        }
    }
}
