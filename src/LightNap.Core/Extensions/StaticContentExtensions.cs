using LightNap.Core.Configuration;
using LightNap.Core.Data.Entities;
using LightNap.Core.StaticContents.Dto.Request;
using LightNap.Core.StaticContents.Dto.Response;

namespace LightNap.Core.Extensions
{
    public static class StaticContentExtensions
    {
        internal static StaticContentDto ToDto(this StaticContent staticContent)
        {
            return new StaticContentDto()
            {
                CreatedByUserId = staticContent.CreatedByUserId,
                CreatedDate = staticContent.CreatedDate,
                Id = staticContent.Id,
                Key = staticContent.Key,
                LastModifiedByUserId = staticContent.LastModifiedByUserId,
                LastModifiedDate = staticContent.LastModifiedDate,
                ReadAccess = staticContent.ReadAccess,
                Status = staticContent.Status,
                StatusChangedByUserId = staticContent.StatusChangedByUserId,
                StatusChangedDate = staticContent.StatusChangedDate,
                Type = staticContent.Type,
            };
        }

        internal static StaticContent ToEntity(this CreateStaticContentDto dto, string userId)
        {
            if (dto.ViewerRoles != null && dto.ReadAccess != StaticContentReadAccess.Explicit)
            {
                throw new InvalidOperationException("ReadAccess must be set to Explicit when ViewerRoles is not null.");
            }

            var staticContent = new StaticContent()
            {
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = userId != Constants.Identity.SystemUserId ? userId : null,
                Key = dto.Key,
                Type = dto.Type,
                Status = dto.Status,
                ReadAccess = dto.ReadAccess,
                ReaderRoles = dto.ViewerRoles,
                EditorRoles = dto.EditorRoles,
            };
            return staticContent;
        }

        internal static void UpdateEntity(this UpdateStaticContentDto dto, StaticContent staticContent, string userId)
        {
            if (dto.ViewerRoles != null && dto.ReadAccess != StaticContentReadAccess.Explicit)
            {
                throw new InvalidOperationException("ReadAccess must be set to Explicit when ViewerRoles is not null.");
            }

            staticContent.Key = dto.Key;
            staticContent.LastModifiedDate = DateTime.UtcNow;
            staticContent.LastModifiedByUserId = userId;
            staticContent.Type = dto.Type;

            staticContent.ReadAccess = dto.ReadAccess;
            staticContent.ReaderRoles = dto.ViewerRoles;
            staticContent.EditorRoles = dto.EditorRoles;

            if (staticContent.Status != dto.Status)
            {
                staticContent.StatusChangedDate = DateTime.UtcNow;
                staticContent.StatusChangedByUserId = userId;
            }
            staticContent.Status = dto.Status;
        }

        internal static string[]? GetExplicitEditorRoles(this StaticContent staticContent)
        {
            return staticContent.EditorRoles?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        internal static string[]? GetExplicitReaderRoles(this StaticContent staticContent)
        {
            return staticContent.ReaderRoles?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        internal static PublishedStaticContentDto ToPublishedDto(this StaticContentLanguage staticContentLanguage)
        {
            return new PublishedStaticContentDto()
            {
                Content = staticContentLanguage.Content,
                Format = staticContentLanguage.Format,
            };
        }

        internal static StaticContentLanguageDto ToDto(this StaticContentLanguage staticContentLanguage)
        {
            return new StaticContentLanguageDto()
            {
                Content = staticContentLanguage.Content,
                Format = staticContentLanguage.Format,
                CreatedByUserId = staticContentLanguage.CreatedByUserId,
                CreatedDate = staticContentLanguage.CreatedDate,
                LanguageCode = staticContentLanguage.LanguageCode,
                LastModifiedDate = staticContentLanguage.LastModifiedDate,
                LastModifiedUserId = staticContentLanguage.LastModifiedUserId,
                StaticContentId = staticContentLanguage.StaticContentId,
            };
        }

        public static StaticContentLanguage ToEntity(this CreateStaticContentLanguageDto dto, int staticContentId, string language)
        {
            var staticContentLanguage = new StaticContentLanguage()
            {
                CreatedDate = DateTime.UtcNow,
                Content = dto.Content,
                Format = dto.Format,
                LanguageCode = language,
                StaticContentId = staticContentId
            };
            return staticContentLanguage;
        }

        public static void UpdateEntity(this UpdateStaticContentLanguageDto dto, StaticContentLanguage staticContentLanguage)
        {
            staticContentLanguage.Content = dto.Content;
            staticContentLanguage.Format = dto.Format;
            staticContentLanguage.LastModifiedDate = DateTime.UtcNow;
        }
    }
}