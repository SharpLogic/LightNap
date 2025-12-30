namespace LightNap.Core.Integrations.Dto.Response;

/// <summary>
/// DTO for admin-level search results for user integrations.
/// </summary>
public class AdminIntegrationDto : IntegrationDto
{
    /// <summary>
    /// The user ID.
    /// </summary>
    public required string UserId { get; set; }
}