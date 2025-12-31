using LightNap.Core.Integrations.Models;

namespace LightNap.Core.Configuration.Integrations
{
    /// <summary>
    /// Defines a category of integrations.
    /// </summary>
    public record IntegrationCategoryDefinition
    {
        /// <summary>
        /// The integration category.
        /// </summary>
        public required IntegrationCategory Category { get; init; }

        /// <summary>
        /// The display name of the category.
        /// </summary>
        public required string DisplayName { get; init; }

        /// <summary>
        /// The services this category includes.
        /// </summary>
        public required HashSet<IntegrationService> Services { get; init; }
    }
}