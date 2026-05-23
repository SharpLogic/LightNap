using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Users.Dto.Response;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Extensions
{
    internal static class IdentityUserClaimExtensions
    {
        extension(IdentityUserClaim<string> userClaim)
        {
            /// <summary>
            /// Converts a UserClaim to a ClaimDto.
            /// </summary>
            /// <returns>A ClaimDto representing the user claim.</returns>
            public ClaimDto ToDto()
            {
                return new ClaimDto
                {
                    Type = userClaim.ClaimType!,
                    Value = userClaim.ClaimValue!
                };
            }

            /// <summary>
            /// Converts a UserClaim to a UserClaimDto.
            /// </summary>
            /// <returns>A UserClaimDto representing the user claim.</returns>
            public UserClaimDto ToUserClaimDto()
            {
                return new UserClaimDto
                {
                    Type = userClaim.ClaimType!,
                    Value = userClaim.ClaimValue!,
                    UserId = userClaim.UserId
                };
            }
        }

        extension(IEnumerable<IdentityUserClaim<string>> claims)
        {
            /// <summary>
            /// Converts a collection of IdentityUserClaim objects to a list of ClaimDto objects.
            /// </summary>
            /// <returns>The list of converted ClaimDto objects.</returns>
            public List<ClaimDto> ToDtoList()
            {
                return [.. claims.Select(claim => claim.ToDto())];
            }

            /// <summary>
            /// Converts a collection of IdentityUserClaim objects to a list of UserClaimDto objects.
            /// </summary>
            /// <returns>The list of converted UserClaimDto objects.</returns>
            public List<UserClaimDto> ToUserClaimDtoList()
            {
                return [.. claims.Select(claim => claim.ToUserClaimDto())];
            }
        }
    }
}
