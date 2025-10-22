using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.StaticContents.Dto.Request;
using LightNap.Core.StaticContents.Dto.Response;

namespace LightNap.Core.StaticContents.Interfaces
{
    /// <summary>
    /// Service interface for managing static content and language variants.
    /// </summary>
    public interface IStaticContentService
    {
        /// <summary>
        /// Retrieves published static content by key and language code.
        /// Returns null if content is not found, not published, or user lacks required permissions.
        /// Automatically falls back to default language ("en") if requested language is not available.
        /// </summary>
        /// <param name="key">The unique key identifying the static content.</param>
        /// <param name="languageCode">The language code for the desired content variant.</param>
        /// <returns>The published content in the requested language, or null if not accessible.</returns>
        Task<PublishedStaticContentDto?> GetPublishedStaticContentAsync(string key, string languageCode);

        /// <summary>
        /// Retrieves static content metadata by key. Requires authentication and edit permissions.
        /// </summary>
        /// <param name="key">The unique key identifying the static content.</param>
        /// <returns>The static content metadata, or null if not found.</returns>
        Task<StaticContentDto?> GetStaticContentAsync(string key);

        /// <summary>
        /// Searches for static content with filtering, sorting, and pagination.
        /// Requires global creator permissions.
        /// </summary>
        /// <param name="searchDto">The search criteria including filters, sorting, and pagination options.</param>
        /// <returns>A paged response containing matching static content items.</returns>
        Task<PagedResponseDto<StaticContentDto>> SearchStaticContentAsync(SearchStaticContentRequestDto searchDto);

        /// <summary>
        /// Creates a new static content item. Requires global creator permissions.
        /// </summary>
        /// <param name="createDto">The data for creating the static content.</param>
        /// <returns>The created static content metadata.</returns>
        Task<StaticContentDto> CreateStaticContentAsync(CreateStaticContentDto createDto);

        /// <summary>
        /// Updates an existing static content item. Requires edit permissions for the specific content.
        /// </summary>
        /// <param name="key">The unique key identifying the static content to update.</param>
        /// <param name="updateDto">The updated data for the static content.</param>
        /// <returns>The updated static content metadata.</returns>
        Task<StaticContentDto> UpdateStaticContentAsync(string key, UpdateStaticContentDto updateDto);

        /// <summary>
        /// Deletes a static content item and all its language variants. Requires global creator permissions.
        /// </summary>
        /// <param name="key">The unique key identifying the static content to delete.</param>
        Task DeleteStaticContentAsync(string key);

        /// <summary>
        /// Retrieves a specific language variant of static content. Requires edit permissions.
        /// </summary>
        /// <param name="key">The unique key identifying the static content.</param>
        /// <param name="languageCode">The language code for the desired variant.</param>
        /// <returns>The language-specific content, or null if the variant doesn't exist.</returns>
        Task<StaticContentLanguageDto?> GetStaticContentLanguageAsync(string key, string languageCode);

        /// <summary>
        /// Retrieves all language variants for a static content item. Requires edit permissions.
        /// </summary>
        /// <param name="key">The unique key identifying the static content.</param>
        /// <returns>A read-only list of all language variants for the content.</returns>
        Task<IReadOnlyList<StaticContentLanguageDto>> GetStaticContentLanguagesAsync(string key);

        /// <summary>
        /// Creates a new language variant for existing static content. Requires edit permissions.
        /// </summary>
        /// <param name="key">The unique key identifying the static content.</param>
        /// <param name="languageCode">The language code for the new variant.</param>
        /// <param name="createDto">The content data for the language variant.</param>
        /// <returns>The created language variant.</returns>
        Task<StaticContentLanguageDto> CreateStaticContentLanguageAsync(string key, string languageCode, CreateStaticContentLanguageDto createDto);

        /// <summary>
        /// Updates an existing language variant of static content. Requires edit permissions.
        /// </summary>
        /// <param name="key">The unique key identifying the static content.</param>
        /// <param name="languageCode">The language code for the variant to update.</param>
        /// <param name="updateDto">The updated content data.</param>
        /// <returns>The updated language variant.</returns>
        Task<StaticContentLanguageDto> UpdateStaticContentLanguageAsync(string key, string languageCode, UpdateStaticContentLanguageDto updateDto);

        /// <summary>
        /// Deletes a specific language variant of static content. Requires edit permissions.
        /// </summary>
        /// <param name="key">The unique key identifying the static content.</param>
        /// <param name="languageCode">The language code for the variant to delete.</param>
        Task DeleteStaticContentLanguageAsync(string key, string languageCode);

        /// <summary>
        /// Retrieves the list of supported language codes for static content.
        /// </summary>
        /// <returns>A read-only list of supported languages.</returns>
        IReadOnlyList<StaticContentSupportedLanguage> GetSupportedLanguages();
    }
}