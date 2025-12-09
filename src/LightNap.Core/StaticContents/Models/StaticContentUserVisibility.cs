namespace LightNap.Core.StaticContents.Models
{
    /// <summary>
    /// Describes the visibility level of static content to the current user.
    /// </summary>
    public enum StaticContentUserVisibility
    {
        /// <summary>
        /// The user must authenticate before accessing the content.
        /// </summary>
        RequiresAuthentication = 0,

        /// <summary>
        /// The user is not authorized to access the content.
        /// </summary>
        Restricted = 1,

        /// <summary>
        /// User has read-only access to the content.
        /// </summary>
        Reader = 2,

        /// <summary>
        /// User has full edit access to the content.
        /// </summary>
        Editor = 3,
    }
}
