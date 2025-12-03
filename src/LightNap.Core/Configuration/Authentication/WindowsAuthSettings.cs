using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Configuration.Authentication
{    
    /// <summary>
    /// Settings for Windows authentication.
    /// </summary>
    public record WindowsAuthSettings
    {
        /// <summary>
        /// Whether Windows authentication is enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
