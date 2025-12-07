using System.ComponentModel.DataAnnotations;

namespace LightNap.WebApi.Configuration
{
    /// <summary>
    /// Represents an external authentication provider supported by the application, including its identifier and
    /// display name.
    /// </summary>
    /// <param name="ProviderName">The unique name or identifier of the external authentication provider (for example, "Google" or "Facebook").
    /// Cannot be null or empty.</param>
    /// <param name="DisplayName">The user-friendly name of the external authentication provider as displayed in the UI. Cannot be null or empty.</param>
    public record SupportedExternalLoginDto(string ProviderName, string DisplayName)
    {
        /// <summary>
        /// Gets the full path URL for initiating an external login with the specified provider.
        /// </summary>
        public string LoginUrl => $"/api/ExternalLogin/login/{ProviderName}";
    }
}