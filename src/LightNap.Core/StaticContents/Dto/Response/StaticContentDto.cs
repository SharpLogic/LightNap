using LightNap.Core.Data.Entities;

namespace LightNap.Core.StaticContents.Dto.Response
{
    public class StaticContentDto
    {
        public required int Id { get; set; }
        public required StaticContentType Type { get; set; }
        public required string Key { get; set; }
        public required StaticContentStatus Status { get; set; }
        public required StaticContentReadAccess ReadAccess { get; set; }
        public string? EditorRoles { get; set; }
        public string? ReaderRoles { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string? CreatedByUserId { get; set; }
        public required DateTime? StatusChangedDate { get; set; }
        public required string? StatusChangedByUserId { get; set; }
        public required DateTime? LastModifiedDate { get; set; }
        public required string? LastModifiedByUserId { get; set; }
    }
}