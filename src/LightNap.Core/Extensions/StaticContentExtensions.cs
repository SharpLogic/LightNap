using LightNap.Core.Data.Entities;
using LightNap.Core.StaticContents.Dto.Request;
using LightNap.Core.StaticContents.Dto.Response;

namespace LightNap.Core.Extensions
{
    public static class StaticContentExtensions
    {
        internal static ManageStaticContentDto ToDto(this StaticContent staticContent)
        {
            return new ManageStaticContentDto()
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

        internal static StaticContent ToCreate(this CreateStaticContentDto dto)
        {
            var staticContent = new StaticContent()
            {
                CreatedDate = DateTime.UtcNow,
                Key = dto.Key,
                Type = dto.Type,
                Status = dto.Status,
            };
            return staticContent;
        }

        internal static void UpdateEntity(this UpdateStaticContentDto dto, StaticContent staticContent)
        {
            staticContent.Key = dto.Key;
            staticContent.Status = dto.Status;
            staticContent.LastModifiedDate = DateTime.UtcNow;

            if (staticContent.Status != dto.Status)
            {
                staticContent.StatusChangedDate = DateTime.UtcNow;
            }
        }

        internal static StaticContentDto ToDto(this StaticContentLanguage staticContentLanguage)
        {
            return new StaticContentDto()
            {
                Content = staticContentLanguage.Content,
                Format = staticContentLanguage.Format,
            };
        }

        internal static ManageStaticContentLanguageDto ToManageDto(this StaticContentLanguage staticContentLanguage)
        {
            return new ManageStaticContentLanguageDto()
            {
                Content = staticContentLanguage.Content,
                Format = staticContentLanguage.Format,
                CreatedByUserId = staticContentLanguage.CreatedByUserId,
                CreatedDate = staticContentLanguage.CreatedDate,
                Language = staticContentLanguage.Language,
                LastModifiedDate = staticContentLanguage.LastModifiedDate,
                LastModifiedUserId = staticContentLanguage.LastModifiedUserId,
                StaticContentId = staticContentLanguage.StaticContentId,
            };
        }

        public static StaticContentLanguage ToCreate(this CreateStaticContentLanguageDto dto, int staticContentId)
        {
            var staticContentLanguage = new StaticContentLanguage()
            {
                CreatedDate = DateTime.UtcNow,
                Content = dto.Content,
                Format = dto.Format,
                Language = dto.Language,
                StaticContentId = staticContentId
            };
            return staticContentLanguage;
        }

        public static void UpdateEntity(this UpdateStaticContentLanguageDto dto, StaticContentLanguage staticContentLanguage)
        {
            staticContentLanguage.Content = dto.Content;
            staticContentLanguage.Format = dto.Format;
            staticContentLanguage.Language = dto.Language;
            staticContentLanguage.LastModifiedDate = DateTime.UtcNow;
        }
    }
}