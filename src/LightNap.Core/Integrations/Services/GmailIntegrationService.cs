using LightNap.Core.Api;
using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Integrations.Services
{
    internal class GmailIntegrationService
    {
        public IntegrationProvider Provider => IntegrationProvider.Gmail;

        public async Task<CreateIntegrationFromOAuthRequestDto> GetCreateIntegrationRequest(ExternalLoginInfo loginInfo)
        {
            string accessToken = loginInfo.AuthenticationProperties?.GetTokenValue("access_token") ?? throw new UserFriendlyApiException("Google did not provide an access token at the end of the successful process");
            string refreshToken = loginInfo.AuthenticationProperties?.GetTokenValue("refresh_token") ?? throw new UserFriendlyApiException("Google did not provide a refresh token at the end of the successful process");
            DateTime credentialsExpiration = DateTime.UtcNow.AddSeconds(int.Parse(loginInfo.AuthenticationProperties?.GetTokenValue("expires_in") ?? "3600"));

            return new CreateIntegrationFromOAuthRequestDto()
            {
                AuthorizationExpiration = null,
                CredentialsExpiration = credentialsExpiration,
                RefreshToken = refreshToken,
                Credentials = accessToken,
                FriendlyName = "Gmail Integration",
                Provider = Provider,
                ShareWithClient = false,
            };
        }
    }
}
