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
                Status = staticContent.Status,
                StatusChangedByUserId = staticContent.StatusChangedByUserId,
                StatusChangedDate = staticContent.StatusChangedDate,
                Type = staticContent.Type,
            };
        }

        internal static StaticContent ToEntity(this CreateStaticContentDto dto)
        {
            var staticContent = new StaticContent()
            {
                CreatedDate = DateTime.UtcNow,
                Key = dto.Key,
                Type = dto.Type,
                Status = dto.Status,
                // If RequiredRoles is set, we assume authentication is required.
                RequiresAuthentication = dto.RequiresAuthentication || (dto.RequiredRoles != null),
                RequiredRoles = dto.RequiredRoles
            };
            return staticContent;
        }

        internal static void UpdateEntity(this UpdateStaticContentDto dto, StaticContent staticContent)
        {
            staticContent.Key = dto.Key;
            staticContent.LastModifiedDate = DateTime.UtcNow;

            // If RequiredRoles is set, we assume authentication is required.
            staticContent.RequiresAuthentication = dto.RequiresAuthentication || (dto.RequiredRoles != null);
            staticContent.RequiredRoles = dto.RequiredRoles;

            if (staticContent.Status != dto.Status)
            {
                staticContent.StatusChangedDate = DateTime.UtcNow;
            }
            staticContent.Status = dto.Status;
        }

        internal static string[]? GetRequiredRoles(this StaticContent staticContent)
        {
            return staticContent.RequiredRoles?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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