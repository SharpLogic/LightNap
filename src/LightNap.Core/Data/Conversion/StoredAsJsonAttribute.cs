namespace LightNap.Core.Data.Conversion
{
    /// <summary>
    /// Marks an entity property to be persisted as JSON via the configured
    /// <see cref="JsonValueConverter{T}"/>. The column is stored as plain text on
    /// all providers; the property type is preserved at the entity boundary.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class StoredAsJsonAttribute : Attribute { }
}
