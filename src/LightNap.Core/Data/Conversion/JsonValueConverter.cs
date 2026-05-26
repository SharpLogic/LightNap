using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LightNap.Core.Data.Conversion
{
    /// <summary>
    /// EF Core value converter that serializes a typed value to JSON for storage and
    /// deserializes it on read. Storage type is <see cref="string"/> (nvarchar(max) on
    /// SQL Server, TEXT on SQLite, in-memory string on the InMemory provider).
    /// </summary>
    /// <typeparam name="T">The CLR type of the property being persisted.</typeparam>
    public sealed class JsonValueConverter<T> : ValueConverter<T, string>
        where T : class
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonValueConverter{T}"/> class.
        /// </summary>
        /// <param name="options">
        /// Optional <see cref="JsonSerializerOptions"/>. When null, an instance with camelCase
        /// naming and null-skipping is used.
        /// </param>
        public JsonValueConverter(JsonSerializerOptions? options = null)
            : base(
                v => JsonSerializer.Serialize(v, options ?? DefaultOptions),
                v => JsonSerializer.Deserialize<T>(v, options ?? DefaultOptions)!)
        { }
    }
}
