namespace LightNap.Core.Users.Models
{
    /// <summary>
    /// Supported user sorting options.
    /// </summary>
    public enum ApplicationUserSortBy
    {
        /// <summary>
        /// Sort by user email address.
        /// </summary>
        Email,

        /// <summary>
        /// Sort by user name.
        /// </summary>
        UserName,

        /// <summary>
        /// Sort by creation date.
        /// </summary>
        CreatedDate,

        /// <summary>
        /// Sort by last modification date.
        /// </summary>
        LastModifiedDate
    }
}
