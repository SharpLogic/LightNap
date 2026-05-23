using LightNap.Core.Configuration.Email;
using LightNap.Core.Data;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Email.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for configuring services in the application.
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Adds an in-memory database for LightNap to the service collection.
            /// </summary>
            /// <param name="databaseName">The name of the in-memory database.</param>
            /// <param name="logger">An optional logger used to report what was wired up.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection AddLightNapInMemoryDatabase(string databaseName = "LightNap", ILogger? logger = null)
            {
                logger?.LogInformation("Configuring in-memory database with name: {DatabaseName}", databaseName);
                return services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
            }

            /// <summary>
            /// Adds a log-to-console email service implementation to the service collection.
            /// </summary>
            /// <param name="logger">An optional logger used to report what was wired up.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection AddLogToConsoleEmailSender(ILogger? logger = null)
            {
                logger?.LogInformation("Configuring log-to-console email sender");
                return services.AddSingleton<IEmailSender, LogToConsoleEmailSender>();
            }

            /// <summary>
            /// Adds an SMTP email service implementation to the service collection.
            /// </summary>
            /// <param name="smtpSettings">The SMPT settings.</param>
            /// <param name="logger">An optional logger used to report what was wired up.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection AddSmtpEmailSender(SmtpSettings smtpSettings, ILogger? logger = null)
            {
                logger?.LogInformation("Configuring SMTP email sender with host: {Host}:{Port}", smtpSettings.Host, smtpSettings.Port);
                return services.AddSingleton<IEmailSender>(_ => new SmtpEmailSender(smtpSettings));
            }
        }
    }
}
