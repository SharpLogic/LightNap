using LightNap.Core.Integrations.Models;

namespace LightNap.Core.Configuration.Integrations
{
    /// <summary>
    /// Represents an integration definition.
    /// </summary>
    public record IntegrationProviderDefinition
    {
        /// <summary>
        /// The integration provider.
        /// </summary>
        public required IntegrationProvider Provider { get; init; }

        /// <summary>
        /// The display name of the integration.
        /// </summary>
        public required string DisplayName { get; init; }

        /// <summary>
        /// The description of the integration.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// The features this integration supports.
        /// </summary>
        public required HashSet<IntegrationFeature> Features { get; init; }

        /// <summary>
        /// True if the integration is configured manually by the user on the frontend. Otherwise the user will be sent to the backend for OAuth or similar flows.
        /// </summary>
        public required bool IsConfiguredManually { get; init; }

        /// <summary>
        /// Verifies that the specified integration service is supported by the current integration type.
        /// </summary>
        /// <param name="service">The integration service to check for support.</param>
        /// <exception cref="InvalidOperationException">Thrown if the specified service is not supported by the current integration type.</exception>
        public void AssertServiceSupported(IntegrationFeature service)
        {
            if (!this.Features.Contains(service))
            {
                throw new InvalidOperationException($"Integration of type {Provider} does not support service {service}.");
            }
        }
    }
}