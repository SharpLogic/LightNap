using LightNap.Core.StaticContents.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a localized version of static content for a specific language.
    /// </summary>
    [PrimaryKey(nameof(StaticContentId), nameof(LanguageCode))]
    public class StaticContentLanguage
    {
        /// <summary>
        /// Gets or sets the identifier of the static content.
        /// </summary>
        public required int StaticContentId { get; set; }

        /// <summary>
        /// Gets or sets the related static content entity.
        /// </summary>
        public StaticContent? StaticContent { get; set; }

        /// <summary>
        /// Gets or sets the language code (e.g., "en-US", "fr-FR").
        /// </summary>
        [MaxLength(16)]
        public required string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the localized content text.
        /// </summary>
        public required string Content { get; set; }

        /// <summary>
        /// Gets or sets the format of the content.
        /// </summary>
        public required StaticContentFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the creation date in UTC.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the identifier of the user who created this entry.
        /// </summary>
        public string? CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the user who created this entry.
        /// </summary>
        public ApplicationUser? CreatedByUser { get; set; }

        /// <summary>
        /// Gets or sets the last modification date in UTC.
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified this entry.
        /// </summary>
        public string? LastModifiedUserId { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified this entry.
        /// </summary>
        public ApplicationUser? LastModifiedUser { get; set; }
    }
}