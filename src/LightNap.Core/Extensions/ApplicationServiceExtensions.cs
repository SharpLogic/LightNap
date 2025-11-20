using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Email.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for configuring services in the application.
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Adds an in-memory database for LightNap to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="databaseName">The name of the in-memory database.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddLightNapInMemoryDatabase(this IServiceCollection services, string databaseName = "LightNap")
        {
            return services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
        }

        /// <summary>
        /// Adds a log-to-console email service implementation to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddLogToConsoleEmailSender(this IServiceCollection services)
        {
            return services.AddSingleton<IEmailSender, LogToConsoleEmailSender>();
        }

        /// <summary>
        /// Adds an SMTP email service implementation to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddSmtpEmailSender(this IServiceCollection services, SmtpSettings smtpSettings)
        {
            return services.AddSingleton<IEmailSender>(_ => new SmtpEmailSender(smtpSettings));
        }
    }
}