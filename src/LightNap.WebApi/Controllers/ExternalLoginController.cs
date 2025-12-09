using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Interfaces;
using LightNap.WebApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StackExchange.Redis;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for handling identity-related actions such as login, registration, and password reset.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting(WebConstants.RateLimiting.AuthPolicyName)]
    public class ExternalLoginController(IExternalLoginService externalLoginService, IEnumerable<SupportedExternalLoginDto> supportedExternalLogins) : ControllerBase
    {
        /// <summary>
        /// Gets the supported external login providers based on the application's configuration.
        /// </summary>
        /// <returns>The API response containing the list of options.</returns>
        [HttpGet("supported", Name = nameof(GetSupportedExternalLogins))]
        public ApiResponseDto<IEnumerable<SupportedExternalLoginDto>> GetSupportedExternalLogins()
        {
            return new ApiResponseDto<IEnumerable<SupportedExternalLoginDto>>(supportedExternalLogins);
        }

        /// <summary>
        /// Gets the supported external login providers based on the application's configuration.
        /// </summary>
        /// <returns>The API response containing the list of options.</returns>
        [HttpPost("search", Name = nameof(SearchExternalLogins))]
        [Authorize(Roles = Constants.Roles.Administrator)]
        public async Task<ApiResponseDto<PagedResponseDto<AdminExternalLoginDto>>> SearchExternalLogins(SearchExternalLoginsRequestDto searchRequestDto)
        {
            return new ApiResponseDto<PagedResponseDto<AdminExternalLoginDto>>(await externalLoginService.SearchExternalLoginsAsync(searchRequestDto));
        }

        /// <summary>
        /// Removes an external login association from the specified user account.
        /// </summary>
        /// <remarks>This operation requires administrator privileges. Removing an external login may
        /// affect the user's ability to sign in using that provider.</remarks>
        /// <param name="userId">The unique identifier of the user whose external login will be removed. Cannot be null or empty.</param>
        /// <param name="loginProvider">The name of the external login provider (for example, "Google" or "Microsoft"). Cannot be null or empty.</param>
        /// <param name="providerKey">The unique key provided by the external login provider that identifies the user's login. Cannot be null or
        /// empty.</param>
        /// <returns>An ApiResponseDto containing <see langword="true"/> if the external login was removed successfully.</returns>
        [HttpDelete("remove/{userId}/{loginProvider}/{providerKey}", Name = nameof(RemoveExternalLogin))]
        [Authorize(Roles = Constants.Roles.Administrator)]
        public async Task<ApiResponseDto<bool>> RemoveExternalLogin(string userId, string loginProvider, string providerKey)
        {
            await externalLoginService.RemoveExternalLoginAsync(userId, loginProvider, providerKey);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Confirms external login association with current user.
        /// </summary>
        /// <param name="confirmationToken">The completion token.</param>
        /// <returns>The API response containing the login result.</returns>
        [HttpGet("result/{confirmationToken}", Name = nameof(GetExternalLoginResult))]
        public async Task<ApiResponseDto<ExternalLoginSuccessDto>> GetExternalLoginResult(string confirmationToken)
        {
            return new ApiResponseDto<ExternalLoginSuccessDto>(await externalLoginService.GetExternalLoginResultAsync(confirmationToken));
        }

        /// <summary>
        /// Completes the external login when the account is already linked.
        /// </summary>
        /// <param name="confirmationToken">The completion token.</param>
        /// <param name="requestDto">The completion request DTO.</param>
        /// <returns>The API response containing the login result.</returns>
        [HttpPost("complete/{confirmationToken}", Name = nameof(CompleteExternalLogin))]
        public async Task<ApiResponseDto<LoginSuccessDto>> CompleteExternalLogin(string confirmationToken, ExternalLoginRequestDto requestDto)
        {
            return new ApiResponseDto<LoginSuccessDto>(await externalLoginService.CompleteExternalLoginAsync(confirmationToken, requestDto));
        }

        /// <summary>
        /// Completes the external login registration when the user is not logged in yet.
        /// </summary>
        /// <param name="confirmationToken">The completion token.</param>
        /// <param name="requestDto">The completion request DTO.</param>
        /// <returns>The API response containing the login result.</returns>
        [HttpPost("register/{confirmationToken}", Name = nameof(CompleteExternalLoginRegistration))]
        public async Task<ApiResponseDto<LoginSuccessDto>> CompleteExternalLoginRegistration(string confirmationToken, ExternalLoginRegisterRequestDto requestDto)
        {
            return new ApiResponseDto<LoginSuccessDto>(await externalLoginService.CompleteExternalLoginRegistrationAsync(confirmationToken, requestDto));
        }


        /// <summary>
        /// Initiates external authentication with the specified provider.
        /// </summary>
        /// <remarks>
        /// This endpoint redirects the user to the external authentication provider's login page.
        /// After successful authentication, the provider will redirect back to the <see cref="LoginCallback"/> endpoint.
        /// </remarks>
        /// <param name="provider">The external authentication provider name (e.g., "Google", "Microsoft", "GitHub").</param>
        /// <param name="returnUrl">Optional. The application URL to redirect to after authentication completes. Defaults to home page if not provided.</param>
        /// <returns>A challenge result (HTTP 302) that redirects to the external provider's authentication endpoint.</returns>
        [HttpGet("login/{provider}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Login(string provider, string? returnUrl)
        {
            var redirectUrl = Url.Action(nameof(LoginCallback), "ExternalLogin", new { returnUrl })!;
            var properties = externalLoginService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return this.Challenge(properties, provider);
        }

        /// <summary>
        /// Handles the callback from an external authentication provider after user authentication.
        /// </summary>
        /// <remarks>
        /// This endpoint is called by the external provider after the user completes authentication.
        /// It processes the authentication response and redirects to the appropriate page:
        /// - On success: redirects to `/identity/external-logins/callback` with authentication token
        /// - On failure: redirects to `/identity/external-logins/error` with error details
        /// </remarks>
        /// <param name="returnUrl">Optional. The application URL to redirect to after the login flow completes. Defaults to "/" if not provided.</param>
        /// <param name="remoteError">Optional. Error message returned by the external provider if authentication failed.</param>
        /// <returns>An HTTP 302 redirect response directing the user to either the success callback page or the error page.</returns>
        /// <exception cref="UserFriendlyApiException">Caught and handled gracefully with user-friendly error messages.</exception>
        [HttpGet("callback")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> LoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            // Something failed at the provider, such as the user declining to authorize.
            if (remoteError != null)
            {
                return this.Redirect($"/identity/external-logins/error?error={Uri.EscapeDataString(remoteError)}");
            }

            try
            {
                var token = await externalLoginService.ExternalLoginCallbackAsync();
                return Redirect($"/identity/external-logins/callback?token={token}&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}");
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