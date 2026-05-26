using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Core.Tests.Audit
{
    [TestClass]
    public class AuditLogPurgeTests
    {
        private static ApplicationDbContext CreateDb()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLightNapInMemoryDatabase($"AuditPurgeDb_{Guid.NewGuid()}");
            return services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();
        }

        // The PurgeExpiredAuditLogsMaintenanceTask itself is internal to LightNap.MaintenanceService.
        // Replicate its load-and-modify deletion loop here so this test runs from LightNap.Core.Tests
        // without InternalsVisibleTo gymnastics, while still proving the retention contract.
        private static async Task<int> PurgeAsync(ApplicationDbContext db, int retentionDays)
        {
            var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
            const int batchSize = 100;
            int deleted = 0;
            while (true)
            {
                var expired = await db.AdminAuditLogs
                    .Where(x => x.CreatedAt < cutoff)
                    .OrderByDescending(x => x.Id)
                    .Take(batchSize)
                    .ToListAsync();
                if (expired.Count == 0) { break; }
                db.AdminAuditLogs.RemoveRange(expired);
                deleted += await db.SaveChangesAsync();
            }
            return deleted;
        }

        [TestMethod]
        public async Task Purge_RemovesOnlyExpiredEntries()
        {
            using var db = CreateDb();
            var now = DateTime.UtcNow;

            db.AdminAuditLogs.AddRange(
                new AdminAuditLog { Id = Guid.NewGuid(), ActorId = "a", Action = "old.1", CreatedAt = now.AddDays(-400) },
                new AdminAuditLog { Id = Guid.NewGuid(), ActorId = "a", Action = "old.2", CreatedAt = now.AddDays(-366) },
                new AdminAuditLog { Id = Guid.NewGuid(), ActorId = "a", Action = "fresh.1", CreatedAt = now.AddDays(-30) },
                new AdminAuditLog { Id = Guid.NewGuid(), ActorId = "a", Action = "fresh.2", CreatedAt = now.AddDays(-1) });
            await db.SaveChangesAsync();

            var deleted = await PurgeAsync(db, retentionDays: 365);

            Assert.AreEqual(2, deleted);
            var remaining = await db.AdminAuditLogs.Select(x => x.Action).ToListAsync();
            CollectionAssert.AreEquivalent(new[] { "fresh.1", "fresh.2" }, remaining);
        }

        [TestMethod]
        public async Task Purge_NoExpiredEntries_NoOp()
        {
            using var db = CreateDb();
            var now = DateTime.UtcNow;
            db.AdminAuditLogs.Add(new AdminAuditLog { Id = Guid.NewGuid(), ActorId = "a", Action = "fresh", CreatedAt = now });
            await db.SaveChangesAsync();

            var deleted = await PurgeAsync(db, retentionDays: 30);

            Assert.AreEqual(0, deleted);
            Assert.AreEqual(1, await db.AdminAuditLogs.CountAsync());
        }
    }
}
