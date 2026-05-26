using LightNap.Core.Extensions;
using LightNap.WebApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LightNap.WebApi.Tests.Extensions
{
    [TestClass]
    public class HealthCheckRegistrationTests
    {
        [TestMethod]
        public async Task NonDistributed_RegistersDatabaseCheck_TaggedReady()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLightNapInMemoryDatabase($"HealthDb_{Guid.NewGuid()}");
            services.AddLightNapHealthChecks(useDistributed: false);

            using var provider = services.BuildServiceProvider();
            var healthCheckService = provider.GetRequiredService<HealthCheckService>();

            var report = await healthCheckService.CheckHealthAsync(r => r.Tags.Contains("ready"));

            Assert.AreEqual(HealthStatus.Healthy, report.Status);
            Assert.IsTrue(report.Entries.ContainsKey("database"));
        }

        [TestMethod]
        public void Distributed_WithoutConnectionString_Throws()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLightNapInMemoryDatabase($"HealthDb_{Guid.NewGuid()}");

            Assert.ThrowsExactly<ArgumentException>(() =>
                services.AddLightNapHealthChecks(useDistributed: true, redisConnectionString: null));
        }

        [TestMethod]
        public async Task NonDistributed_DoesNotIncludeRedisCheck()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLightNapInMemoryDatabase($"HealthDb_{Guid.NewGuid()}");
            services.AddLightNapHealthChecks(useDistributed: false);

            using var provider = services.BuildServiceProvider();
            var healthCheckService = provider.GetRequiredService<HealthCheckService>();
            var report = await healthCheckService.CheckHealthAsync();

            Assert.IsFalse(report.Entries.ContainsKey("redis"));
        }
    }
}
