using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Users.Dto.Response;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Extensions
{
    public static class IdentityUserClaimExtensions
    {
        /// <summary>
        /// Converts a UserClaim to a ClaimDto.
        /// </summary>
        /// <param name="userClaim">The user claim to convert.</param>
        /// <returns>A ClaimDto representing the user claim.</returns>
        public static ClaimDto ToDto(this IdentityUserClaim<string> userClaim)
        {
            return new ClaimDto
            {
                Type = userClaim.ClaimType!,
                Value = userClaim.ClaimValue!
            };
        }

        /// <summary>
        /// Converts a UserClaim to a ClaimDto.
        /// </summary>
        /// <param name="userClaim">The user claim to convert.</param>
        /// <returns>A UserClaimDto representing the user claim.</returns>
        public static UserClaimDto ToUserClaimDto(this IdentityUserClaim<string> userClaim)
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
        public static List<ClaimDto> ToDtoList(this IEnumerable<IdentityUserClaim<string>> claims)
        {
            return [.. claims.Select(claim => claim.ToDto())];
        }

        /// <summary>
        /// Converts a collection of IdentityUserClaim objects to a list of UserClaimDto objects.
        /// </summary>
        /// <param name="claims">The collection of IdentityUserClaim objects to convert.</param>
        /// <returns>The list of converted UserClaimDto objects.</returns>
        public static List<UserClaimDto> ToUserClaimDtoList(this IEnumerable<IdentityUserClaim<string>> claims)
        {
            return [.. claims.Select(claim => claim.ToUserClaimDto())];
        }

    }
}