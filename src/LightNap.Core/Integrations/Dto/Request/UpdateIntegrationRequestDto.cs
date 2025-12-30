using LightNap.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Integrations.Dto.Request;

/// <summary>
/// DTO for updating an integration.
/// </summary>
public class UpdateIntegrationRequestDto
{
    /// <summary>
    /// The friendly name to display for this integration.
    /// </summary>
    [MaxLength(Constants.Dto.MaxIntegrationFriendlyNameLength)]
    public required string FriendlyName { get; set; }

    /// <summary>
    /// True if the credentials should be returned wth future client API requests.
    /// </summary>
    public bool ShareWithClient { get; set; }

    /// <summary>
    /// The credentials to use for API requests. 
    /// </summary>
    public string? Credentials { get; set; }
}