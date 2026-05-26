using System.Reflection;
using LightNap.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LightNap.Core.Data.Extensions
{
    /// <summary>
    /// Extension methods for wiring <see cref="JsonValueConverter{T}"/> instances
    /// onto entity properties marked with <see cref="StoredAsJsonAttribute"/>.
    /// </summary>
    public static class ModelBuilderJsonExtensions
    {
        extension(ModelBuilder modelBuilder)
        {
            /// <summary>
            /// Scans all configured entity types for properties marked with
            /// <see cref="StoredAsJsonAttribute"/> and applies <see cref="JsonValueConverter{T}"/>
            /// to each. Call from <see cref="DbContext.OnModelCreating"/>.
            /// </summary>
            /// <returns>The same <see cref="ModelBuilder"/> for chaining.</returns>
            public ModelBuilder ApplyJsonValueConverters()
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.ClrType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (property.GetCustomAttribute<StoredAsJsonAttribute>() is null) { continue; }

                        var propertyType = property.PropertyType;
                        if (!propertyType.IsClass || propertyType == typeof(string)) { continue; }

                        var converterType = typeof(JsonValueConverter<>).MakeGenericType(propertyType);
                        // JsonValueConverter<T> has a single optional ctor parameter (JsonSerializerOptions).
                        // Pass null explicitly so Activator binds to that ctor.
                        var converter = (ValueConverter)Activator.CreateInstance(converterType, [null])!;

                        modelBuilder.Entity(entityType.ClrType)
                            .Property(property.Name)
                            .HasConversion(converter);
                    }
                }
                return modelBuilder;
            }
        }
    }
}
