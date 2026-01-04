using LightNap.Core.Integrations.Models;

namespace LightNap.Core.Configuration.Integrations;

/// <summary>
/// Represents an integration definition.
/// </summary>
public record IntegrationProviderDefinition
{
    /// <summary>
    /// The integration provider key.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// The organization the provider functionality belongs to, such as Google, Microsoft, etc.
    /// </summary>
    public required string Organization { get; init; }

    /// <summary>
    /// The display name of the provider.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// The description of the provider.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the collection of features supported by the integration.
    /// </summary>
    /// <remarks>The returned list provides information about the capabilities or options available for this
    /// integration. The collection is read-only and reflects the current set of supported features.</remarks>
    public required IReadOnlyList<IntegrationFeature> Features { get; init; }

    /// <summary>
    /// True if the user needs to be redirected to the backend for OAuth or similar flows. If false, the user can enter
    /// credentials, such as API keys, directly on the frontend.
    /// </summary>
    public required bool RequiresBackendConfiguration { get; init; }
}