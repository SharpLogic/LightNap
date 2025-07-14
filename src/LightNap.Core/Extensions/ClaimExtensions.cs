using LightNap.Core.Identity.Dto.Response;
using System.Security.Claims;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for converting Claim objects to ClaimDto objects.
    /// </summary>
    public static class ClaimExtensions
    {
        /// <summary>
        /// Converts a Claim object to a ClaimDto object.
        /// </summary>
        /// <param name="claim">The Claim object to convert.</param>
        /// <returns>A ClaimDto object.</returns>
        public static ClaimDto ToDto(this Claim claim)
        {
            return new ClaimDto()
            {
                Type = claim.Type,
                Value = claim.Value,
            };
        }

        /// <summary>
        /// Converts the current <see cref="ClaimDto"/> instance to a <see cref="Claim"/> object.
        /// </summary>
        /// <param name="claimDto">The <see cref="ClaimDto"/> instance to convert. Cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="Claim"/> object with the same type and value as the <paramref name="claimDto"/>.</returns>
        public static Claim ToClaim(this ClaimDto claimDto)
        {
            return new Claim(claimDto.Type, claimDto.Value);
        }

        /// <summary>
        /// Converts a collection of Claim objects to a list of ClaimDto objects.
        /// </summary>
        /// <param name="claims">The collection of Claim objects to convert.</param>
        /// <returns>A list of ClaimDto objects.</returns>
        public static List<ClaimDto> ToDtoList(this IEnumerable<Claim> claims)
        {
            return claims.Select(claim => claim.ToDto()).ToList();
        }
    }
}