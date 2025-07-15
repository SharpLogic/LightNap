using LightNap.Core.Users.Dto.Request;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for converting SearchClaimsRequests.
    /// </summary>
    public static class SearchClaimsRequestExtensions
    {
        public static SearchUserClaimsRequestDto ToUserClaimsSearchRequestDto(this SearchClaimsRequestDto dto, string userId)
        {
            return new SearchUserClaimsRequestDto()
            {
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                Type = dto.Type,
                TypeContains = dto.TypeContains,
                Value = dto.Value,
                ValueContains = dto.ValueContains,
                UserId = userId
            };
        }
    }
}