namespace LightNap.Core.StaticContents.Enums
{
    /// <summary>
    /// Supported CMS content formats.
    /// </summary>
    public enum StaticContentFormat
    {
        /// <summary>
        /// Renders as HTML.
        /// </summary>
        Html = 0,

        /// <summary>
        /// Renders as Markdown.
        /// </summary>
        Markdown = 1,

        /// <summary>
        /// Renders as plaintext (or used in other ways).
        /// </summary>
        PlainText = 2,
    }
}
