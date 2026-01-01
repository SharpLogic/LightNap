using LightNap.Core.Api;
using LightNap.Core.Integrations.Models;

namespace LightNap.Core.Integrations.Dto.Request;

/// <summary>
/// DTO for searching integrations.
/// </summary>
public class SearchIntegrationsRequestDto : PagedRequestDtoBase
{
    /// <summary>
    /// The provider to filter by.
    /// </summary>
    public IntegrationProvider? Provider { get; set; }

    /// <summary>
    /// The user ID to filter for.
    /// </summary>
    public string? UserId { get; set; }
}