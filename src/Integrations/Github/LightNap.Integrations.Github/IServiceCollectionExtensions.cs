using AspNet.Security.OAuth.GitHub;
using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Integrations.Github;

/// <summary>
/// Provides extension methods for configuring services like login.
/// </summary>
public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddGithubLogin(OAuthProviderSettings oAuthSettings)
        {
            services.AddAuthentication()
                .AddGitHub(options =>
                {
                    options.ClientId = oAuthSettings.ClientId;
                    options.ClientSecret = oAuthSettings.ClientSecret;
                });

            services.AddSingleton<SupportedExternalLoginDto>(
                new SupportedExternalLoginDto(GitHubAuthenticationDefaults.AuthenticationScheme, GitHubAuthenticationDefaults.DisplayName));

            return services;
        }
    }
}