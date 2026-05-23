using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LightNap.MaintenanceService
{
    /// <summary>
    /// Represents the main service that runs maintenance tasks.
    /// </summary>
    internal class MainService(ILogger<MainService> logger, IEnumerable<IMaintenanceTask> tasks)
    {
        /// <summary>
        /// Runs each registered maintenance task in turn, tracking success/failure counts and total duration.
        /// </summary>
        public async Task RunAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            logger.LogInformation("Starting maintenance run");

            int successCount = 0;
            int failureCount = 0;

            foreach (var task in tasks)
            {
                logger.LogInformation("Starting '{task}'", task.Name);

                try
                {
                    await task.RunAsync();
                    successCount++;
                    logger.LogInformation("Completed '{task}'", task.Name);
                }
                catch (Exception e)
                {
                    failureCount++;
                    logger.LogError(e, "Error occurred during '{task}': {e}", task.Name, e);
                }
            }

            stopwatch.Stop();

            logger.LogInformation("Completed maintenance run. Success: {successCount}, Failures: {failureCount}, Duration: {durationMs}ms",
                successCount, failureCount, stopwatch.ElapsedMilliseconds);
        }
    }
}
