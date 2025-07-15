using LightNap.Core.Api;

namespace LightNap.Core.Users.Dto.Request
{
    /// <summary>
    /// Represents a request to search claims.
    /// </summary>
    public class SearchClaimsRequestDto : PagedRequestDtoBase
    {
        /// <summary>
        /// Filter by exact claim type.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Filter by claim type substring.
        /// </summary>
        public string? TypeContains { get; set; }

        /// <summary>
        /// Filter by exact claim value.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Filter by claim value substring.
        /// </summary>
        public string? ValueContains { get; set; }
    }
}