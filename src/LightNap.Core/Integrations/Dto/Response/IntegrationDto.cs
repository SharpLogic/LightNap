namespace LightNap.Core.Integrations.Dto.Response;

/// <summary>
/// DTO for a user integration.
/// </summary>
public class IntegrationDto
{
    /// <summary>
    /// The ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// When the integration was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The external provider this integration is for.
    /// </summary>
    public required string Provider { get; set; }

    /// <summary>
    /// The expiration for integrations that need to be refreshed.
    /// </summary>
    public DateTime? Expiration { get; set; }

    /// <summary>
    /// THe last time the integration was updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// The friendly name of the integration.
    /// </summary>
    public required string FriendlyName { get; set; }

    /// <summary>
    /// True if the credentials should be included when the user requests the integration from a client request.
    /// </summary>
    public bool ShareWithClient { get; set; }

    /// <summary>
    /// The credentials, if shared with the client.
    /// </summary>
    public string? Credentials { get; set; }

    /// <summary>
    /// True if the integration is expired.
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// The active error, if any.
    /// </summary>
    public string? Error { get; set; }
}