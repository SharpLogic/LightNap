using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Identity.Models;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Integrations.Microsoft;

/// <summary>
/// Provides extension methods for configuring services like login.
/// </summary>
public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMicrosoftLogin(OAuthProviderSettings oAuthSettings)
        {
            services.AddAuthentication()
                .AddMicrosoftAccount(options =>
                {
                    options.ClientId = oAuthSettings.ClientId;
                    options.ClientSecret = oAuthSettings.ClientSecret;
                });

            services.AddSingleton<SupportedExternalLoginDto>(
                new SupportedExternalLoginDto(MicrosoftAccountDefaults.AuthenticationScheme, MicrosoftAccountDefaults.DisplayName));

            return services;
        }
    }
}