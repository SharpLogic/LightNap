using System.Collections.ObjectModel;

namespace LightNap.Core.Configuration
{
    /// <summary>
    /// Provides configuration and lookup for all static content settings in the system.
    /// </summary>
    internal static class StaticContentConfig
    {
        /// <summary>
        /// Change this to select a different fallback language when the selected option is not available.
        /// See StaticContentConfig.SupportedLanguages for available options (and to add more).
        /// </summary>
        public const string DefaultLanguageCode = "en";

        /// <summary>
        /// Defines the list of roles that have global permissions to create and manage static content.
        /// </summary>
        public static IReadOnlyList<string> ContentAdministratorRoles =>
        [
            Constants.Roles.Administrator,
            Constants.Roles.ContentEditor,
        ];

        /// <summary>
        /// The list of all supported static content languages. These are the languages you can localize static content into.
        /// </summary>
        public static IReadOnlyList<StaticContentSupportedLanguage> SupportedLanguages =>
        [
            new StaticContentSupportedLanguage("en", "English"),
            new StaticContentSupportedLanguage("fr", "French"),
            new StaticContentSupportedLanguage("es", "Spanish"),
        ];

        /// <summary>
        /// A lookup dictionary for supported static content languages by their language code.
        /// </summary>
        public static ReadOnlyDictionary<string, StaticContentSupportedLanguage> SupportedLanguagesLookup { get; } =
            new ReadOnlyDictionary<string, StaticContentSupportedLanguage>(StaticContentConfig.SupportedLanguages.ToDictionary(l => l.LanguageCode));
    }
}
