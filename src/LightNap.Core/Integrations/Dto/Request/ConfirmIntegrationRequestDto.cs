namespace LightNap.Core.Integrations.Dto.Request;

/// <summary>
/// DTO for confirming an OAuth integration.
/// </summary>
public class ConfirmIntegrationRequestDto
{
    /// <summary>
    /// The confirmation token.
    /// </summary>
    public required string ConfirmationToken { get; set; }
}