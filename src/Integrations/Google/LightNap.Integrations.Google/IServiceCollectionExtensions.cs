using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Integrations.Interfaces;
using LightNap.Core.Integrations.Providers;
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
        public IServiceCollection AddGoogleLogin(OAuthProviderSettings oAuthSettings)
        {
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = oAuthSettings.ClientId;
                    options.ClientSecret = oAuthSettings.ClientSecret;
                });

            return services;
        }

        public IServiceCollection AddGmailIntegration(OAuthProviderSettings oAuthSettings)
        {
            var provider = new GmailIntegrationProvider();

            services.AddKeyedSingleton<IIntegrationProvider>(provider.Key, provider);

            services.AddAuthentication()
                .AddGoogle(
                provider.Key,
                options =>
                {
                    options.ClientId = oAuthSettings.ClientId;
                    options.ClientSecret = oAuthSettings.ClientSecret;
                    options.CallbackPath = "/signin-gmail";
                    options.SaveTokens = true;

                    options.AccessType = "offline";
                    options.Scope.Add("https://www.googleapis.com/auth/gmail.readonly");
                    options.Scope.Add("https://www.googleapis.com/auth/gmail.send");
                    options.AdditionalAuthorizationParameters.Add("prompt", "consent");
                });

            return services;
        }

    }
}