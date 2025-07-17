namespace LightNap.Core.Users.Dto.Response
{
    public class PrivilegedUserDto : PublicUserDto
    {
        /// <summary>
        /// The username of the user.
        /// </summary>
        public required string Email { get; set; }
    }
}