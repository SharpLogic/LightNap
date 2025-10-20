using LightNap.Core.StaticContents.Dto.Response;

namespace LightNap.Core.StaticContents.Interfaces
{
    public interface IStaticContentService
    {
        Task<StaticContentDto?> GetStaticContentAsync(string key, string languageCode);
    }
}