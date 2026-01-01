using LightNap.Core.Configuration.UserSettings;
using LightNap.Core.Integrations.Models;
using System.Collections.ObjectModel;

namespace LightNap.Core.Configuration.Integrations
{
    /// <summary>
    /// Provides configuration and lookup for all integration definitions in the system.
    /// </summary>
    internal static class IntegrationsConfig
    {
        /// <summary>
        /// Array containing all integration definitions.
        /// </summary>
        public static readonly ReadOnlyCollection<IntegrationProviderDefinition> Providers =
            [
                new IntegrationProviderDefinition()
                {
                    Provider = IntegrationProvider.TestProvider,
                    DisplayName = "Test Integration",
                    Features = [IntegrationFeature.TestFeature]
                }
            ];

        /// <summary>
        /// Gets a read-only dictionary mapping integration types to their definitions.
        /// </summary>
        public static ReadOnlyDictionary<IntegrationProvider, IntegrationProviderDefinition> ProviderLookup { get; } =
            new ReadOnlyDictionary<IntegrationProvider, IntegrationProviderDefinition>(
                IntegrationsConfig.Providers.ToDictionary(us => us.Provider, us => us)
            );

        /// <summary>
        /// Array containing all integration categories.
        /// </summary>
        public static readonly ReadOnlyCollection<IntegrationCategoryDefinition> Categories =
            [
                new IntegrationCategoryDefinition()
                {
                    Category = IntegrationCategory.TestCategory,
                    DisplayName = "Test Category",
                    Features = [IntegrationFeature.TestFeature]
                }
            ];

        /// <summary>
        /// Array containing all integration features.
        /// </summary>
        public static readonly ReadOnlyCollection<IntegrationFeatureDefinition> Features =
            [
                new IntegrationFeatureDefinition()
                {
                    Feature = IntegrationFeature.TestFeature,
                    DisplayName = "Test Feature",
                }
            ];
    }
}
