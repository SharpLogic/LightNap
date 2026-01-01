using LightNap.Core.Configuration.Integrations;
using System.Collections.ObjectModel;

namespace LightNap.Core.Integrations.Dto.Response;

/// <summary>
/// DTO for supported integrations.
/// </summary>
public class SupportedIntegrationsDto
{
    /// <summary>
    /// The supported integration providers.
    /// </summary>
    public required ReadOnlyCollection<IntegrationProviderDefinition> Providers { get; set; }

    /// <summary>
    /// The supported integration categories.
    /// </summary>
    public required ReadOnlyCollection<IntegrationCategoryDefinition> Categories { get; set; }

    /// <summary>
    /// The supported integration features.
    /// </summary>
    public required ReadOnlyCollection<IntegrationFeatureDefinition> Features { get; set; }
}