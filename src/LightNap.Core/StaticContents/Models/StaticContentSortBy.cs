namespace LightNap.Core.StaticContents.Models
{
    /// <summary>
    /// Supported static content sorting options.
    /// </summary>
    public enum StaticContentSortBy
    {
        /// <summary>
        /// Sort by the content key identifier.
        /// </summary>
        Key,

        /// <summary>
        /// Sort by the content status.
        /// </summary>
        Status,

        /// <summary>
        /// Sort by the content type.
        /// </summary>
        Type,

        /// <summary>
        /// Sort by the read access level.
        /// </summary>
        ReadAccess,

        /// <summary>
        /// Sort by the creation date.
        /// </summary>
        CreatedDate,

        /// <summary>
        /// Sort by the last modified date.
        /// </summary>
        LastModifiedDate
    }
}
