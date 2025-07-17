namespace LightNap.Core.Users.Dto.Request
{
    /// <summary>
    /// Represents a request to search user claims.
    /// </summary>
    public class SearchUserClaimsRequestDto : SearchClaimsRequestDto
    {
        /// <summary>
        /// Filter by user.
        /// </summary>
        public string? UserId { get; set; }
    }
}