using LightNap.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LightNap.MaintenanceService.Tasks
{
    /// <summary>
    /// A maintenance task that purges expired refresh tokens.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="db">The database context.</param>
    internal class PurgeExpiredRefreshTokensMaintenanceTask(ILogger<PurgeExpiredRefreshTokensMaintenanceTask> logger, ApplicationDbContext db) : IMaintenanceTask
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
            logger.LogInformation("Starting with {count} refresh tokens", await db.RefreshTokens.CountAsync());

            const int batchSize = 100;

            int deletedCount = 0;

            while (true)
            {
                var expiredTokens = await db.RefreshTokens
                    .Where(token => token.Expires < DateTime.UtcNow)
                    .OrderByDescending(token => token.Id)
                    .Take(batchSize)
                    .ToListAsync();
                if (expiredTokens.Count == 0) { break; }

                db.RefreshTokens.RemoveRange(expiredTokens);
                deletedCount += await db.SaveChangesAsync();
            }

            logger.LogInformation("Deleted {deletedCount} expired refresh tokens", deletedCount);

            // It's possible that some may have been created since we started.
            logger.LogInformation("Finished with {count} refresh tokens", await db.RefreshTokens.CountAsync());
        }
    }
}
