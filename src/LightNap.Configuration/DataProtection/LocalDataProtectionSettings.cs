namespace LightNap.Configuration.DataProtection
{
    /// <summary>
    /// Configuration settings for local file-system data protection key persistence with
    /// certificate-based key protection.
    /// </summary>
    public record LocalDataProtectionSettings
    {
        /// <summary>
        /// The certificate subject name used to look up the data-protection certificate from
        /// the CurrentUser X.509 store.
        /// </summary>
        public required string CertificateName { get; init; }
    }
}
