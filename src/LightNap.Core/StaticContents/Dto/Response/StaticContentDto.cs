using LightNap.Core.StaticContents.Enums;

namespace LightNap.Core.StaticContents.Dto.Response
{
    /// <summary>
    /// Data transfer object for static content response.
    /// Contains metadata and audit information about static content items.
    /// </summary>
    public class StaticContentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the static content.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Gets or sets the type of static content.
        /// </summary>
        public required StaticContentType Type { get; set; }

        /// <summary>
        /// Gets or sets the unique key identifier for the static content.
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// Gets or sets the current status of the static content.
        /// </summary>
        public required StaticContentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the read access level for the static content.
        /// </summary>
        public required StaticContentReadAccess ReadAccess { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of roles that can edit this content.
        /// </summary>
        public string? EditorRoles { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of roles that can read this content.
        /// </summary>
        public string? ReaderRoles { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the content was created.
        /// </summary>
        public required DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who created this content.
        /// </summary>
        public required string? CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the status was last changed.
        /// </summary>
        public required DateTime? StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who last changed the status.
        /// </summary>
        public required string? StatusChangedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the content was last modified.
        /// </summary>
        public required DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who last modified this content.
        /// </summary>
        public required string? LastModifiedByUserId { get; set; }
    }
}