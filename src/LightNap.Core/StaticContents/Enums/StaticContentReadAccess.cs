namespace LightNap.Core.StaticContents.Enums
{
    /// <summary>
    /// The read access of a CMS item.
    /// </summary>
    public enum StaticContentReadAccess
    {
        /// <summary>
        /// Anyone may request the content.
        /// </summary>
        Public = 0,

        /// <summary>
        /// Users must be logged in to request the content.
        /// </summary>
        Authenticated = 1,

        /// <summary>
        /// The content is only visible to specified roles and users.
        /// </summary>
        Explicit = 2,
    }
}
