using LightNap.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        /// <returns>The service collection with the added services.</returns>
        public static IServiceCollection AddLightNapSqlServer(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly("LightNap.DataProviders.SqlServer")));
        }
    }
}