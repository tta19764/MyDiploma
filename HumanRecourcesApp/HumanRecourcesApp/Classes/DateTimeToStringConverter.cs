using System;
using System.Globalization;
using System.Windows.Data;

namespace HumanResourcesApp.Classes
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Check if date is default/empty
                if (dateTime == DateTime.MinValue)
                    return "Unknown";

                // Format the date - you can adjust the format as needed
                return dateTime.ToString("MM/dd/yyyy HH:mm");
            }
            else if (value is DateTime?)
            {
                DateTime? nullableDate = (DateTime?)value;
                if (!nullableDate.HasValue || nullableDate.Value == DateTime.MinValue)
                    return "Unknown";

                return nullableDate.Value.ToString("MM/dd/yyyy HH:mm");
            }

            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ConvertBack is not needed for this scenario, but required by the interface
            throw new NotImplementedException();
        }
    }
}