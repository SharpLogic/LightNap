namespace LightNap.Core.Api;

/// <summary>
/// Represents a data transfer object for an option with display information and metadata.
/// </summary>
/// <param name="Value">The unique identifier (value) for the option.</param>
/// <param name="Label">The human-readable label to display for the option.</param>
/// <param name="Description">The optional description of the option. Defaults to an empty string if not provided.</param>
public record OptionDto(string Value, string Label, string Description = "")
{
}
