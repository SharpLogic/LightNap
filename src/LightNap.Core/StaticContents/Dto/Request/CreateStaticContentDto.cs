using LightNap.Core.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.StaticContents.Dto.Request
{
    public class CreateStaticContentDto : UpdateStaticContentDto
    {
        public required StaticContentType Type { get; set; }
    }
}