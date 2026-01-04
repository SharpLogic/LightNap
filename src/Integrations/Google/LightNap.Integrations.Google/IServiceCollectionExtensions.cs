using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Identity.Models;
using LightNap.Core.Integrations.Interfaces;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Integrations.Google;

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

            services.AddSingleton(new SupportedExternalLoginDto(GoogleDefaults.AuthenticationScheme, GoogleDefaults.DisplayName));

            return services;
        }

        public IServiceCollection AddGmailIntegration(OAuthProviderSettings oAuthSettings)
        {
            var provider = new GmailIntegrationProvider();

            services.AddSingleton< IIntegrationProvider>(provider);

            services.AddAuthentication()
                .AddGoogle(
                provider.Definition.Key,
                options =>
                {
                    options.ClientId = oAuthSettings.ClientId;
                    options.ClientSecret = oAuthSettings.ClientSecret;
                    options.CallbackPath = $"/signin-{provider.Definition.Key}";
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