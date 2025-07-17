namespace LightNap.Core.Api
{
    /// <summary>
    /// Represents a paginated response.
    /// </summary>
    /// <typeparam name="T">The type of data in the response.</typeparam>
    public class PagedResponseDto<T>
    {
        /// <summary>
        /// Gets or sets the data for the current page.
        /// </summary>
        public IList<T> Data { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total count of items.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResponseDto{T}"/> class.
        /// </summary>
        /// <param name="data">The data for the current page.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="totalCount">The total count of items.</param>
        public PagedResponseDto(IList<T> data, int pageNumber, int pageSize, int totalCount)
        {
            this.Data = data;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
        }
    }

}
