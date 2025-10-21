using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.StaticContents.Dto.Response;
using LightNap.Core.StaticContents.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LightNap.Core.StaticContents.Services
{
    public class StaticContentService(ApplicationDbContext db, IUserContext userContext) : IStaticContentService
    {
        const string DefaultLanguageCode = "en";

        public async Task<PublishedStaticContentDto?> GetPublishedStaticContentAsync(string key, string languageCode)
        {
            var staticContent = await db.StaticContents
                .Where(sc => sc.Key == key && sc.Status == StaticContentStatus.Published)
                .FirstOrDefaultAsync();
            if (staticContent is null) { return null; }

            // TODO: Auth

            return await this.GetPublishedStaticContentInternalAsync(key, languageCode);
        }

        private async Task<PublishedStaticContentDto?> GetPublishedStaticContentInternalAsync(string key, string languageCode)
        {
            var content = await db.StaticContentLanguages
                .Where(scl => scl.Language == languageCode && scl.StaticContent!.Key == key)
                .Select(scl => scl.ToPublishedDto())
                .FirstOrDefaultAsync();

            if (content is null && languageCode != StaticContentService.DefaultLanguageCode)
            {
                return await this.GetPublishedStaticContentAsync(key, DefaultLanguageCode);
            }

            return content;
        }
    }
}