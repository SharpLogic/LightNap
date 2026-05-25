using LightNap.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LightNap.MaintenanceService.Tasks
{
    /// <summary>
    /// A maintenance task that counts the number of registered users.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="db">The database context.</param>
    internal class CountUsersMaintenanceTask(ILogger<CountUsersMaintenanceTask> logger, ApplicationDbContext db) : IMaintenanceTask
    {
        /// <summary>
        /// Gets the name of the maintenance task.
        /// </summary>
        public string Name => "Count Users";

        /// <summary>
        /// Runs the maintenance task asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            logger.LogInformation("There are {count} registered users", await db.Users.CountAsync());
        }
    }
}
