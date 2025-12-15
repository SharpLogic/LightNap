using LightNap.Core.Configuration;
using LightNap.Core.StaticContents.Enums;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a CMS content item, including its type, status, access permissions, and metadata such as creation
    /// and modification details.
    /// </summary>
    public class StaticContent
    {
        /// <summary>
        /// Gets or sets the unique identifier for the static content.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the type of static content.
        /// </summary>
        public required StaticContentType Type { get; set; }

        /// <summary>
        /// Gets or sets the unique key for identifying the static content.
        /// </summary>
        [MaxLength(Constants.Dto.MaxStaticContentKeyLength)]
        public required string Key { get; set; }

        /// <summary>
        /// Gets or sets the current status of the static content.
        /// </summary>
        public required StaticContentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the status was last changed.
        /// </summary>
        public DateTime? StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the person who last changed the status.
        /// </summary>
        public string? StatusChangedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the user who last changed the status.
        /// </summary>
        public ApplicationUser? StatusChangedByUser { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the static content was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the user ID of the person who created the static content.
        /// </summary>
        public string? CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the user who created the static content.
        /// </summary>
        public ApplicationUser? CreatedByUser { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the static content was last modified.
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the person who last modified the static content.
        /// </summary>
        public string? LastModifiedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the static content.
        /// </summary>
        public ApplicationUser? LastModifiedByUser { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of roles that can edit this static content.
        /// </summary>
        public string? EditorRoles { get; set; }

        /// <summary>
        /// Gets or sets the read access level for this static content.
        /// </summary>
        public required StaticContentReadAccess ReadAccess { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of roles that can read this static content.
        /// </summary>
        public string? ReaderRoles { get; set; }

        /// <summary>
        /// Gets or sets the collection of language versions for this static content.
        /// </summary>
        public ICollection<StaticContentLanguage>? Languages { get; set; }
    }
}