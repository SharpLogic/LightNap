using LightNap.Core.Api;
using LightNap.Core.StaticContents.Enums;
using LightNap.Core.StaticContents.Models;

namespace LightNap.Core.StaticContents.Dto.Request
{
    public class SearchStaticContentRequestDto : PagedRequestDtoBase
    {
        /// <summary>
        /// Substring key search filter.
        /// </summary>
        public string? KeyContains { get; set; }

        /// <summary>
        /// Filter for read access.
        /// </summary>
        public StaticContentReadAccess? ReadAccess { get; set; }

        /// <summary>
        /// Filter for status.
        /// </summary>
        public StaticContentStatus? Status { get; set; }

        /// <summary>
        /// Filter for type.
        /// </summary>
        public StaticContentType? Type { get; set; }

        /// <summary>
        /// Sorting order for the results.
        /// </summary>
        public StaticContentSortBy SortBy { get; set; }

        /// <summary>
        /// Reverses the default sort behavior of the SortBy field.
        /// </summary>
        public bool ReverseSort { get; set; }
    }
}