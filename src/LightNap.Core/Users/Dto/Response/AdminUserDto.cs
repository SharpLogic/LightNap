namespace LightNap.Core.Users.Dto.Response
{
    /// <summary>
    /// Represents a user DTO with public information.
    /// </summary>
    public class AdminUserDto : PrivilegedUserDto
    {
        /// <summary>
        /// The last modified date of the user.
        /// </summary>
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// When the user's lockout ends, if applicable.
        /// </summary>
        public DateTime? LockoutEnd { get; set; }
    }
}