using LightNap.Core.Configuration.Authentication;
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

            return services;
        }
    }
}