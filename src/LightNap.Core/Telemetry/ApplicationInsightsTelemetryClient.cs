using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace LightNap.Core.Telemetry
{
    /// <summary>
    /// <see cref="ITelemetryClient"/> implementation that forwards calls to a
    /// <see cref="TelemetryClient"/> backed by Application Insights.
    /// </summary>
    public sealed class ApplicationInsightsTelemetryClient(TelemetryClient telemetryClient) : ITelemetryClient
    {
        /// <inheritdoc />
        public void TrackEvent(string eventName, IDictionary<string, string>? properties = null)
        {
            telemetryClient.TrackEvent(eventName, properties);
        }

        /// <inheritdoc />
        public void TrackException(Exception exception, IDictionary<string, string>? properties = null)
        {
            var telemetry = new ExceptionTelemetry(exception);
            if (properties is not null)
            {
                foreach (var kvp in properties)
                {
                    telemetry.Properties[kvp.Key] = kvp.Value;
                }
            }
            telemetryClient.TrackException(telemetry);
        }

        /// <inheritdoc />
        public void TrackMetric(string metricName, double value, IDictionary<string, string>? properties = null)
        {
            telemetryClient.TrackMetric(metricName, value, properties);
        }
    }
}
