namespace LightNap.Core.Api;

/// <summary>
/// Represents a data transfer object for an option with display information and metadata.
/// </summary>
/// <param name="Key">The unique identifier for the option.</param>
/// <param name="DisplayName">The human-readable name to display for the option.</param>
/// <param name="Description">The optional description of the option. Defaults to an empty string if not provided.</param>
public record OptionDto(string Key, string DisplayName, string Description = "")
{
}
