using LightNap.Core.Data.Entities;

namespace LightNap.Core.StaticContents.Dto.Request
{
    public class UpdateStaticContentLanguageDto
    {
        public required string Language { get; set; }
        public required string Content { get; set; }
        public required StaticContentFormat Format { get; set; }
    }
}