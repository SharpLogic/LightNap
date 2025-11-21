using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration;

/// <summary>
/// Represents the database settings for the application.
/// </summary>
public record DatabaseSettings
{
    /// <summary>
    /// The database provider (e.g., "Sqlite", "SqlServer", "InMemory").
    /// </summary>
    [Required]
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Whether to automatically apply EF migrations on startup.
    /// </summary>
    public bool AutomaticallyApplyEfMigrations { get; set; } = true;
}