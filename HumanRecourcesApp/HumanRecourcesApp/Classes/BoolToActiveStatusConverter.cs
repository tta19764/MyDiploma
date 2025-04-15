using System;
using System.Globalization;
using System.Windows.Data;

namespace HumanResourcesApp.Classes
{
    /// <summary>
    /// Converts a boolean value to "Active" or "Inactive" status text
    /// </summary>
    public class BoolToActiveStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? "Active" : "Inactive";
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.Equals("Active", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}