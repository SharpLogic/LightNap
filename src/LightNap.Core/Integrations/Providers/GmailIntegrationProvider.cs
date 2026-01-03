using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Interfaces;
using LightNap.Core.Integrations.Models;
using Microsoft.AspNetCore.Identity;
using LightNap.Core.Extensions;

namespace LightNap.Core.Integrations.Providers
{
    /// <summary>
    /// Provides integration support for Gmail using OAuth authentication.
    /// </summary>
    public class GmailIntegrationProvider : IIntegrationProvider
    {
        /// <inheritdoc/>
        public IntegrationProvider Provider => IntegrationProvider.Gmail;

        /// <inheritdoc/>
        public string DisplayName => "Gmail";

        /// <inheritdoc/>
        public async Task<CreateIntegrationFromOAuthRequestDto> BuildCreateIntegrationRequest(ExternalLoginInfo externalLoginInfo)
        {
            return new CreateIntegrationFromOAuthRequestDto()
            {
                AuthorizationExpiration = null,
                CredentialsExpiration = externalLoginInfo.Expiration,
                RefreshToken = externalLoginInfo.RefreshToken,
                Credentials = externalLoginInfo.AccessToken,
                FriendlyName = this.DisplayName,
                Provider = this.Provider,
                ShareWithClient = false,
            };
        }
    }
}
