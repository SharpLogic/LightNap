using LightNap.Core.Data.Entities;

namespace LightNap.Core.StaticContents.Dto.Request
{
    public class CreateStaticContentDto : UpdateStaticContentDto
    {
        public required StaticContentType Type { get; set; }
    }
}