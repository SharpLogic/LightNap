using LightNap.Core.Audit.Services;
using LightNap.Core.Data;
using LightNap.Core.Extensions;
using LightNap.Core.Tests.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Core.Tests.Audit
{
    [TestClass]
    public class AuditLoggerTests
    {
#pragma warning disable CS8618
        private ApplicationDbContext _db;
        private TestUserContext _userContext;
        private AuditLogger _logger;
#pragma warning restore CS8618

        [TestInitialize]
        public void Init()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLightNapInMemoryDatabase($"AuditDb_{Guid.NewGuid()}");
            var provider = services.BuildServiceProvider();

            this._db = provider.GetRequiredService<ApplicationDbContext>();
            this._userContext = new TestUserContext();
            this._userContext.LogIn("user-123");
            this._logger = new AuditLogger(this._db, this._userContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this._db.Database.EnsureDeleted();
            this._db.Dispose();
        }

        [TestMethod]
        public async Task WriteAsync_PersistsRowWithExpectedFields()
        {
            await this._logger.WriteAsync(
                "user.deactivate",
                targetType: "User",
                targetId: "target-id",
                before: new { active = true },
                after: new { active = false });

            var row = await this._db.AdminAuditLogs.SingleAsync();
            Assert.AreEqual("user-123", row.ActorId);
            Assert.AreEqual("user.deactivate", row.Action);
            Assert.AreEqual("User", row.TargetType);
            Assert.AreEqual("target-id", row.TargetId);
            StringAssert.Contains(row.BeforeJson ?? "", "\"active\":true");
            StringAssert.Contains(row.AfterJson ?? "", "\"active\":false");
            Assert.IsTrue(row.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
        }

        [TestMethod]
        public async Task WriteAsync_NullPayloads_AreNotSerialized()
        {
            await this._logger.WriteAsync("simple.action");

            var row = await this._db.AdminAuditLogs.SingleAsync();
            Assert.IsNull(row.BeforeJson);
            Assert.IsNull(row.AfterJson);
            Assert.IsNull(row.TargetType);
            Assert.IsNull(row.TargetId);
        }
    }
}
