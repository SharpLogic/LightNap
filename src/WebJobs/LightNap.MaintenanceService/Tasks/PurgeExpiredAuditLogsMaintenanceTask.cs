using LightNap.Configuration.Audit;
using LightNap.Core.Data;
using LightNap.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LightNap.MaintenanceService.Tasks
{
    /// <summary>
    /// A maintenance task that purges expired administrative audit log entries.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="db">The application database context.</param>
    /// <param name="userContext">The user context (asserted to be administrator).</param>
    /// <param name="options">The retention settings.</param>
    internal class PurgeExpiredAuditLogsMaintenanceTask(
        ILogger<PurgeExpiredAuditLogsMaintenanceTask> logger,
        ApplicationDbContext db,
        IUserContext userContext,
        IOptions<AuditLogRetentionSettings> options) : IMaintenanceTask
    {
        /// <summary>
        /// Gets the name of the maintenance task.
        /// </summary>
        public string Name => "Purge Expired Audit Logs";

        /// <summary>
        /// Runs the maintenance task asynchronously.
        /// </summary>
        public async Task RunAsync()
        {
            if (!userContext.IsAdministrator)
            {
                throw new InvalidOperationException("Purge task must run under an administrator context (typically SystemUserContext).");
            }

            var cutoff = DateTime.UtcNow.AddDays(-options.Value.RetentionDays);
            const int batchSize = 100;
            int deletedCount = 0;

            // Load-and-modify rather than ExecuteDeleteAsync — the InMemory provider does not
            // support the latter and our test suite exercises it.
            while (true)
            {
                var expired = await db.AdminAuditLogs
                    .Where(x => x.CreatedAt < cutoff)
                    .OrderByDescending(x => x.Id)
                    .Take(batchSize)
                    .ToListAsync();

                if (expired.Count == 0) { break; }

                db.AdminAuditLogs.RemoveRange(expired);
                deletedCount += await db.SaveChangesAsync();
            }

            logger.LogInformation("Deleted {Count} expired audit log entries", deletedCount);
        }
    }
}
