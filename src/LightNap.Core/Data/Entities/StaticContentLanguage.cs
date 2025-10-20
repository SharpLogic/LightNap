using Microsoft.EntityFrameworkCore;

namespace LightNap.Core.Data.Entities
{
    [PrimaryKey(nameof(StaticContentId), nameof(Language))]
    public class StaticContentLanguage
    {
        public required int StaticContentId { get; set; }
        public StaticContent? StaticContent { get; set; }

        public required string Language { get; set; }
        
        public required string Content { get; set; }
        public required StaticContentFormat ContentType { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedUserId { get; set; }
        public ApplicationUser? LastModifiedUser { get; set; }
    }
}