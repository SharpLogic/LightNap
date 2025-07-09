namespace LightNap.Core.Users.Dto.Response
{
    /// <summary>
    /// Represents a complete device/refresh token DTO intended for use by Administrator accounts.
    /// </summary>
    public class AdminDeviceDto
    {
        /// <summary>
        /// Gets or sets the ID of the admin device.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of when the admin device was last seen.
        /// </summary>
        public long LastSeen { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the admin device.
        /// </summary>
        public required string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the details of the admin device.
        /// </summary>
        public required string Details { get; set; }

        /// <summary>
        /// Gets or sets the expiration timestamp of the admin device.
        /// </summary>
        public long Expires { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user associated with the admin device.
        /// </summary>
        public required string UserId { get; set; }
    }
}