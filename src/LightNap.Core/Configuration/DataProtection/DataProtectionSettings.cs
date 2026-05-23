namespace LightNap.Core.Configuration.DataProtection
{
    /// <summary>
    /// Configuration settings for ASP.NET Core data protection, including the provider and any
    /// provider-specific sub-settings.
    /// </summary>
    public record DataProtectionSettings
    {
        /// <summary>
        /// The data protection provider to use for cryptographic key storage and protection.
        /// </summary>
        public required DataProtectionProvider Provider { get; init; }

        /// <summary>
        /// Azure-specific data protection settings. Required when <see cref="Provider"/> is
        /// <see cref="DataProtectionProvider.Azure"/>.
        /// </summary>
        public AzureDataProtectionSettings? Azure { get; init; }

        /// <summary>
        /// Local file-system data protection settings. Required when <see cref="Provider"/> is
        /// <see cref="DataProtectionProvider.Local"/>.
        /// </summary>
        public LocalDataProtectionSettings? Local { get; init; }
    }
}
