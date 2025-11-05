using LightNap.Core.StaticContents.Enums;

namespace LightNap.Core.StaticContents.Dto.Request
{
    public class UpdateStaticContentLanguageDto
    {
        public required string Content { get; set; }
        public required StaticContentFormat Format { get; set; }
    }
}