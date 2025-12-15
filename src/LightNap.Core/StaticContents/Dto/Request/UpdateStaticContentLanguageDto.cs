using LightNap.Core.StaticContents.Enums;

namespace LightNap.Core.StaticContents.Dto.Request
{
    /// <summary>
    /// Specifies the settings to update the exiting language content of a CMS item with.
    /// </summary>
    public class UpdateStaticContentLanguageDto
    {
        /// <summary>
        /// The content.
        /// </summary>
        public required string Content { get; set; }

        /// <summary>
        /// The format of the content.
        /// </summary>
        public required StaticContentFormat Format { get; set; }
    }
}