using LightNap.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace LightNap.MaintenanceService.Tasks
{
    /// <summary>
    /// A maintenance task that purges expired refresh tokens.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="refreshTokenService">The refresh token service.</param>
    internal class PurgeExpiredRefreshTokensMaintenanceTask(ILogger<PurgeExpiredRefreshTokensMaintenanceTask> logger, IRefreshTokenService refreshTokenService) : IMaintenanceTask
    {
        /// <summary>
        /// Gets the name of the maintenance task.
        /// </summary>
        public string Name => "Purge Expired Refresh Tokens";

        /// <summary>
        /// Runs the maintenance task asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            logger.LogInformation("Starting token purge...");

            await refreshTokenService.PurgeExpiredRefreshTokens();

            logger.LogInformation("Finished token purge.");
        }
    }
}
