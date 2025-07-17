using LightNap.Core.Data.Entities;
using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for converting RefreshToken objects to DeviceDto objects.
    /// </summary>
    public static class RefreshTokenExtensions
    {
        /// <summary>
        /// Converts a RefreshToken object to a DeviceDto object.
        /// </summary>
        /// <param name="token">The RefreshToken object to convert.</param>
        /// <returns>The converted DeviceDto object.</returns>
        public static DeviceDto ToDto(this RefreshToken token)
        {
            return new DeviceDto()
            {
                Details = token.Details,
                Id = token.Id,
                LastSeen = token.LastSeen,
                IpAddress = token.IpAddress,
            };
        }

        /// <summary>
        /// Converts a collection of RefreshToken objects to a list of DeviceDto objects.
        /// </summary>
        /// <param name="tokens">The collection of RefreshToken objects to convert.</param>
        /// <returns>The list of converted DeviceDto objects.</returns>
        public static List<DeviceDto> ToDtoList(this IEnumerable<RefreshToken> tokens)
        {
            return tokens.Select(token => token.ToDto()).ToList();
        }
    }
}