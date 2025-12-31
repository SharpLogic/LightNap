using LightNap.Core.Integrations.Models;

namespace LightNap.Core.Configuration.Integrations
{
    /// <summary>
    /// Represents an integration definition.
    /// </summary>
    public record IntegrationDefinition
    {
        /// <summary>
        /// The type of integration.
        /// </summary>
        public required IntegrationType Type { get; init; }

        /// <summary>
        /// The display name of the integration.
        /// </summary>
        public required string DisplayName { get; init; }

        /// <summary>
        /// The services this integration supports.
        /// </summary>
        public required HashSet<IntegrationService> Services { get; init; }

        /// <summary>
        /// Verifies that the specified integration service is supported by the current integration type.
        /// </summary>
        /// <param name="service">The integration service to check for support.</param>
        /// <exception cref="InvalidOperationException">Thrown if the specified service is not supported by the current integration type.</exception>
        public void AssertServiceSupported(IntegrationService service)
        {
            if (!this.Services.Contains(service))
            {
                throw new InvalidOperationException($"Integration of type {Type} does not support service {service}.");
            }
        }
    }
}