using LightNap.Core.Users.Dto.Response;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Data.Extensions
{
    public static class IdentityUserClaimExtensions
    {
        /// <summary>
        /// Converts a UserClaim to a ClaimDto.
        /// </summary>
        /// <param name="userClaim">The user claim to convert.</param>
        /// <returns>A UserClaimDto representing the user claim.</returns>
        public static UserClaimDto ToDto(this IdentityUserClaim<string> userClaim)
        {
            return new UserClaimDto
            {
                Type = userClaim.ClaimType!,
                Value = userClaim.ClaimValue!,
                UserId = userClaim.UserId
            };
        }

        /// <summary>
        /// Converts a collection of IdentityUserClaim objects to a list of ClaimDto objects.
        /// </summary>
        /// <param name="claims">The collection of IdentityUserClaim objects to convert.</param>
        /// <returns>The list of converted ClaimDto objects.</returns>
        public static List<UserClaimDto> ToDtoList(this IEnumerable<IdentityUserClaim<string>> claims)
        {
            return claims.Select(token => token.ToDto()).ToList();
        }
    }
}