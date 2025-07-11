using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Identity.Interfaces
{
    /// <summary>  
    /// Provides methods to manage identity.  
    /// </summary>  
    public interface IIdentityService
    {
        /// <summary>
        /// Logs in a user asynchronously.
        /// </summary>
        /// <param name="loginRequest">The login request data transfer object.</param>
        /// <returns>The result of the login operation which may be a token or indicate additional required steps.</returns>
        Task<LoginSuccessDto> LogInAsync(LoginRequestDto loginRequest);

        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="registerRequest">The register request data transfer object.</param>
        /// <returns>The result of the login operation which may be a token or indicate additional required steps.</returns>
        Task<LoginSuccessDto> RegisterAsync(RegisterRequestDto registerRequest);

        /// <summary>
        /// Logs out the current user asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task LogOutAsync();

        /// <summary>
        /// Resets the password of a user asynchronously.
        /// </summary>
        /// <param name="resetPasswordRequest">The reset password request data transfer object.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest);

        /// <summary>
        /// Sets a new password for a user asynchronously.
        /// </summary>
        /// <param name="newPasswordRequest">The new password request data transfer object.</param>
        /// <returns>The result of the login operation which may be a token or indicate additional required steps.</returns>
        Task<LoginSuccessDto> NewPasswordAsync(NewPasswordRequestDto newPasswordRequest);

        /// <summary>
        /// Verifies a 2FA code asynchronously.
        /// </summary>
        /// <param name="verifyCodeRequest">The verify code request data transfer object.</param>
        /// <returns>The access token.</returns>
        Task<string> VerifyCodeAsync(VerifyCodeRequestDto verifyCodeRequest);

        /// <summary>
        /// Gets the access token of the current user asynchronously.
        /// </summary>
        /// <returns>A new access token.</returns>
        Task<string> GetAccessTokenAsync();

        /// <summary>
        /// Requests email verification for a user asynchronously.
        /// </summary>
        /// <param name="verificationEmailRequest">Contains the email address of the user.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RequestVerificationEmailAsync(SendVerificationEmailRequestDto verificationEmailRequest);

        /// <summary>
        /// Verifies an email asynchronously.
        /// </summary>
        /// <param name="verifyEmailRequest">The verify email request data transfer object.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task VerifyEmailAsync(VerifyEmailRequestDto verifyEmailRequest);

        /// <summary>
        /// Requests a magic link the user can use to log in.
        /// </summary>
        /// <param name="magicLinkRequest">The request data transfer object containing the email address.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RequestMagicLinkEmailAsync(SendMagicLinkRequestDto magicLinkRequest);
    }
}