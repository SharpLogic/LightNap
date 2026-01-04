using LightNap.Core.Integrations.Models;
using System.Collections.ObjectModel;

namespace LightNap.Core.Configuration.Integrations
{
    /// <summary>
    /// Provides configuration and lookup for all integration definitions in the system.
    /// </summary>
    public static class IntegrationsConfig
    {
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
                },
                new IntegrationCategoryDefinition()
                {
                    Category = IntegrationCategory.Productivity,
                    DisplayName = "Productivity",
                    Features = [IntegrationFeature.Email]
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
                },
                new IntegrationFeatureDefinition()
                {
                    Feature = IntegrationFeature.Email,
                    DisplayName = "Email",
                }
            ];
    }
}
