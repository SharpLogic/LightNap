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
        public IntegrationType Type { get; set; }

        /// <summary>
        /// The services this integration supports.
        /// </summary>
        public List<IntegrationService> Services { get; set; } = [];
    }
}