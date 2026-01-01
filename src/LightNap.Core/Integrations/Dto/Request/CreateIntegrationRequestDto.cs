using LightNap.Core.Integrations.Models;

namespace LightNap.Core.Integrations.Dto.Request;

/// <summary>
/// DTO for creating an integration.
/// </summary>
public class CreateIntegrationRequestDto : UpdateIntegrationRequestDto
{
    /// <summary>
    /// The integration provider.
    /// </summary>
    public required IntegrationProvider Provider { get; set; }
}