namespace LightNap.Core.StaticContents.Models
{
    /// <summary>
    /// Describes the visibility level of static content to the current user.
    /// </summary>
    public enum StaticContentUserVisibility
    {
        RequiresAuthentication = 0,
        Restricted = 1,
        Reader = 2,
        Editor = 3,
    }
}
