using System.ComponentModel.DataAnnotations;

namespace LightNap.Configuration.Cache
{
    /// <summary>
    /// Represents the cache settings for the web API.
    /// </summary>
    public record CacheSettings
    {
        /// <summary>
        /// The expiration period, in minutes, for the associated item.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int ExpirationMinutes { get; init; }
    }
}
