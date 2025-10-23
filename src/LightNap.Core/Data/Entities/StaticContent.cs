using LightNap.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Data.Entities
{
    public class StaticContent
    {
        public int Id { get; set; }

        public required StaticContentType Type { get; set; }

        [MaxLength(Constants.Dto.MaxStaticContentKeyLength)]
        public required string Key { get; set; }

        public required StaticContentStatus Status { get; set; }
        public DateTime? StatusChangedDate { get; set; }
        public string? StatusChangedByUserId { get; set; }
        public ApplicationUser? StatusChangedByUser { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedByUserId { get; set; }
        public ApplicationUser? LastModifiedByUser { get; set; }

        public string? EditorRoles { get; set; }

        public required StaticContentReadAccess ReadAccess { get; set; }
        public string? ReaderRoles { get; set; }

        public ICollection<StaticContentLanguage>? Languages { get; set; }
    }
}