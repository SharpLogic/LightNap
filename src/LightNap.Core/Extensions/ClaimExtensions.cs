using LightNap.Core.Identity.Dto.Response;
using System.Security.Claims;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for converting Claim objects to ClaimDto objects.
    /// </summary>
    internal static class ClaimExtensions
    {
        extension(Claim claim)
        {
            /// <summary>
            /// Converts a Claim object to a ClaimDto object.
            /// </summary>
            /// <returns>A ClaimDto object.</returns>
            public ClaimDto ToDto()
            {
                return new ClaimDto()
                {
                    Type = claim.Type,
                    Value = claim.Value,
                };
            }
        }

        extension(ClaimDto claimDto)
        {
            /// <summary>
            /// Converts the current <see cref="ClaimDto"/> instance to a <see cref="Claim"/> object.
            /// </summary>
            /// <returns>A <see cref="Claim"/> object with the same type and value as the <see cref="ClaimDto"/>.</returns>
            public Claim ToClaim()
            {
                return new Claim(claimDto.Type, claimDto.Value);
            }
        }

        extension(IEnumerable<Claim> claims)
        {
            /// <summary>
            /// Converts a collection of Claim objects to a list of ClaimDto objects.
            /// </summary>
            /// <returns>A list of ClaimDto objects.</returns>
            public List<ClaimDto> ToDtoList()
            {
                return claims.Select(claim => claim.ToDto()).ToList();
            }
        }
    }
}
