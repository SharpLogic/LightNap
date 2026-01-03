using LightNap.Core.Integrations.Models;

namespace LightNap.Core.Integrations.Dto.Request;

/// <summary>
/// DTO for creating an integration from an OAuth process.
/// </summary>
public class CreateIntegrationFromOAuthRequestDto : CreateIntegrationRequestDto
{
    /// <summary>
    /// The refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// When the access token expires.
    /// </summary>
    public DateTime? CredentialsExpiration { get; set; }

    /// <summary>
    /// When the user will need to re-authorize.
    /// </summary>
    public DateTime? AuthorizationExpiration { get; set; }
}