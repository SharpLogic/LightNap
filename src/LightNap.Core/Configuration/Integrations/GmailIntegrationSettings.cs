using LightNap.Core.Configuration.Authentication;

namespace LightNap.Core.Configuration.Integrations
{
    /// <summary>
    /// Settings for Google Gmail integration.
    /// </summary>
    /// <remarks>
    /// Requires the following OAuth scopes:
    /// - https://www.googleapis.com/auth/gmail.readonly: Read access to Gmail messages and labels
    /// </remarks>
    public record GmailIntegrationSettings : OAuthProviderSettings
    {
        /// <summary>
        /// Gets the OAuth scopes required for Gmail integration.
        /// </summary>
        public override string Scopes => "https://www.googleapis.com/auth/gmail.readonly";
    }
}
