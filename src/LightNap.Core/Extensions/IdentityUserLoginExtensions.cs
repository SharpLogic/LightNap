using LightNap.Core.Identity.Dto.Response;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for converting <see cref="IdentityUserLogin{T}"/> objects to DTOs.
    /// </summary>
    internal static class IdentityUserLoginExtensions
    {
        /// <summary>
        /// Converts a <see cref="IdentityUserLogin{T}"/> object to an <see cref="AdminExternalLoginDto"/>.
        /// </summary>
        /// <param name="userLogin">The user login information to convert.</param>
        /// <returns>A DTO representation of the external login with admin fields like user ID.</returns>
        public static AdminExternalLoginDto ToAdminDto(this IdentityUserLogin<string> userLogin)
        {
            return new AdminExternalLoginDto(
                userLogin.UserId,
                userLogin.LoginProvider,
                userLogin.ProviderKey,
                userLogin.ProviderDisplayName ?? userLogin.LoginProvider);
        }
    }
}