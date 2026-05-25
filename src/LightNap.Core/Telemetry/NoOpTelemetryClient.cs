namespace LightNap.Core.Telemetry
{
    /// <summary>
    /// Default <see cref="ITelemetryClient"/> implementation used when application telemetry is
    /// disabled. All operations are no-ops.
    /// </summary>
    public sealed class NoOpTelemetryClient : ITelemetryClient
    {
        /// <inheritdoc />
        public void TrackEvent(string eventName, IDictionary<string, string>? properties = null) { }

        /// <inheritdoc />
        public void TrackException(Exception exception, IDictionary<string, string>? properties = null) { }

        /// <inheritdoc />
        public void TrackMetric(string metricName, double value, IDictionary<string, string>? properties = null) { }
    }
}
