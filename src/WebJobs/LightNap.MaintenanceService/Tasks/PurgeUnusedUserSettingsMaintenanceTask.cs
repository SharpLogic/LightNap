using LightNap.Core.UserSettings.Interfaces;

namespace LightNap.MaintenanceService.Tasks
{
    /// <summary>
    /// A maintenance task that purges unused user settings.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="userSettingsService">The user settings service.</param>
    internal class PurgeUnusedUserSettingsMaintenanceTask(IUserSettingsService userSettingsService) : IMaintenanceTask
    {
        /// <summary>
        /// Gets the name of the maintenance task.
        /// </summary>
        public string Name => "Purge Unused User Settings";

        /// <summary>
        /// Runs the maintenance task asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            await userSettingsService.PurgeUnusedUserSettingsAsync();
        }
    }
}
