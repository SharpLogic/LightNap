using LightNap.Core.Data.Entities;
using LightNap.Core.Identity.Dto.Response;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for converting RefreshToken objects to DeviceDto objects.
    /// </summary>
    internal static class RefreshTokenExtensions
    {
        extension(RefreshToken token)
        {
            /// <summary>
            /// Converts a RefreshToken object to a DeviceDto object.
            /// </summary>
            /// <returns>The converted DeviceDto object.</returns>
            public DeviceDto ToDto()
            {
                return new DeviceDto()
                {
                    Details = token.Details,
                    Id = token.Id,
                    LastSeen = token.LastSeen,
                    IpAddress = token.IpAddress,
                };
            }
        }

        extension(IEnumerable<RefreshToken> tokens)
        {
            /// <summary>
            /// Converts a collection of RefreshToken objects to a list of DeviceDto objects.
            /// </summary>
            /// <returns>The list of converted DeviceDto objects.</returns>
            public List<DeviceDto> ToDtoList()
            {
                return tokens.Select(token => token.ToDto()).ToList();
            }
        }
    }
}
