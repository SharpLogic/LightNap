using LightNap.Core.Data;
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

        public async Task<StaticContentDto?> GetStaticContentAsync(string key, string languageCode)
        {
            var content = await db.StaticContentLanguages
                .Where(scl => scl.Language == languageCode && scl.StaticContent!.Key == key)
                .Select(scl => scl.ToDto())
                .FirstOrDefaultAsync();

            if (content is null && languageCode != StaticContentService.DefaultLanguageCode)
            {
                return await this.GetStaticContentAsync(key, DefaultLanguageCode);
            }

            return content;
        }
    }
}