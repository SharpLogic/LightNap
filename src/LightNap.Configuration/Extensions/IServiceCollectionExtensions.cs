using Azure.Identity;
using LightNap.Configuration.DataProtection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace LightNap.Configuration.Extensions
{
    /// <summary>
    /// Service collection extensions provided by the LightNap.Configuration project.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Configures ASP.NET Core data protection key storage based on the selected provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="dataProtectionSettings">The data protection settings.</param>
        /// <param name="applicationName">The data protection application name. All instances that share keys must use the same value.</param>
        /// <param name="logger">An optional logger used to report what was wired up.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentException">Thrown when the provider is unsupported or required sub-settings are missing.</exception>
        public static IServiceCollection AddLightNapDataProtectionServices(this IServiceCollection services, DataProtectionSettings dataProtectionSettings, string applicationName = "LightNap", ILogger? logger = null)
        {
            switch (dataProtectionSettings.Provider)
            {
                case DataProtection.DataProtectionProvider.Azure:
                    ArgumentNullException.ThrowIfNull(dataProtectionSettings.Azure, $"Azure data protection settings are required if '{dataProtectionSettings.Provider}' data protection option is set");

                    logger?.LogInformation("Configuring Azure Key Vault data protection with Key Vault URL '{KeyVaultUrl}' and Blob URL '{BlobUrl}'",
                        dataProtectionSettings.Azure.KeyVaultUrl, dataProtectionSettings.Azure.BlobUrl);
                    services.AddDataProtection()
                        .PersistKeysToAzureBlobStorage(dataProtectionSettings.Azure.BlobUrl, new DefaultAzureCredential())
                        .ProtectKeysWithAzureKeyVault(dataProtectionSettings.Azure.KeyVaultUrl, new DefaultAzureCredential())
                        .SetApplicationName(applicationName);
                    break;

                case DataProtection.DataProtectionProvider.Local:
                    ArgumentNullException.ThrowIfNull(dataProtectionSettings.Local, $"Certificate settings are required if '{dataProtectionSettings.Provider}' data protection option is set");

                    logger?.LogInformation("Configuring local certificate data protection with certificate name '{CertificateName}'", dataProtectionSettings.Local.CertificateName);
                    var keysFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{applicationName}-Keys");
                    using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
                    {
                        X509Certificate2 certificate = store.Certificates
                            .Find(X509FindType.FindBySubjectName, dataProtectionSettings.Local.CertificateName, false)
                            .FirstOrDefault()
                            ?? throw new ArgumentException($"Unable to load data protection certificate '{dataProtectionSettings.Local.CertificateName}'");

                        services.AddDataProtection()
                            .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
                            .ProtectKeysWithCertificate(certificate)
                            .SetApplicationName(applicationName);
                    }
                    break;

                case DataProtection.DataProtectionProvider.Ephemeral:
                    logger?.LogInformation("Configuring ephemeral data protection (keys will not be persisted)");
                    services.AddDataProtection().UseEphemeralDataProtectionProvider().SetApplicationName(applicationName);
                    break;

                default: throw new ArgumentException($"Unsupported 'DataProtection:Provider' setting: '{dataProtectionSettings.Provider}'");
            }
            return services;
        }
    }
}
