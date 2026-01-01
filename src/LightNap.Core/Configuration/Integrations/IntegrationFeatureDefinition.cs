using LightNap.Core.Integrations.Models;

namespace LightNap.Core.Configuration.Integrations
{
    /// <summary>
    /// Defines a feature supported via integration.
    /// </summary>
    public record IntegrationFeatureDefinition
    {
        /// <summary>
        /// The integration feature.
        /// </summary>
        public required IntegrationFeature Feature { get; init; }

        /// <summary>
        /// The display name of the category.
        /// </summary>
        public required string DisplayName { get; init; }
    }
}