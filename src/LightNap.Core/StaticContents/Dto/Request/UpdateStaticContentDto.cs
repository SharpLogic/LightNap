using LightNap.Core.Configuration;
using LightNap.Core.StaticContents.Enums;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.StaticContents.Dto.Request
{
    /// <summary>
    /// Data transfer object for updating static content.
    /// </summary>
    public class UpdateStaticContentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the static content.
        /// Must be in kebab-case (lowercase alphanumeric with hyphens).
        /// </summary>
        [RegularExpression(@"^[a-z0-9]+(-[a-z0-9]+)*$", ErrorMessage = "Static content key must be lowercase alphanumeric with hyphens (kebab-case), cannot start/end with hyphen, and cannot contain consecutive hyphens.")]
        [Length(1, Constants.Dto.MaxStaticContentKeyLength)]
        public required string Key { get; set; }

        /// <summary>
        /// Gets or sets the type of static content.
        /// </summary>
        public required StaticContentType Type { get; set; }

        /// <summary>
        /// Gets or sets the publication status of the static content.
        /// </summary>
        public required StaticContentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the read access level for the static content.
        /// </summary>
        public required StaticContentReadAccess ReadAccess { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of role names that can edit this content.
        /// Role names must be alphanumeric with hyphens and underscores only.
        /// </summary>
        [RegularExpression(@"^[a-zA-Z0-9_-]+(,[a-zA-Z0-9_-]+)*$", ErrorMessage = "Editor roles must be a comma-separated list of valid role names (alphanumeric, hyphens, and underscores only).")]
        [MaxLength(256)]
        public string? EditorRoles { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of role names that can view this content.
        /// Role names must be alphanumeric with hyphens and underscores only.
        /// </summary>
        [RegularExpression(@"^[a-zA-Z0-9_-]+(,[a-zA-Z0-9_-]+)*$", ErrorMessage = "Viewer roles must be a comma-separated list of valid role names (alphanumeric, hyphens, and underscores only).")]
        [MaxLength(256)]
        public string? ReaderRoles { get; set; }
    }
}