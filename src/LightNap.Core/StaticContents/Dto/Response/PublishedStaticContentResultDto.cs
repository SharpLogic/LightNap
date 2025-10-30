using LightNap.Core.StaticContents.Models;

namespace LightNap.Core.StaticContents.Dto.Response
{
    public class PublishedStaticContentResultDto
    {
        public required StaticContentUserVisibility Visibility { get; set; }
        public PublishedStaticContentDto? Content { get; set; }
    }
}