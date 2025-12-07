using LightNap.Core.Api;

namespace LightNap.Core.Identity.Dto.Request
{
    /// <summary>
    /// Represents a request to search external logins.
    /// </summary>
    public class SearchExternalLoginsRequestDto : PagedRequestDtoBase
    {
        /// <summary>
        /// The optional login provider to filter by.
        /// </summary>
        public string? LoginProvider { get; set; }

        /// <summary>
        /// The optional user ID to filter by.
        /// </summary>
        public string? UserId { get; set; }
    }
}