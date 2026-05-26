using LightNap.Configuration.Database;
using LightNap.WebApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Core.Tests.Extensions
{
    [TestClass]
    public class DatabaseProviderUnconfiguredTests
    {
        // Verifies that the existing fail-fast in AddDatabaseServices fires for the
        // DatabaseProvider.Unconfigured sentinel (the default value used to detect missing
        // configuration). The current implementation uses the switch default branch with a
        // clear "Unsupported 'Database:Provider' setting: 'Unconfigured'" message.

        [TestMethod]
        public void Unconfigured_Throws()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var settings = new DatabaseSettings { Provider = DatabaseProvider.Unconfigured };

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                services.AddDatabaseServices(configuration, settings));

            StringAssert.Contains(ex.Message, "Unconfigured");
        }

        [TestMethod]
        public void UnknownEnumValue_Throws()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var settings = new DatabaseSettings { Provider = (DatabaseProvider)999 };

            Assert.ThrowsExactly<ArgumentException>(() =>
                services.AddDatabaseServices(configuration, settings));
        }
    }
}
