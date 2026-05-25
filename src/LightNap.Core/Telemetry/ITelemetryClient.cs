namespace LightNap.Core.Telemetry
{
    /// <summary>
    /// Generic telemetry seam for emitting custom events, exceptions, and metrics. Implementations
    /// route to a concrete telemetry sink (Application Insights, etc.) or no-op.
    /// </summary>
    public interface ITelemetryClient
    {
        /// <summary>
        /// Records a custom named event with optional properties.
        /// </summary>
        void TrackEvent(string eventName, IDictionary<string, string>? properties = null);

        /// <summary>
        /// Records an exception, optionally with extra properties.
        /// </summary>
        void TrackException(Exception exception, IDictionary<string, string>? properties = null);

        /// <summary>
        /// Records a numeric metric value with optional properties.
        /// </summary>
        void TrackMetric(string metricName, double value, IDictionary<string, string>? properties = null);
    }
}
