namespace LightNap.Core.StaticContents.Dto.Response
{
    /// <summary>
    /// Data transfer object for static content in a specific language.
    /// Extends <see cref="PublishedStaticContentDto"/> with language-specific metadata.
    /// </summary>
    public class StaticContentLanguageDto : PublishedStaticContentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the parent static content.
        /// </summary>
        public required int StaticContentId { get; set; }

        /// <summary>
        /// Gets or sets the language code (e.g., "en-US", "fr-FR").
        /// </summary>
        public required string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this language version was created.
        /// </summary>
        public required DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID of who created this language version.
        /// </summary>
        public required string? CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this language version was last modified.
        /// </summary>
        public required DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID of who last modified this language version.
        /// </summary>
        public required string? LastModifiedUserId { get; set; }
    }
}