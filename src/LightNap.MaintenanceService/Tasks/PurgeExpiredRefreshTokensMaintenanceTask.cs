using LightNap.Core.Identity.Interfaces;

namespace LightNap.MaintenanceService.Tasks
{
    /// <summary>
    /// A maintenance task that purges expired refresh tokens.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="db">The database context.</param>
    internal class PurgeExpiredRefreshTokensMaintenanceTask(IIdentityService identityService) : IMaintenanceTask
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
            await identityService.PurgeExpiredRefreshTokens();
        }
    }
}
