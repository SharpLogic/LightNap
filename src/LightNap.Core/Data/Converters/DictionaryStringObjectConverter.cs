using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace LightNap.Core.Data.Converters
{
    internal class DictionaryStringObjectConverter : ValueConverter<Dictionary<string, object>, string>
    {
        public DictionaryStringObjectConverter()
            : base(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
            )
        { }
    }
}