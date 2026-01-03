using LightNap.Core.Configuration.Authentication;

namespace LightNap.WebApi.Configuration
{
    public record WebApiAuthenticationSettings : AuthenticationSettings
    {
        /// <summary>
        /// OAuth provider settings.
        /// </summary>
        public OAuthSettings? OAuth { get; init; }
    }
}
