using LightNap.Core.StaticContents.Dto.Response;

namespace LightNap.Core.StaticContents.Interfaces
{
    public interface IStaticContentService
    {
        Task<PublishedStaticContentDto?> GetPublishedStaticContentAsync(string key, string languageCode);
    }
}