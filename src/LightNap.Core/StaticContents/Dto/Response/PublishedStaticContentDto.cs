using LightNap.Core.StaticContents.Enums;

namespace LightNap.Core.StaticContents.Dto.Response
{
    public class PublishedStaticContentDto
    {
        public required string Content { get; set; }
        public required StaticContentFormat Format { get; set; }
    }
}