using LightNap.Core.Identity.Dto.Response;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for converting <see cref="IdentityUserLogin{T}"/> objects to DTOs.
    /// </summary>
    internal static class IdentityUserLoginExtensions
    {
        extension(IdentityUserLogin<string> userLogin)
        {
            /// <summary>
            /// Converts a <see cref="IdentityUserLogin{T}"/> object to an <see cref="AdminExternalLoginDto"/>.
            /// </summary>
            /// <returns>A DTO representation of the external login with admin fields like user ID.</returns>
            public AdminExternalLoginDto ToAdminDto()
            {
                return new AdminExternalLoginDto(
                    userLogin.UserId,
                    userLogin.LoginProvider,
                    userLogin.ProviderKey,
                    userLogin.ProviderDisplayName ?? userLogin.LoginProvider);
            }
        }
    }
}
