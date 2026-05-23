using LightNap.Core.Identity.Dto.Response;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for converting <see cref="UserLoginInfo"/> objects to DTOs.
    /// </summary>
    internal static class UserLoginInfoExtensions
    {
        extension(UserLoginInfo userLogin)
        {
            /// <summary>
            /// Converts a <see cref="UserLoginInfo"/> object to an <see cref="ExternalLoginDto"/>.
            /// </summary>
            /// <returns>A DTO representation of the external login.</returns>
            public ExternalLoginDto ToDto()
            {
                return new ExternalLoginDto(
                    userLogin.LoginProvider,
                    userLogin.ProviderKey,
                    userLogin.ProviderDisplayName ?? userLogin.LoginProvider);
            }
        }
    }
}
