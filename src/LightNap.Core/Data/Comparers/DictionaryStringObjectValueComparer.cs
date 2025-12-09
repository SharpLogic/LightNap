using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace LightNap.Core.Data.Comparers
{
    internal class DictionaryStringObjectValueComparer : ValueComparer<Dictionary<string, object>>
    {
        public DictionaryStringObjectValueComparer()
            : base(
                (d1, d2) => JsonSerializer.Serialize(d1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(d2, (JsonSerializerOptions?)null),
                d => d == null ? 0 : JsonSerializer.Serialize(d, (JsonSerializerOptions?)null).GetHashCode(),
                d => (d == null ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(d, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null))!
            )
        { }
    }
}