using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HumanResourcesApp.Classes
{
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert from DateOnly to DateTime
            if (value is DateOnly dateOnly)
            {
                return new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day);
            }

            // Handle nullable DateOnly
            if (value is DateOnly?)
            {
                DateOnly? nullableDateOnly = (DateOnly?)value;
                if (nullableDateOnly.HasValue)
                {
                    var date = nullableDateOnly.Value;
                    return new DateTime(date.Year, date.Month, date.Day);
                }
            }

            return DateTime.MinValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert from DateTime to DateOnly
            if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }

            // Return null for nullable DateOnly
            if (targetType == typeof(DateOnly?))
            {
                return DateTime.MinValue;
            }

            // Return default DateOnly
            return default(DateOnly);
        }
    }
}
