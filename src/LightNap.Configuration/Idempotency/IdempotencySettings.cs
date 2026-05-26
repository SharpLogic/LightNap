namespace LightNap.Configuration.Idempotency
{
    /// <summary>
    /// Settings for the Idempotency-Key filter.
    /// </summary>
    public sealed class IdempotencySettings
    {
        /// <summary>
        /// How long a cached response is replayed for repeated calls with the same
        /// <c>Idempotency-Key</c>. Defaults to one hour.
        /// </summary>
        public TimeSpan Ttl { get; set; } = TimeSpan.FromHours(1);
    }
}
