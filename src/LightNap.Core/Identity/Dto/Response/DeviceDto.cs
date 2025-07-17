namespace LightNap.Core.Identity.Dto.Response
{
    /// <summary>
    /// Data transfer object representing a device.
    /// </summary>
    public class DeviceDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the device.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last time the device was seen.
        /// </summary>
        public DateTime LastSeen { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the device.
        /// </summary>
        public required string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets additional details about the device.
        /// </summary>
        public required string Details { get; set; }
    }
}