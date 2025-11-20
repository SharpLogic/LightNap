using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations; // Add this using directive

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IConfiguration"/>.
    /// </summary>
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Gets the value of a required setting from the configuration.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The value of the setting.</returns>
        /// <exception cref="ArgumentException">Thrown when the required setting is missing.</exception>
        public static string GetRequiredSetting(this IConfiguration configuration, string key)
        {
            return configuration[key] ?? throw new ArgumentException($"Required setting '{key}' is missing");
        }

        /// <summary>
        /// Gets a required configuration section and binds it to the specified type, with attribute-based validation.
        /// </summary>
        /// <typeparam name="T">The type to bind the section to.</typeparam>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="key">The key of the section.</param>
        /// <returns>The bound and validated object.</returns>
        /// <exception cref="ArgumentException">Thrown when the required section is missing.</exception>
        /// <exception cref="ValidationException">Thrown when the bound object fails attribute validation.</exception>
        public static T GetRequiredSection<T>(this IConfiguration configuration, string key) where T : class
        {
            var section = configuration.GetSection(key);
            var value = section.Get<T>() ?? throw new ArgumentException($"Required section '{key}' is missing or invalid");
            // Validate based on attributes
            Validator.ValidateObject(value, new ValidationContext(value), validateAllProperties: true);
            return value;
        }
    }
}