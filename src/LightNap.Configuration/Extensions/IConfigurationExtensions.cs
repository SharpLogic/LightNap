using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Configuration.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IConfiguration"/>.
    /// </summary>
    public static class IConfigurationExtensions
    {
        extension(IConfiguration configuration)
        {
            /// <summary>
            /// Gets the value of a required connection string from the configuration.
            /// </summary>
            /// <param name="key">The key of the connection string.</param>
            /// <returns>The value of the connection string.</returns>
            /// <exception cref="ArgumentException">Thrown when the required connection string is missing.</exception>
            public string GetRequiredConnectionString(string key)
            {
                return configuration.GetConnectionString(key) ?? throw new ArgumentException($"Required connection string '{key}' is missing");
            }

            /// <summary>
            /// Gets the value of a required setting from the configuration.
            /// </summary>
            /// <param name="key">The key of the setting.</param>
            /// <returns>The value of the setting.</returns>
            /// <exception cref="ArgumentException">Thrown when the required setting is missing.</exception>
            public string GetRequiredSetting(string key)
            {
                return configuration[key] ?? throw new ArgumentException($"Required setting '{key}' is missing");
            }

            /// <summary>
            /// Gets a required configuration section and binds it to the specified type, with attribute-based validation.
            /// </summary>
            /// <typeparam name="T">The type to bind the section to.</typeparam>
            /// <param name="key">The key of the section.</param>
            /// <returns>The bound and validated object.</returns>
            /// <exception cref="ArgumentException">Thrown when the required section is missing.</exception>
            /// <exception cref="ValidationException">Thrown when the bound object fails attribute validation.</exception>
            public T GetRequiredSection<T>(string key) where T : class
            {
                return configuration.GetOptionalSection<T>(key) ?? throw new ArgumentException($"Required section '{key}' is missing or invalid");
            }

            /// <summary>
            /// Gets an optional configuration section and binds it to the specified type with attribute-based
            /// validation, returning null if the section is missing or cannot be bound.
            /// </summary>
            /// <typeparam name="T">The reference type to bind the section to.</typeparam>
            /// <param name="key">The key of the section.</param>
            /// <returns>The bound and validated object, or null if the section is absent.</returns>
            /// <exception cref="ValidationException">Thrown when the bound object fails attribute validation.</exception>
            public T? GetOptionalSection<T>(string key) where T : class
            {
                var section = configuration.GetSection(key);
                var value = section.Get<T>();
                if (value is null) { return null; }

                Validator.ValidateObject(value, new ValidationContext(value), validateAllProperties: true);
                return value;
            }
        }
    }
}
