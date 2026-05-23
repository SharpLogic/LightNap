using AspNet.Security.OAuth.GitHub;
using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Identity.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightNap.Integrations.GitHub;

/// <summary>
/// Service collection extensions for wiring up GitHub external login.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds GitHub as an external authentication provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="oAuthSettings">The OAuth provider settings (ClientId / ClientSecret).</param>
    /// <param name="logger">An optional logger used to report what was wired up.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddGitHubLogin(this IServiceCollection services, OAuthProviderSettings oAuthSettings, ILogger? logger = null)
    {
        logger?.LogInformation("Configuring GitHub external login");

        services.AddAuthentication()
            .AddGitHub(options =>
            {
                options.ClientId = oAuthSettings.ClientId;
                options.ClientSecret = oAuthSettings.ClientSecret;
            });

        services.AddSingleton(new SupportedExternalLoginDto(GitHubAuthenticationDefaults.AuthenticationScheme, GitHubAuthenticationDefaults.DisplayName));

        return services;
    }
}
