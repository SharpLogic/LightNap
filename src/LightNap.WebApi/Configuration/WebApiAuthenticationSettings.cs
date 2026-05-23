using LightNap.Configuration.Authentication;
using LightNap.Core.Configuration.Authentication;

namespace LightNap.WebApi.Configuration;
/// <summary>
/// Web API-specific authentication settings: the base <see cref="AuthenticationSettings"/> plus
/// OAuth and Windows authentication options that only the web host needs.
/// </summary>
public record WebApiAuthenticationSettings : AuthenticationSettings
{
    /// <summary>
    /// OAuth provider settings.
    /// </summary>
    public SupportedOAuthProviderSettings? OAuth { get; init; }

    /// <summary>
    /// Windows authentication settings.
    /// </summary>
    public WindowsAuthSettings? WindowsAuth { get; init; }
}
