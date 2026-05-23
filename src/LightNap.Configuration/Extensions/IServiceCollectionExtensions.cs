using Azure.Identity;
using LightNap.Configuration.Authentication;
using LightNap.Configuration.DataProtection;
using LightNap.Integrations.GitHub;
using LightNap.Integrations.Google;
using LightNap.Integrations.Microsoft;
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
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Wires up the configured OAuth providers by delegating to each vendor's integration library.
            /// This is the hub that lets the host project (WebApi) avoid referencing vendor NuGets directly:
            /// only <c>LightNap.Configuration</c> references the <c>LightNap.Integrations.*</c> projects.
            /// </summary>
            /// <param name="oAuthSettings">The configured OAuth providers. If <c>null</c>, no providers are wired up.</param>
            /// <param name="logger">An optional logger used to report what was wired up.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection AddLightNapOAuthProviders(SupportedOAuthProviderSettings? oAuthSettings, ILogger? logger = null)
            {
                if (oAuthSettings is null)
                {
                    logger?.LogInformation("No OAuth providers configured");
                    return services;
                }

                if (oAuthSettings.Google is not null)
                {
                    services.AddGoogleLogin(oAuthSettings.Google, logger);
                }
                if (oAuthSettings.Microsoft is not null)
                {
                    services.AddMicrosoftLogin(oAuthSettings.Microsoft, logger);
                }
                if (oAuthSettings.GitHub is not null)
                {
                    services.AddGitHubLogin(oAuthSettings.GitHub, logger);
                }

                return services;
            }

            /// <summary>
            /// Configures ASP.NET Core data protection key storage based on the selected provider.
            /// </summary>
            /// <param name="dataProtectionSettings">The data protection settings.</param>
            /// <param name="applicationName">The data protection application name. All instances that share keys must use the same value.</param>
            /// <param name="logger">An optional logger used to report what was wired up.</param>
            /// <returns>The updated service collection.</returns>
            /// <exception cref="ArgumentException">Thrown when the provider is unsupported or required sub-settings are missing.</exception>
            public IServiceCollection AddLightNapDataProtectionServices(DataProtectionSettings dataProtectionSettings, string applicationName = "LightNap", ILogger? logger = null)
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
}
