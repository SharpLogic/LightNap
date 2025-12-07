namespace LightNap.Core.Configuration.StaticContents
{
    /// <summary>
    /// Defines a supported language for static content.
    /// </summary>
    /// <param name="LanguageCode">The language shortcode, such as "en".</param>
    /// <param name="LanguageName">The full name of the language, such as "English".</param>
    public record StaticContentSupportedLanguage(string LanguageCode, string LanguageName);
}
