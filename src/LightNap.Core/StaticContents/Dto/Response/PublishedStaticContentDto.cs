using LightNap.Core.StaticContents.Enums;

namespace LightNap.Core.StaticContents.Dto.Response
{
    /// <summary>
    /// An end-user-ready CMS item.
    /// </summary>
    public class PublishedStaticContentDto
    {
        /// <summary>
        /// The content.
        /// </summary>
        public required string Content { get; set; }

        /// <summary>
        /// The format of the content.
        /// </summary>
        public required StaticContentFormat Format { get; set; }
    }
}