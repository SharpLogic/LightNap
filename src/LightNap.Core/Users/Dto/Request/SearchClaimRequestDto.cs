using LightNap.Core.Api;

namespace LightNap.Core.Users.Dto.Request
{
    /// <summary>
    /// Represents a request to search a specific claim.
    /// </summary>
    public class SearchClaimRequestDto : PagedRequestDtoBase
    {
        /// <summary>
        /// Filter by exact claim type.
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// Filter by exact claim value.
        /// </summary>
        public required string Value { get; set; }
    }
}