using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Dto.Response;
using Microsoft.AspNetCore.Authentication;

namespace LightNap.Core.Identity.Interfaces
{
    /// <summary>  
    /// Provides methods to manage identity.  
    /// </summary>  
    public interface IExternalLoginService
    {
        /// <summary>
        /// Configures the external authentication properties for the specified provider.
        /// </summary>
        /// <param name="provider">The external authentication provider.</param>
        /// <param name="redirectUrl">The URL to redirect to after authentication.</param>
        /// <returns>The authentication properties.</returns>
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);

        /// <summary>
        /// Handles the callback from an external authentication provider.
        /// </summary>
        /// <returns>The confirmation token required to complete the process.</returns>
        Task<string> ExternalLoginCallbackAsync();

        /// <summary>
        /// Completes the external login registration.
        /// </summary>
        /// <param name="confirmationToken">The confirmation token for the login process.</param>
        /// <returns>The result of the external login.</returns>
        Task<ExternalLoginSuccessDto> GetExternalLoginResultAsync(string confirmationToken);

        /// <summary>
        /// Completes the external login registration.
        /// </summary>
        /// <param name="confirmationToken">The confirmation token for the login process.</param>
        /// <param name="requestDto">The completion request DTO.</param>
        /// <returns>The login result.</returns>
        Task<LoginSuccessDto> CompleteExternalLoginAsync(string confirmationToken, ExternalLoginRequestDto requestDto);

        /// <summary>
        /// Completes the external login registration.
        /// </summary>
        /// <param name="confirmationToken">The confirmation token for the login process.</param>
        /// <param name="requestDto">The completion request DTO.</param>
        /// <returns>The login result.</returns>
        Task<LoginSuccessDto> CompleteExternalLoginRegistrationAsync(string confirmationToken, ExternalLoginRegisterRequestDto requestDto);
    }
}