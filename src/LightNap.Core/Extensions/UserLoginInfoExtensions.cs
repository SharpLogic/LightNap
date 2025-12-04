using LightNap.Core.Identity.Dto.Response;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for converting <see cref="UserLoginInfo"/> objects to DTOs.
    /// </summary>
    public static class UserLoginInfoExtensions
    {
        /// <summary>
        /// Converts a <see cref="UserLoginInfo"/> object to an <see cref="ExternalLoginDto"/>.
        /// </summary>
        /// <param name="userLogin">The user login information to convert.</param>
        /// <returns>A DTO representation of the external login.</returns>
        public static ExternalLoginDto ToDto(this UserLoginInfo userLogin)
        {
            return new ExternalLoginDto(
                userLogin.LoginProvider,
                userLogin.ProviderKey,
                userLogin.ProviderDisplayName ?? userLogin.LoginProvider);
        }
   }
}