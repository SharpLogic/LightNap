using LightNap.Core.StaticContents.Models;

namespace LightNap.Core.StaticContents.Dto.Response
{
    /// <summary>
    /// The result of a request for a specific CMS item based on user permissions.
    /// </summary>
    public class PublishedStaticContentResultDto
    {
        /// <summary>
        /// The visibility state of the requested item for the current user.
        /// </summary>
        public required StaticContentUserVisibility Visibility { get; set; }

        /// <summary>
        /// The CMS item.
        /// </summary>
        public PublishedStaticContentDto? Content { get; set; }
    }
}