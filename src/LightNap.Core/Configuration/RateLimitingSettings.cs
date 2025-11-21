using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration;

/// <summary>
/// Represents the rate limiting settings for the web API.
/// </summary>
public record RateLimitingSettings
{
    /// <summary>
    /// The global rate limit (requests per minute per IP/user).
    /// </summary>
    [Range(1, int.MaxValue)]
    public int GlobalPermitLimit { get; set; }

    /// <summary>
    /// The auth endpoints rate limit (requests per minute per IP/user).
    /// </summary>
    [Range(1, int.MaxValue)]
    public int AuthPermitLimit { get; set; }

    /// <summary>
    /// The content endpoints rate limit (requests per minute per IP/user).
    /// </summary>
    [Range(1, int.MaxValue)]
    public int ContentPermitLimit { get; set; }

    /// <summary>
    /// The registration endpoint rate limit (requests per minute per IP/user).
    /// </summary>
    [Range(1, int.MaxValue)]
    public int RegistrationPermitLimit { get; set; }
}