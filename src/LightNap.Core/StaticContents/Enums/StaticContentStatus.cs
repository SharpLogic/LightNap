namespace LightNap.Core.StaticContents.Enums
{
    /// <summary>
    /// Represents the publication status of static content.
    /// </summary>
    public enum StaticContentStatus
    {
        /// <summary>
        /// Content is in draft state and not yet published.
        /// </summary>
        Draft = 0,

        /// <summary>
        /// Content is published and visible to users.
        /// </summary>
        Published = 1,

        /// <summary>
        /// Content is archived and no longer active.
        /// </summary>
        Archived = 2,
    }
}
