namespace LightNap.WebApi.Configuration;

/// <summary>
/// Represents the configuration for a user.
/// </summary>
public class SeededUserConfiguration
{
    /// <summary>
    /// Gets or sets the email of the user. This field is required.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the username of the user. This field is required.
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password of the user. This field is optional.
    /// </summary>
    public string? Password { get; set; }
}
