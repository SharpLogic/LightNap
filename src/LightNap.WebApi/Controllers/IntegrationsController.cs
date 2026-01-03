using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Configuration.Integrations;
using LightNap.Core.Data.Entities;
using LightNap.Core.Identity.Interfaces;
using LightNap.Core.Identity.Services;
using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Dto.Response;
using LightNap.Core.Integrations.Interfaces;
using LightNap.Core.Integrations.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing user integrations with external services.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationsController(IIntegrationsService integrationsService, IExternalLoginService externalLoginService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a list of all supported integration types.
        /// </summary>
        /// <returns>An <see cref="ApiResponseDto{T}"/> containing a <see cref="SupportedIntegrationsDto"/> object with the
        /// available integration types.</returns>
        [HttpGet("types", Name = nameof(GetSupportedIntegrations))]
        public ApiResponseDto<SupportedIntegrationsDto> GetSupportedIntegrations()
        {
            return new ApiResponseDto<SupportedIntegrationsDto>(integrationsService.GetSupportedIntegrations());
        }

        /// <summary>
        /// Searches all integrations (admin only).
        /// </summary>
        /// <param name="searchIntegrationsRequest">The search parameters.</param>
        /// <returns>The integrations matching the search criteria.</returns>
        /// <response code="200">Returns the search results.</response>
        [Authorize(Roles = Constants.Roles.Administrator)]
        [HttpPost("search", Name = nameof(SearchIntegrations))]
        public async Task<ApiResponseDto<PagedResponseDto<AdminIntegrationDto>>> SearchIntegrations([FromBody] SearchIntegrationsRequestDto searchIntegrationsRequest)
        {
            return new ApiResponseDto<PagedResponseDto<AdminIntegrationDto>>(await integrationsService.SearchIntegrationsAsync(searchIntegrationsRequest));
        }

        /// <summary>
        /// Deletes an integration by ID (admin only).
        /// </summary>
        /// <param name="integrationId">The ID of the integration to delete.</param>
        /// <returns>True if the integration was successfully deleted.</returns>
        /// <response code="200">Integration successfully deleted.</response>
        /// <response code="400">If there was an error deleting the integration.</response>
        [Authorize(Roles = Constants.Roles.Administrator)]
        [HttpDelete("{integrationId}", Name = nameof(DeleteIntegration))]
        public async Task<ApiResponseDto<bool>> DeleteIntegration(int integrationId)
        {
            await integrationsService.DeleteIntegrationAsync(integrationId);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Confirms an integration using the specified request data.
        /// </summary>
        /// <param name="confirmIntegrationRequestDto">The request data containing information required to confirm the integration. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="ApiResponseDto{IntegrationDto}"/> with the details of the confirmed integration.</returns>
        [HttpPost("confirm", Name = nameof(ConfirmIntegration))]
        public async Task<ApiResponseDto<IntegrationDto>> ConfirmIntegration([FromBody] ConfirmIntegrationRequestDto confirmIntegrationRequestDto)
        {
            return new ApiResponseDto<IntegrationDto>(await integrationsService.ConfirmIntegrationAsync(confirmIntegrationRequestDto));
        }

        /// <summary>
        /// Initiates an external authentication challenge using the specified provider.
        /// </summary>
        /// <remarks>This endpoint is typically used to start an OAuth or OpenID Connect authentication
        /// flow with a third-party provider. The user will be redirected to the provider's login page, and upon
        /// successful authentication, will be returned to the application via the configured callback route.</remarks>
        /// <param name="provider">The name of the external authentication provider to use for sign-in. This value is case-sensitive and must
        /// correspond to a configured provider (for example, "Google" or "Facebook").</param>
        /// <returns>A challenge result that redirects the user to the external provider's login page.</returns>
        [HttpGet("connect/{provider}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public IActionResult Connect(string provider)
        {
            var redirectUrl = Url.Action(nameof(ConnectCallback), "Integrations")!;
            var properties = externalLoginService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return this.Challenge(properties, provider);
        }

        /// <summary>
        /// Handles the callback from an external authentication provider and completes the integration process.
        /// </summary>
        /// <remarks>This endpoint is intended to be called by external authentication providers as part
        /// of the OAuth or OpenID Connect flow. It is not intended for direct user access.</remarks>
        /// <param name="returnUrl">The URL to redirect to after a successful connection. If null, a default location is used.</param>
        /// <param name="remoteError">An error message returned by the external provider, if any. If not null, the user is redirected to an error
        /// page.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects the user to the appropriate page based on the outcome of the
        /// external authentication process.</returns>
        [HttpGet("callback")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public async Task<IActionResult> ConnectCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                return this.Redirect($"/identity/external-logins/error?error={Uri.EscapeDataString(remoteError)}");
            }

            try
            {
                var confirmationToken = await integrationsService.ConnectCallbackAsync();
                return this.Redirect($"/profile/integrations/confirm/{confirmationToken}");
            }
            catch (UserFriendlyApiException ex)
            {
                return this.Redirect($"/identity/external-logins/error?error={string.Join(',', ex.Errors.Select(e => Uri.EscapeDataString(e)))}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/identity/external-logins/error?error={Uri.EscapeDataString(ex.Message)}");
            }
        }
    }
}
