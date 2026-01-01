using LightNap.Core.Configuration.UserSettings;
using LightNap.Core.Integrations.Models;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

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
                    Description = "This is a test integration provider for development and testing purposes that is configured via backend flow.",
                    Features = [IntegrationFeature.TestFeature],
                    IsConfiguredManually = false
                },
                new IntegrationProviderDefinition()
                {
                    Provider = IntegrationProvider.TestProviderManual,
                    DisplayName = "Test Integration (Manual)",
                    Description = "This is a test integration provider for development and testing purposes that can be configured on the frontend.",
                    Features = [IntegrationFeature.TestFeature],
                    IsConfiguredManually = true
                }
            ];

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

        /// <summary>
        /// Gets a read-only dictionary mapping integration providers to their definitions.
        /// </summary>
        public static ReadOnlyDictionary<IntegrationProvider, IntegrationProviderDefinition> ProviderLookup { get; private set; }

        static IntegrationsConfig()
        {
            // Basic validation to ensure no duplicates exist and that references are valid. Ideally this should run on load, but that would require creating
            // an initialization service for bootstrapping code to run on startup, which is overkill for now. So for now we run this when the class is first accessed.

            var providers = new HashSet<IntegrationProvider>(IntegrationsConfig.Providers.Select(f => f.Provider));
            if (providers.Count != IntegrationsConfig.Providers.Count)
            {
                throw new InvalidOperationException("Duplicate integration provider definitions found in IntegrationsConfig.");
            }

            var categories = new HashSet<IntegrationCategory>(IntegrationsConfig.Categories.Select(f => f.Category));
            if (categories.Count != IntegrationsConfig.Categories.Count)
            {
                throw new InvalidOperationException("Duplicate integration category definitions found in IntegrationsConfig.");
            }

            var features = new HashSet<IntegrationFeature>(IntegrationsConfig.Features.Select(f => f.Feature));
            if (features.Count != IntegrationsConfig.Features.Count)
            {
                throw new InvalidOperationException("Duplicate integration feature definitions found in IntegrationsConfig.");
            }

            foreach (var provider in IntegrationsConfig.Providers)
            {
                foreach (var feature in provider.Features)
                {
                    if (!features.Contains(feature))
                    {
                        throw new InvalidOperationException($"Integration provider '{provider.Provider}' references undefined feature '{feature}'.");
                    }
                }
            }

            IntegrationsConfig.ProviderLookup = new ReadOnlyDictionary<IntegrationProvider, IntegrationProviderDefinition>(
                IntegrationsConfig.Providers.ToDictionary(us => us.Provider, us => us)
            );
        }
    }
}
