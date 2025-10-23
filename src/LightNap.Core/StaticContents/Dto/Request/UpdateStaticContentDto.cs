using LightNap.Core.Configuration;
using LightNap.Core.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.StaticContents.Dto.Request
{
    public class UpdateStaticContentDto
    {
        [RegularExpression(@"^[a-z0-9]+(-[a-z0-9]+)*$", ErrorMessage = "Static content key must be lowercase alphanumeric with hyphens (kebab-case), cannot start/end with hyphen, and cannot contain consecutive hyphens.")]
        [Length(1, Constants.Dto.MaxStaticContentKeyLength)]
        public required string Key { get; set; }
        public required StaticContentStatus Status { get; set; }
        public required StaticContentReadAccess ReadAccess { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_-]+(,[a-zA-Z0-9_-]+)*$", ErrorMessage = "Editor roles must be a comma-separated list of valid role names (alphanumeric, hyphens, and underscores only).")]
        [MaxLength(256)]
        public string? EditorRoles { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_-]+(,[a-zA-Z0-9_-]+)*$", ErrorMessage = "Viewer roles must be a comma-separated list of valid role names (alphanumeric, hyphens, and underscores only).")]
        [MaxLength(256)]
        public string? ViewerRoles { get; set; }
    }
}