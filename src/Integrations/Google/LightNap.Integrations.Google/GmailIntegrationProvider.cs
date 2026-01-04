using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Interfaces;
using Microsoft.AspNetCore.Identity;
using LightNap.Core.Extensions;
using LightNap.Core.Integrations.Models;
using LightNap.Core.Configuration.Integrations;

namespace LightNap.Integrations.Google;

/// <summary>
/// Provides integration support for Gmail using OAuth authentication.
/// </summary>
public class GmailIntegrationProvider : IIntegrationProvider
{
    /// <inheritdoc/>
    public IntegrationProviderDefinition Definition => new()
    {
        Description = "Gmail integration for access to reading and sending emails.",
        DisplayName = "Gmail",
        Features = [IntegrationFeature.Email, IntegrationFeature.TestFeature],
        Key = "Gmail",
        Organization = "Google",
        RequiresBackendConfiguration = true,
    };

    /// <inheritdoc/>
    public async Task<CreateIntegrationFromOAuthRequestDto> BuildCreateIntegrationRequest(ExternalLoginInfo externalLoginInfo)
    {
        return new CreateIntegrationFromOAuthRequestDto()
        {
            AuthorizationExpiration = null,
            CredentialsExpiration = externalLoginInfo.Expiration,
            RefreshToken = externalLoginInfo.RefreshToken,
            Credentials = externalLoginInfo.AccessToken,
            FriendlyName = this.Definition.DisplayName,
            ProviderKey = this.Definition.Key,
            ShareWithClient = false,
        };
    }
}
