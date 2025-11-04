using LightNap.Core.StaticContents.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Data.Entities
{
    [PrimaryKey(nameof(StaticContentId), nameof(LanguageCode))]
    public class StaticContentLanguage
    {
        public required int StaticContentId { get; set; }
        public StaticContent? StaticContent { get; set; }

        [MaxLength(16)]
        public required string LanguageCode { get; set; }

        public required string Content { get; set; }
        public required StaticContentFormat Format { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedUserId { get; set; }
        public ApplicationUser? LastModifiedUser { get; set; }
    }
}