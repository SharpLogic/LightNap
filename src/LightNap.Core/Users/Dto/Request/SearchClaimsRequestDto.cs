using LightNap.Core.Api;

namespace LightNap.Core.Users.Dto.Request
{
    /// <summary>
    /// Represents a request to search claims.
    /// </summary>
    public class SearchClaimsRequestDto : PaginationRequestDtoBase
    {
        /// <summary>
        /// Filter by user.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Filter by claim type.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Filter by claim value.
        /// </summary>
        public string? Value { get; set; }
    }
}