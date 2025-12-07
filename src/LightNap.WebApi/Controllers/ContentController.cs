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
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting(WebConstants.RateLimiting.ContentPolicyName)]
    public class ContentController(IStaticContentService staticContentService) : ControllerBase
    {
        [HttpGet("published/{key}/{languageCode}")]
        [AllowAnonymous]
        public async Task<ApiResponseDto<PublishedStaticContentResultDto?>> GetPublishedStaticContentAsync(string key, string languageCode)
        {
            return new ApiResponseDto<PublishedStaticContentResultDto?>(await staticContentService.GetPublishedStaticContentAsync(key, languageCode));
        }

        [HttpGet("supported-languages")]
        [AllowAnonymous]
        [OutputCache(Duration = 3600)]
        public ApiResponseDto<IReadOnlyList<StaticContentSupportedLanguage>> GetSupportedLanguages()
        {
            return new ApiResponseDto<IReadOnlyList<StaticContentSupportedLanguage>>(staticContentService.GetSupportedLanguages());
        }

        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.ContentEditor}")]
        public async Task<ApiResponseDto<StaticContentDto>> CreateStaticContentAsync(CreateStaticContentDto createDto)
        {
            return new ApiResponseDto<StaticContentDto>(await staticContentService.CreateStaticContentAsync(createDto));
        }

        [HttpGet("{key}")]
        public async Task<ApiResponseDto<StaticContentDto?>> GetStaticContentAsync(string key)
        {
            return new ApiResponseDto<StaticContentDto?>(await staticContentService.GetStaticContentAsync(key));
        }

        [HttpPost("search")]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.ContentEditor}")]
        public async Task<ApiResponseDto<PagedResponseDto<StaticContentDto>>> SearchStaticContentAsync(SearchStaticContentRequestDto searchDto)
        {
            return new ApiResponseDto<PagedResponseDto<StaticContentDto>>(await staticContentService.SearchStaticContentAsync(searchDto));
        }

        [HttpPut("{key}")]
        public async Task<ApiResponseDto<StaticContentDto>> UpdateStaticContentAsync(string key, UpdateStaticContentDto updateDto)
        {
            return new ApiResponseDto<StaticContentDto>(await staticContentService.UpdateStaticContentAsync(key, updateDto));
        }

        [HttpDelete("{key}")]
        [Authorize(Roles = Constants.Roles.Administrator)]
        public async Task<ApiResponseDto<bool>> DeleteStaticContentAsync(string key)
        {
            await staticContentService.DeleteStaticContentAsync(key);
            return new ApiResponseDto<bool>(true);
        }

        [HttpGet("{key}/languages/{languageCode}")]
        public async Task<ApiResponseDto<StaticContentLanguageDto?>> GetStaticContentLanguageAsync(string key, string languageCode)
        {
            return new ApiResponseDto<StaticContentLanguageDto?>(await staticContentService.GetStaticContentLanguageAsync(key, languageCode));
        }

        [HttpGet("{key}/languages")]
        public async Task<ApiResponseDto<IReadOnlyList<StaticContentLanguageDto>>> GetStaticContentLanguagesAsync(string key)
        {
            return new ApiResponseDto<IReadOnlyList<StaticContentLanguageDto>>(await staticContentService.GetStaticContentLanguagesAsync(key));
        }

        [HttpPost("{key}/languages/{languageCode}")]
        public async Task<ApiResponseDto<StaticContentLanguageDto>> CreateStaticContentLanguageAsync(string key, string languageCode, CreateStaticContentLanguageDto createDto)
        {
            return new ApiResponseDto<StaticContentLanguageDto>(await staticContentService.CreateStaticContentLanguageAsync(key, languageCode, createDto));
        }

        [HttpPut("{key}/languages/{languageCode}")]
        public async Task<ApiResponseDto<StaticContentLanguageDto>> UpdateStaticContentLanguageAsync(string key, string languageCode, UpdateStaticContentLanguageDto updateDto)
        {
            return new ApiResponseDto<StaticContentLanguageDto>(await staticContentService.UpdateStaticContentLanguageAsync(key, languageCode, updateDto));
        }

        [HttpDelete("{key}/languages/{languageCode}")]
        [Authorize(Roles = Constants.Roles.Administrator)]
        public async Task<ApiResponseDto<bool>> DeleteStaticContentLanguageAsync(string key, string languageCode)
        {
            await staticContentService.DeleteStaticContentLanguageAsync(key, languageCode);
            return new ApiResponseDto<bool>(true);
        }
    }
}