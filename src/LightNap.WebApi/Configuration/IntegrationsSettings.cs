using LightNap.Core.Configuration.Authentication;

namespace LightNap.WebApi.Configuration;

/// <summary>
/// Container for all OAuth integration settings.
/// </summary>
/// <remarks>
/// This class allows independent enabling/disabling of each Google integration.
/// Users can enable any combination of Gmail and other services,
/// but enabling each integration means accepting all required scopes for that integration.
/// </remarks>
public record IntegrationsSettings
{
    /// <summary>
    /// Gmail integration settings. Leave null to disable Gmail integration.
    /// </summary>
    public OAuthProviderSettings? Gmail { get; init; }
}
