using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Api
{
    /// <summary>
    /// Base class for pagination requests.
    /// </summary>
    public class PagedRequestDtoBase
    {
        private const int _maxPageSize = 50;

        /// <summary>
        /// Gets or sets the page number. Must be greater than 0.
        /// </summary>
        /// <value>The page number.</value>
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the page size. Must be between 1 and 50.
        /// </summary>
        /// <value>The page size.</value>
        [Range(1, PagedRequestDtoBase._maxPageSize, ErrorMessage = "Page size must be between 1 and 50.")]
        public int PageSize { get; set; } = 10;
    }
}