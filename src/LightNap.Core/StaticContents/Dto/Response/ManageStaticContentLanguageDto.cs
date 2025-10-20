using LightNap.Core.Data.Entities;

namespace LightNap.Core.StaticContents.Dto.Response
{
    public class ManageStaticContentLanguageDto
    {
        public required int StaticContentId { get; set; }
        public required string Language { get; set; }
        public required string Content { get; set; }
        public required StaticContentFormat Format { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string? CreatedByUserId { get; set; }
        public required DateTime? LastModifiedDate { get; set; }
        public required string? LastModifiedUserId { get; set; }
    }
}