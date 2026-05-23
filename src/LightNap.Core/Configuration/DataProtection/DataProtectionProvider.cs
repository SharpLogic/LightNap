namespace LightNap.Core.Configuration.DataProtection
{
    /// <summary>
    /// Specifies the available providers for data protection key storage and management.
    /// </summary>
    public enum DataProtectionProvider
    {
        /// <summary>
        /// Provider was not configured.
        /// </summary>
        Unconfigured,

        /// <summary>
        /// Store keys in Azure Blob Storage and protect with Azure Key Vault.
        /// </summary>
        Azure,

        /// <summary>
        /// Store keys in the file system and protect with a certificate.
        /// </summary>
        Local,

        /// <summary>
        /// Use the ephemeral data protection provider, which does not persist keys. Suitable for tests
        /// or scenarios where key persistence is not required.
        /// </summary>
        Ephemeral,
    }
}
