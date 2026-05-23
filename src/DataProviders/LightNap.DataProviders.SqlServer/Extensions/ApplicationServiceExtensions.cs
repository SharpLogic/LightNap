using LightNap.Core.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightNap.DataProviders.SqlServer.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring application services.
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Adds the LightNap SQL Server services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to add the services to.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="logger">An optional logger used to report what was wired up (with a masked connection string).</param>
        /// <returns>The service collection with the added services.</returns>
        public static IServiceCollection AddLightNapSqlServer(this IServiceCollection services, string connectionString, ILogger? logger = null)
        {
            if (logger is not null)
            {
                var maskedConnectionString = MaskConnectionString(connectionString);
                logger.LogInformation("Configuring SQL Server with connection string: {ConnectionString}", maskedConnectionString);
            }
            return services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly("LightNap.DataProviders.SqlServer")));
        }

        /// <summary>
        /// Returns a masked version of a SQL Server connection string, exposing the server and database names but
        /// hiding credentials and other parameters.
        /// </summary>
        private static string MaskConnectionString(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return $"Server={builder.DataSource};Database={builder.InitialCatalog};***";
        }
    }
}
