using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Configuration.StaticContents;
using LightNap.Core.StaticContents.Dto.Request;
using LightNap.Core.StaticContents.Dto.Response;
using LightNap.Core.StaticContents.Interfaces;
using LightNap.WebApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing static content resources.
    /// Provides endpoints for creating, retrieving, updating, and deleting static content and their language-specific versions.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting(WebConstants.RateLimiting.ContentPolicyName)]
    public class ContentController(IStaticContentService staticContentService) : ControllerBase
    {
        /// <summary>
        /// Retrieves published static content by key and language code.
        /// </summary>
        /// <param name="key">The unique identifier of the static content.</param>
        /// <param name="languageCode">The language code for the content (e.g., "en-US", "fr-FR").</param>
        /// <returns>The published static content result if found; otherwise null.</returns>
        [HttpGet("published/{key}/{languageCode}")]
        [AllowAnonymous]
        public async Task<ApiResponseDto<PublishedStaticContentResultDto?>> GetPublishedStaticContentAsync(string key, string languageCode)
        {
            return new ApiResponseDto<PublishedStaticContentResultDto?>(await staticContentService.GetPublishedStaticContentAsync(key, languageCode));
        }

        /// <summary>
        /// Retrieves the list of supported languages for static content.
        /// </summary>
        /// <returns>A read-only list of supported language configurations.</returns>
        [HttpGet("supported-languages")]
        [AllowAnonymous]
        [OutputCache(Duration = 3600)]
        public ApiResponseDto<IReadOnlyList<StaticContentSupportedLanguage>> GetSupportedLanguages()
        {
            return new ApiResponseDto<IReadOnlyList<StaticContentSupportedLanguage>>(staticContentService.GetSupportedLanguages());
        }

        /// <summary>
        /// Creates a new static content resource.
        /// </summary>
        /// <param name="createDto">The data transfer object containing the content to create.</param>
        /// <returns>The newly created static content.</returns>
        /// <remarks>Requires Administrator or ContentEditor role.</remarks>
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.ContentEditor}")]
        public async Task<ApiResponseDto<StaticContentDto>> CreateStaticContentAsync(CreateStaticContentDto createDto)
        {
            return new ApiResponseDto<StaticContentDto>(await staticContentService.CreateStaticContentAsync(createDto));
        }

        /// <summary>
        /// Retrieves static content by its key.
        /// </summary>
        /// <param name="key">The unique identifier of the static content.</param>
        /// <returns>The static content if found; otherwise null.</returns>
        [HttpGet("{key}")]
        public async Task<ApiResponseDto<StaticContentDto?>> GetStaticContentAsync(string key)
        {
            return new ApiResponseDto<StaticContentDto?>(await staticContentService.GetStaticContentAsync(key));
        }

        /// <summary>
        /// Searches for static content based on the provided criteria.
        /// </summary>
        /// <param name="searchDto">The search criteria and pagination parameters.</param>
        /// <returns>A paginated list of matching static content.</returns>
        /// <remarks>Requires Administrator or ContentEditor role.</remarks>
        [HttpPost("search")]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.ContentEditor}")]
        public async Task<ApiResponseDto<PagedResponseDto<StaticContentDto>>> SearchStaticContentAsync(SearchStaticContentRequestDto searchDto)
        {
            return new ApiResponseDto<PagedResponseDto<StaticContentDto>>(await staticContentService.SearchStaticContentAsync(searchDto));
        }

        /// <summary>
        /// Updates an existing static content resource.
        /// </summary>
        /// <param name="key">The unique identifier of the static content to update.</param>
        /// <param name="updateDto">The data transfer object containing the updated content.</param>
        /// <returns>The updated static content.</returns>
        [HttpPut("{key}")]
        public async Task<ApiResponseDto<StaticContentDto>> UpdateStaticContentAsync(string key, UpdateStaticContentDto updateDto)
        {
            return new ApiResponseDto<StaticContentDto>(await staticContentService.UpdateStaticContentAsync(key, updateDto));
        }

        /// <summary>
        /// Deletes a static content resource by its key.
        /// </summary>
        /// <param name="key">The unique identifier of the static content to delete.</param>
        /// <returns>True if the deletion was successful.</returns>
        /// <remarks>Requires Administrator role.</remarks>
        [HttpDelete("{key}")]
        [Authorize(Roles = Constants.Roles.Administrator)]
        public async Task<ApiResponseDto<bool>> DeleteStaticContentAsync(string key)
        {
            await staticContentService.DeleteStaticContentAsync(key);
            return new ApiResponseDto<bool>(true);
        }

        /// <summary>
        /// Retrieves a specific language version of static content.
        /// </summary>
        /// <param name="key">The unique identifier of the static content.</param>
        /// <param name="languageCode">The language code for the content version.</param>
        /// <returns>The language-specific static content if found; otherwise null.</returns>
        [HttpGet("{key}/languages/{languageCode}")]
        public async Task<ApiResponseDto<StaticContentLanguageDto?>> GetStaticContentLanguageAsync(string key, string languageCode)
        {
            return new ApiResponseDto<StaticContentLanguageDto?>(await staticContentService.GetStaticContentLanguageAsync(key, languageCode));
        }

        /// <summary>
        /// Retrieves all language versions of a static content resource.
        /// </summary>
        /// <param name="key">The unique identifier of the static content.</param>
        /// <returns>A read-only list of all language versions.</returns>
        [HttpGet("{key}/languages")]
        public async Task<ApiResponseDto<IReadOnlyList<StaticContentLanguageDto>>> GetStaticContentLanguagesAsync(string key)
        {
            return new ApiResponseDto<IReadOnlyList<StaticContentLanguageDto>>(await staticContentService.GetStaticContentLanguagesAsync(key));
        }

        /// <summary>
        /// Creates a new language version for an existing static content resource.
        /// </summary>
        /// <param name="key">The unique identifier of the static content.</param>
        /// <param name="languageCode">The language code for the new version.</param>
        /// <param name="createDto">The data transfer object containing the language-specific content.</param>
        /// <returns>The newly created language-specific static content.</returns>
        [HttpPost("{key}/languages/{languageCode}")]
        public async Task<ApiResponseDto<StaticContentLanguageDto>> CreateStaticContentLanguageAsync(string key, string languageCode, CreateStaticContentLanguageDto createDto)
        {
            return new ApiResponseDto<StaticContentLanguageDto>(await staticContentService.CreateStaticContentLanguageAsync(key, languageCode, createDto));
        }

        /// <summary>
        /// Updates a language version of static content.
        /// </summary>
        /// <param name="key">The unique identifier of the static content.</param>
        /// <param name="languageCode">The language code of the version to update.</param>
        /// <param name="updateDto">The data transfer object containing the updated language-specific content.</param>
        /// <returns>The updated language-specific static content.</returns>
        [HttpPut("{key}/languages/{languageCode}")]
        public async Task<ApiResponseDto<StaticContentLanguageDto>> UpdateStaticContentLanguageAsync(string key, string languageCode, UpdateStaticContentLanguageDto updateDto)
        {
            return new ApiResponseDto<StaticContentLanguageDto>(await staticContentService.UpdateStaticContentLanguageAsync(key, languageCode, updateDto));
        }

        /// <summary>
        /// Deletes a language version of static content.
        /// </summary>
        /// <param name="key">The unique identifier of the static content.</param>
        /// <param name="languageCode">The language code of the version to delete.</param>
        /// <returns>True if the deletion was successful.</returns>
        /// <remarks>Requires Administrator role.</remarks>
        [HttpDelete("{key}/languages/{languageCode}")]
        [Authorize(Roles = Constants.Roles.Administrator)]
        public async Task<ApiResponseDto<bool>> DeleteStaticContentLanguageAsync(string key, string languageCode)
        {
            await staticContentService.DeleteStaticContentLanguageAsync(key, languageCode);
            return new ApiResponseDto<bool>(true);
        }
    }
}