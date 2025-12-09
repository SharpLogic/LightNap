namespace LightNap.Core.Users.Dto.Response
{
    /// <summary>
    /// Represents a data transfer object for a user with elevated privileges, extending the public user information
    /// with additional properties.
    /// </summary>
    /// <remarks>Use this type when transferring privileged user data between application layers or services.
    /// Inherits all properties from <see cref="PublicUserDto"/> and adds privileged-specific information.</remarks>
    public class PrivilegedUserDto : PublicUserDto
    {
        /// <summary>
        /// The email of the user.
        /// </summary>
        public required string Email { get; set; }
    }
}