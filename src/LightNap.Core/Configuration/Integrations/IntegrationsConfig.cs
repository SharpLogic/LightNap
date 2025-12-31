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
        public static readonly ReadOnlyCollection<IntegrationDefinition> AllIntegrations =
            [
                new IntegrationDefinition()
                {
                    Type = IntegrationType.TestType,
                    DisplayName = "Test Integration",
                    Services = [IntegrationService.TestService]
                }
            ];

        /// <summary>
        /// Gets a read-only dictionary mapping integration types to their definitions.
        /// </summary>
        public static ReadOnlyDictionary<IntegrationType, IntegrationDefinition> AllIntegrationsLookup { get; } =
            new ReadOnlyDictionary<IntegrationType, IntegrationDefinition>(
                IntegrationsConfig.AllIntegrations.ToDictionary(us => us.Type, us => us)
            );

        /// <summary>
        /// Array containing all integration categories.
        /// </summary>
        public static readonly ReadOnlyCollection<IntegrationCategoryDefinition> AllIntegrationCategories =
            [
                new IntegrationCategoryDefinition()
                {
                    Category = IntegrationCategory.TestCategory,
                    DisplayName = "Test Category",
                    Services = [IntegrationService.TestService]
                }
            ];
    }
}
