using LightNap.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightNap.DataProviders.Sqlite.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring application services.
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Adds the LightNap SQLite services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to add the services to.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="logger">An optional logger used to report what was wired up (with a masked connection string).</param>
        /// <returns>The service collection with the added services.</returns>
        public static IServiceCollection AddLightNapSqlite(this IServiceCollection services, string connectionString, ILogger? logger = null)
        {
            if (logger is not null)
            {
                var maskedConnectionString = MaskConnectionString(connectionString);
                logger.LogInformation("Configuring SQLite with connection string: {ConnectionString}", maskedConnectionString);
            }
            return services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly("LightNap.DataProviders.Sqlite")));
        }

        /// <summary>
        /// Returns a masked version of a SQLite connection string, exposing only the data source and hiding any
        /// other parameters.
        /// </summary>
        private static string MaskConnectionString(string connectionString)
        {
            var builder = new SqliteConnectionStringBuilder(connectionString);
            return $"Data Source={builder.DataSource};***";
        }
    }
}
