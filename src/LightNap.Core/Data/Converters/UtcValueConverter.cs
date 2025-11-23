using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LightNap.Core.Data.Converters
{
    /// <summary>
    /// Converts DateTime values to UTC DateTime values and vice versa. This ensures DateTimes going into EF are marked as UTC.
    /// </summary>
    public class UtcValueConverter : ValueConverter<DateTime, DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UtcValueConverter"/> class.
        /// </summary>
        public UtcValueConverter() : base(value => value.ToUniversalTime(), value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
        {
        }
    }
}