using LightNap.Core.Configuration;
using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Data.Entities;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Interfaces;
using LightNap.Core.Identity.Models;
using LightNap.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LightNap.Core.Identity.Services
{
    /// <summary>
    /// Service for managing identity.
    /// </summary>
    public class InternalLoginService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IEmailService emailService,
        IOptions<AuthenticationSettings> authenticationSettings,
        ICookieManager cookieManager,
        IRefreshTokenService refreshTokenService)
    {
        /// <summary>
        /// Handles user login asynchronously.
        /// </summary>
        /// <param name="user">The application user.</param>
        /// <param name="rememberMe">Indicates whether to remember the user.</param>
        /// <param name="deviceDetails">The device details.</param>
        /// <returns>The login result DTO containing the access token or a flag indicating whether further steps are required.</returns>
        public async Task<LoginSuccessDto> HandleUserLoginAsync(ApplicationUser user, bool rememberMe, string deviceDetails)
        {
            if (authenticationSettings.Value.RequireEmailVerification && !user.EmailConfirmed)
            {
                return new LoginSuccessDto() { Type = LoginSuccessType.EmailVerificationRequired };
            }

            if (user.TwoFactorEnabled)
            {
                string code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                await emailService.SendTwoFactorAsync(user, code);
                return new LoginSuccessDto() { Type = LoginSuccessType.TwoFactorRequired };
            }

            await this.CreateRefreshTokenAsync(user, rememberMe, deviceDetails);
            return new LoginSuccessDto()
            {
                AccessToken = await tokenService.GenerateAccessTokenAsync(user),
                Type = LoginSuccessType.AccessToken
            };
        }

        /// <summary>
        /// Creates a refresh token asynchronously.
        /// </summary>
        /// <param name="user">The application user.</param>
        /// <param name="rememberMe">Indicates whether to remember the user.</param>
        /// <param name="deviceDetails">The device details.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CreateRefreshTokenAsync(ApplicationUser user, bool rememberMe, string deviceDetails)
        {
            DateTime expires = DateTime.UtcNow.AddDays(rememberMe ? authenticationSettings.Value.LogOutInactiveDeviceDays : tokenService.ExpirationMinutes / (60.0 * 24));
            var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user, deviceDetails, rememberMe, expires);
            cookieManager.SetCookie(Constants.Cookies.RefreshToken, refreshToken.Token, rememberMe, expires);
        }
    }
}
