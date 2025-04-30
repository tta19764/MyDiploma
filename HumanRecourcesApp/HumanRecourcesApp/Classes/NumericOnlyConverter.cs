using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace HumanResourcesApp.Classes
{
    public class NumericOnlyConverter : IValueConverter
    {
        private static readonly Regex _numericRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string textValue)
            {
                if (string.IsNullOrEmpty(textValue))
                    return true; // Empty string is allowed

                return _numericRegex.IsMatch(textValue);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}