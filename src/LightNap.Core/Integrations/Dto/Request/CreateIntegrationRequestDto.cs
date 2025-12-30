namespace LightNap.Core.Integrations.Dto.Request;

/// <summary>
/// DTO for creating an integration.
/// </summary>
public class CreateIntegrationRequestDto : UpdateIntegrationRequestDto
{
    /// <summary>
    /// The integration provider.
    /// </summary>
    public required string Provider { get; set; }
}