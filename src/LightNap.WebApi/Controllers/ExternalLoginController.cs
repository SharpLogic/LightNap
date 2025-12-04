using LightNap.Core.Api;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Interfaces;
using LightNap.WebApi.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for handling identity-related actions such as login, registration, and password reset.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("Auth")]
    public class ExternalLoginController(IExternalLoginService externalLoginService, IEnumerable<SupportedExternalLoginDto> supportedExternalLogins) : ControllerBase
    {
        /// <summary>
        /// Gets the supported external login providers based on the application's configuration.
        /// </summary>
        /// <returns>The API response containing the list of options.</returns>
        [HttpGet("supported")]
        [ProducesResponseType(typeof(ApiResponseDto<LoginSuccessDto>), 200)]
        public ApiResponseDto<IEnumerable<SupportedExternalLoginDto>> GetSupportedExternalLogins()
        {
            return new ApiResponseDto<IEnumerable<SupportedExternalLoginDto>>(supportedExternalLogins);
        }

        /// <summary>
        /// Confirms external login association with current user.
        /// </summary>
        /// <param name="confirmationToken">The completion token.</param>
        /// <returns>The API response containing the login result.</returns>
        [HttpGet("result/{confirmationToken}")]
        [ProducesResponseType(typeof(ApiResponseDto<LoginSuccessDto>), 200)]
        [ProducesResponseType(400)]
        [EnableRateLimiting("Registration")]  // Override the controller-level "Auth" policy
        public async Task<ApiResponseDto<ExternalLoginSuccessDto>> GetExternalLoginResultAsync(string confirmationToken)
        {
            return new ApiResponseDto<ExternalLoginSuccessDto>(await externalLoginService.GetExternalLoginResultAsync(confirmationToken));
        }

        /// <summary>
        /// Completes the external login when the account is already linked.
        /// </summary>
        /// <param name="confirmationToken">The completion token.</param>
        /// <param name="requestDto">The completion request DTO.</param>
        /// <returns>The API response containing the login result.</returns>
        [HttpPost("complete/{confirmationToken}")]
        [ProducesResponseType(typeof(ApiResponseDto<LoginSuccessDto>), 200)]
        [ProducesResponseType(400)]
        [EnableRateLimiting("Registration")]  // Override the controller-level "Auth" policy
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
        [HttpPost("register/{confirmationToken}")]
        [ProducesResponseType(typeof(ApiResponseDto<LoginSuccessDto>), 200)]
        [ProducesResponseType(400)]
        [EnableRateLimiting("Registration")]  // Override the controller-level "Auth" policy
        public async Task<ApiResponseDto<LoginSuccessDto>> CompleteExternalLoginRegistration(string confirmationToken, ExternalLoginRegisterRequestDto requestDto)
        {
            return new ApiResponseDto<LoginSuccessDto>(await externalLoginService.CompleteExternalLoginRegistrationAsync(confirmationToken, requestDto));
        }


        /// <summary>
        /// Initiates external authentication with the specified provider.
        /// </summary>
        /// <param name="provider">The external authentication provider (e.g., Google, Microsoft, GitHub).</param>
        /// <param name="returnUrl">The URL to redirect to after authentication.</param>
        /// <returns>A challenge result that redirects to the external provider.</returns>
        [HttpGet("login/{provider}")]
        [ProducesResponseType(302)]
        [EnableRateLimiting("Registration")]  // Override the controller-level "Auth" policy
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Login(string provider, string? returnUrl)
        {
            var redirectUrl = Url.Action(nameof(LoginCallback), "ExternalLogin", new { returnUrl })!;
            var properties = externalLoginService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return this.Challenge(properties, provider);
        }

        /// <summary>
        /// Handles the callback from an external authentication provider.
        /// </summary>
        /// <param name="returnUrl">The URL to redirect to after successful authentication.</param>
        /// <param name="remoteError">Any error from the external provider.</param>
        /// <returns>A redirect to the confirmation page or return URL, or an error response.</returns>
        [HttpGet("callback")]
        [ProducesResponseType(302)]
        [ProducesResponseType(400)]
        [EnableRateLimiting("Registration")]  // Override the controller-level "Auth" policy
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> LoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            // Something failed at the provider, such as the user declining to authorize.
            if (remoteError != null)
            {
                return this.Redirect($"/identity/external-login-error?error={Uri.EscapeDataString(remoteError)}");
            }

            try
            {
                var token = await externalLoginService.ExternalLoginCallbackAsync();
                return Redirect($"/identity/external-login-register?token={token}&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}");
            }
            catch (UserFriendlyApiException ex)
            {
                return this.Redirect($"/identity/external-login-error?error={string.Join(',', ex.Errors.Select(e => Uri.EscapeDataString(e)))}");
            }
            catch (Exception ex)
            {
                return this.Redirect($"/identity/external-login-error?error={Uri.EscapeDataString(ex.Message)}");
            }
        }
    }
}