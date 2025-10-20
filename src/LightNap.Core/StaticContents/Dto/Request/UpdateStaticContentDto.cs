using LightNap.Core.Configuration;
using LightNap.Core.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.StaticContents.Dto.Request
{
    public class UpdateStaticContentDto
    {
        [MaxLength(Constants.Dto.MaxStaticContentKeyLength)]
        public required string Key { get; set; }
        public required StaticContentStatus Status { get; set; }
    }
}