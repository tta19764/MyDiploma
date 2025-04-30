using System;
using System.Globalization;
using System.Windows.Data;

namespace HumanResourcesApp.Classes
{
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueValue { get; set; } = "Yes";
        public string FalseValue { get; set; } = "No";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueValue : FalseValue;
            }
            return FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == TrueValue;
        }
    }
}