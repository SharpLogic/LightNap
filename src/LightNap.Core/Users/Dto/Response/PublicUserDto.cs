using System.Text.Json.Serialization;

namespace LightNap.Core.Users.Dto.Response
{
    /// <summary>
    /// Represents a user DTO with public information.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(PublicUserDto), nameof(PublicUserDto))]
    [JsonDerivedType(typeof(PrivilegedUserDto), nameof(PrivilegedUserDto))]
    [JsonDerivedType(typeof(AdminUserDto), nameof(AdminUserDto))]
    public class PublicUserDto
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// The username of the user.
        /// </summary>
        public required string UserName { get; set; }

        /// <summary>
        /// The created date of the user.
        /// </summary>
        public long CreatedDate { get; set; }
    }
}