using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.DataProtection
{
    /// <summary>
    /// Configuration settings required to use Azure Blob Storage + Key Vault for ASP.NET Core data
    /// protection key persistence and protection.
    /// </summary>
    public record AzureDataProtectionSettings
    {
        /// <summary>
        /// The Azure Blob URL where data protection keys are persisted.
        /// </summary>
        [Required]
        [Url]
        public required Uri BlobUrl { get; init; }

        /// <summary>
        /// The Azure Key Vault URL used to protect the persisted keys.
        /// </summary>
        [Required]
        [Url]
        public required Uri KeyVaultUrl { get; init; }
    }
}
