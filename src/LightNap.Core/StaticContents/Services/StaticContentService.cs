using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.StaticContents.Dto.Request;
using LightNap.Core.StaticContents.Dto.Response;
using LightNap.Core.StaticContents.Enums;
using LightNap.Core.StaticContents.Interfaces;
using LightNap.Core.StaticContents.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.StaticContents.Services
{
    /// <summary>
    /// Service implementation for managing static content and language variants.
    /// </summary>
    public class StaticContentService(ApplicationDbContext db, IUserContext userContext, ILogger<StaticContentService> logger, HybridCache cache) : IStaticContentService
    {
        /// <summary>
        /// Tests if the current user is a global content administrator.
        /// </summary>
        /// <returns>True if they are global creators.</returns>
        private bool IsContentAdministrator()
        {
            // Must be authenticated to create.
            if (!userContext.IsAuthenticated) { return false; }

            // Allow globally configured roles.
            if (StaticContentConfig.ContentAdministratorRoles.Any(userContext.IsInRole))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Throws if the user is not a global content administrator.
        /// </summary>
        private void AssertContentAdministrator()
        {
            if (!this.IsContentAdministrator())
            {
                logger.LogWarning("User '{UserId}' attempted to create static content without permission.", userContext.GetUserId());
                throw new UserFriendlyApiException("You do not have permission to create static content.");
            }
        }

        /// <summary>
        /// Tests if the current user can edit the specified static content.
        /// </summary>
        /// <param name="staticContent">The static content.</param>
        /// <returns>True if they can edit.</returns>
        private bool CanEdit(StaticContent staticContent)
        {
            // If the user can globally create content, then they can globally edit content.
            if (this.IsContentAdministrator()) { return true; }

            // All content-specific roles explicitly allowed for this content.
            var requiredRoles = staticContent.GetExplicitEditorRoles();
            if (requiredRoles is not null && requiredRoles.Any(userContext.IsInRole))
            {
                return true;
            }

            // Check for claim explicitly added to user for this content.
            if (userContext.HasClaim(Constants.Claims.ContentEditor, staticContent.Id.ToString()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Throws if the user cannot edit the specified static content.
        /// </summary>
        /// <param name="staticContent">The static content.</param>
        private void AssertCanEdit(StaticContent staticContent)
        {
            if (!this.CanEdit(staticContent))
            {
                logger.LogWarning("User '{UserId}' attempted to edit static content '{StaticContentId}' without permission.", userContext.GetUserId(), staticContent.Id);
                throw new UserFriendlyApiException("You do not have permission to edit this static content.");
            }
        }

        /// <summary>
        /// Gets the requested published static content, falling back to default language if needed.
        /// </summary>
        /// <param name="key">The key of the content.</param>
        /// <param name="languageCode">The language.</param>
        /// <returns></returns>
        private async Task<PublishedStaticContentDto?> GetPublishedStaticContentInternalAsync(string key, string languageCode)
        {
            return await cache.GetOrCreateAsync($"published-content-internal:{key}:{languageCode}", async _ =>
            {
                var content = await db.StaticContentLanguages
                    .Where(scl => scl.LanguageCode == languageCode && scl.StaticContent!.Key == key)
                    .Select(scl => scl.ToPublishedDto())
                    .FirstOrDefaultAsync();

                if (content is null && languageCode != StaticContentConfig.DefaultLanguageCode)
                {
                    return await this.GetPublishedStaticContentInternalAsync(key, StaticContentConfig.DefaultLanguageCode);
                }

                return content;
            });
        }

        /// <summary>
        /// Invalidates the cache for all published language variants of the specified static content.
        /// </summary>
        /// <param name="staticContentId">The ID of the static content.</param>
        /// <param name="key">The key of the static content.</param>
        private async Task InvalidatePublishedContentCacheAsync(int staticContentId, string key)
        {
            foreach (var lang in StaticContentConfig.SupportedLanguages)
            {
                await cache.RemoveAsync($"published-content-internal:{key}:{lang.LanguageCode}");
            }
        }

        /// <summary>
        /// Determines the visibility of static content for the current user based on the content's read access
        /// settings.
        /// </summary>
        /// <remarks>The visibility is determined based on the <see cref="StaticContent.ReadAccess"/>
        /// property of the content and the user's authentication status, claims, and roles. If the <see
        /// cref="StaticContent.ReadAccess"/> value is unknown, access is denied.</remarks>
        /// <param name="staticContent">The static content whose visibility is being evaluated.</param>
        /// <returns>A <see cref="StaticContentUserVisibility"/> value indicating the visibility of the static content for the
        /// current user. Returns <see cref="StaticContentUserVisibility.Visible"/> if the content is accessible, <see
        /// cref="StaticContentUserVisibility.RequiresAuthentication"/> if authentication is required, or <see
        /// cref="StaticContentUserVisibility.Restricted"/> if access is denied.</returns>
        private StaticContentUserVisibility GetUserVisibilityInternal(StaticContent staticContent)
        {
            if (this.CanEdit(staticContent)) { return StaticContentUserVisibility.Editor; }

            if (staticContent.ReadAccess == StaticContentReadAccess.Public) { return StaticContentUserVisibility.Reader; }

            if (!userContext.IsAuthenticated) { return StaticContentUserVisibility.RequiresAuthentication; }

            if (staticContent.ReadAccess == StaticContentReadAccess.Authenticated) { return StaticContentUserVisibility.Reader; }

            if (userContext.HasClaim(Constants.Claims.ContentReader, staticContent.Id.ToString())) { return StaticContentUserVisibility.Reader; }

            var ReaderRoles = staticContent.GetExplicitReaderRoles();
            if (ReaderRoles is not null && ReaderRoles.Any(userContext.IsInRole)) { return StaticContentUserVisibility.Reader; }

            return StaticContentUserVisibility.Restricted;
        }

        /// <inheritdoc/>
        public async Task<PublishedStaticContentResultDto?> GetPublishedStaticContentAsync(string key, string languageCode)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentException.ThrowIfNullOrEmpty(languageCode);

            var staticContent = await db.StaticContents
                .Where(sc => sc.Key == key && sc.Status == StaticContentStatus.Published)
                .FirstOrDefaultAsync();
            if (staticContent is null) { return null; }

            var visibility = this.GetUserVisibilityInternal(staticContent);
            var result = new PublishedStaticContentResultDto
            {
                Visibility = visibility,
            };

            switch (visibility)
            {
                case StaticContentUserVisibility.Reader:
                case StaticContentUserVisibility.Editor:
                    result.Content = await this.GetPublishedStaticContentInternalAsync(key, languageCode);
                    break;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<StaticContentDto?> GetStaticContentAsync(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            userContext.AssertAuthenticated();

            var staticContent = await db.StaticContents.Where(sc => sc.Key == key).FirstOrDefaultAsync();
            if (staticContent is not null)
            {
                this.AssertCanEdit(staticContent);
            }
            return staticContent?.ToDto();
        }

        /// <inheritdoc/>
        public async Task<PagedResponseDto<StaticContentDto>> SearchStaticContentAsync(SearchStaticContentRequestDto searchDto)
        {
            ArgumentNullException.ThrowIfNull(searchDto);
            Validator.ValidateObject(searchDto, new ValidationContext(searchDto), true);

            this.AssertContentAdministrator();

            var query = db.StaticContents.AsQueryable();

            if (searchDto.ReadAccess.HasValue)
            {
                query = query.Where(sc => sc.ReadAccess == searchDto.ReadAccess.Value);
            }

            if (searchDto.Status.HasValue)
            {
                query = query.Where(sc => sc.Status == searchDto.Status.Value);
            }

            if (searchDto.Type.HasValue)
            {
                query = query.Where(sc => sc.Type == searchDto.Type.Value);
            }

            if (!string.IsNullOrEmpty(searchDto.KeyContains))
            {
                query = query.Where(sc => EF.Functions.Like(sc.Key.ToUpper(), $"%{searchDto.KeyContains.ToUpper()}%"));
            }

            query = searchDto.SortBy switch
            {
                StaticContentSortBy.Key => searchDto.ReverseSort ? query.OrderByDescending(sc => sc.Key) : query.OrderBy(sc => sc.Key),
                StaticContentSortBy.CreatedDate => searchDto.ReverseSort ? query.OrderByDescending(sc => sc.CreatedDate) : query.OrderBy(sc => sc.CreatedDate),
                StaticContentSortBy.LastModifiedDate => searchDto.ReverseSort ? query.OrderByDescending(sc => sc.LastModifiedDate) : query.OrderBy(sc => sc.LastModifiedDate),
                StaticContentSortBy.ReadAccess => searchDto.ReverseSort ? query.OrderByDescending(sc => sc.ReadAccess) : query.OrderBy(sc => sc.ReadAccess),
                StaticContentSortBy.Status => searchDto.ReverseSort ? query.OrderByDescending(sc => sc.Status) : query.OrderBy(sc => sc.Status),
                StaticContentSortBy.Type => searchDto.ReverseSort ? query.OrderByDescending(sc => sc.Type) : query.OrderBy(sc => sc.Type),
                _ => throw new ArgumentException("Invalid sort field: '{sortBy}'", searchDto.SortBy.ToString()),
            };

            var totalCount = await query.CountAsync();

            if (searchDto.PageNumber > 1)
            {
                query = query.Skip((searchDto.PageNumber - 1) * searchDto.PageSize);
            }

            var staticContents = await query
                .Take(searchDto.PageSize)
                .Select(sc => sc.ToDto())
                .ToListAsync();

            return new PagedResponseDto<StaticContentDto>(staticContents, searchDto.PageNumber, searchDto.PageSize, totalCount);
        }

        /// <inheritdoc/>
        public async Task<StaticContentDto> CreateStaticContentAsync(CreateStaticContentDto createDto)
        {
            Validator.ValidateObject(createDto, new ValidationContext(createDto), true);

            this.AssertContentAdministrator();

            if (await db.StaticContents.AnyAsync(sc => sc.Key == createDto.Key))
            {
                throw new UserFriendlyApiException($"Static content with key '{createDto.Key}' already exists.");
            }

            var staticContent = createDto.ToEntity(userContext.GetUserId());
            db.StaticContents.Add(staticContent);
            await db.SaveChangesAsync();
            return staticContent.ToDto();
        }

        /// <inheritdoc/>
        public async Task<StaticContentDto> UpdateStaticContentAsync(string key, UpdateStaticContentDto updateDto)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            Validator.ValidateObject(updateDto, new ValidationContext(updateDto), true);

            userContext.AssertAuthenticated();

            var staticContent = await db.StaticContents.FirstOrDefaultAsync(sc => sc.Key == key) ?? throw new UserFriendlyApiException($"Static content with key '{key}' not found.");

            this.AssertCanEdit(staticContent);

            updateDto.UpdateEntity(staticContent, userContext.GetUserId());
            db.StaticContents.Update(staticContent);
            await db.SaveChangesAsync();

            // Invalidate cache for all language variants of this content
            await this.InvalidatePublishedContentCacheAsync(staticContent.Id, key);

            return staticContent.ToDto();
        }

        /// <inheritdoc/>
        public async Task DeleteStaticContentAsync(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            this.AssertContentAdministrator();

            var staticContent = await db.StaticContents.FirstOrDefaultAsync(sc => sc.Key == key) ?? throw new UserFriendlyApiException($"Static content with key '{key}' not found.");

            // Invalidate cache for all language variants before deletion
            await this.InvalidatePublishedContentCacheAsync(staticContent.Id, key);

            db.StaticContents.Remove(staticContent);
            await db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<StaticContentLanguageDto?> GetStaticContentLanguageAsync(string key, string languageCode)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentException.ThrowIfNullOrEmpty(languageCode);
            userContext.AssertAuthenticated();

            var staticContent = await db.StaticContents.Where(sc => sc.Key == key).FirstOrDefaultAsync() ?? throw new UserFriendlyApiException($"Static content with key '{key}' not found.");

            this.AssertCanEdit(staticContent);

            var staticContentLanguage = await db.StaticContentLanguages
                .Where(scl => scl.StaticContentId == staticContent.Id && scl.LanguageCode == languageCode)
                .FirstOrDefaultAsync();

            return staticContentLanguage?.ToDto();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<StaticContentLanguageDto>> GetStaticContentLanguagesAsync(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            userContext.AssertAuthenticated();
            var staticContent = await db.StaticContents.Where(sc => sc.Key == key).FirstOrDefaultAsync() ?? throw new UserFriendlyApiException($"Static content with key '{key}' not found.");
            this.AssertCanEdit(staticContent);
            var staticContentLanguages = await db.StaticContentLanguages
                .Where(scl => scl.StaticContentId == staticContent.Id)
                .Select(scl => scl.ToDto())
                .ToListAsync();
            return staticContentLanguages;
        }

        /// <inheritdoc/>
        public async Task<StaticContentLanguageDto> CreateStaticContentLanguageAsync(string key, string languageCode, CreateStaticContentLanguageDto createDto)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentException.ThrowIfNullOrEmpty(languageCode);
            Validator.ValidateObject(createDto, new ValidationContext(createDto), true);
            userContext.AssertAuthenticated();

            if (!StaticContentConfig.SupportedLanguagesLookup.ContainsKey(languageCode))
            {
                throw new UserFriendlyApiException($"Unsupported language code '{languageCode}'.");
            }

            if (await db.StaticContentLanguages.AnyAsync(scl => scl.StaticContent!.Key == key && scl.LanguageCode == languageCode))
            {
                throw new UserFriendlyApiException($"Static content language with key '{key}' and language '{languageCode}' already exists.");
            }

            var staticContent = await db.StaticContents.Where(sc => sc.Key == key).FirstOrDefaultAsync() ?? throw new UserFriendlyApiException($"Static content with key '{key}' not found.");
            this.AssertCanEdit(staticContent);

            var staticContentLanguage = createDto.ToEntity(staticContent.Id, languageCode);
            db.StaticContentLanguages.Add(staticContentLanguage);
            await db.SaveChangesAsync();
            return staticContentLanguage.ToDto();
        }

        /// <inheritdoc/>
        public async Task<StaticContentLanguageDto> UpdateStaticContentLanguageAsync(string key, string languageCode, UpdateStaticContentLanguageDto updateDto)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentException.ThrowIfNullOrEmpty(languageCode);
            Validator.ValidateObject(updateDto, new ValidationContext(updateDto), true);
            userContext.AssertAuthenticated();

            var staticContent = await db.StaticContents.Where(sc => sc.Key == key).FirstOrDefaultAsync() ?? throw new UserFriendlyApiException($"Static content with key '{key}' not found.");
            this.AssertCanEdit(staticContent);

            var staticContentLanguage =
                await db.StaticContentLanguages.FirstOrDefaultAsync(scl => scl.StaticContentId == staticContent.Id && scl.LanguageCode == languageCode)
                    ?? throw new UserFriendlyApiException($"Static content language with key '{key}' and language '{languageCode}' not found.");

            updateDto.UpdateEntity(staticContentLanguage);
            db.StaticContentLanguages.Update(staticContentLanguage);
            await db.SaveChangesAsync();

            // Invalidate cache for this specific language variant
            await cache.RemoveAsync($"published-content-internal:{key}:{languageCode}");

            return staticContentLanguage.ToDto();
        }

        /// <inheritdoc/>
        public async Task DeleteStaticContentLanguageAsync(string key, string languageCode)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentException.ThrowIfNullOrEmpty(languageCode);
            userContext.AssertAuthenticated();

            var staticContent = await db.StaticContents.Where(sc => sc.Key == key).FirstOrDefaultAsync() ?? throw new UserFriendlyApiException($"Static content with key '{key}' not found.");
            this.AssertCanEdit(staticContent);

            var staticContentLanguage =
                await db.StaticContentLanguages.FirstOrDefaultAsync(scl => scl.StaticContentId == staticContent.Id && scl.LanguageCode == languageCode)
                    ?? throw new UserFriendlyApiException($"Static content language with key '{key}' and language '{languageCode}' not found.");

            // Invalidate cache for this language variant
            await cache.RemoveAsync($"published-content-internal:{key}:{languageCode}");

            db.StaticContentLanguages.Remove(staticContentLanguage);
            await db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public IReadOnlyList<StaticContentSupportedLanguage> GetSupportedLanguages()
        {
            return StaticContentConfig.SupportedLanguages;
        }
    }
}