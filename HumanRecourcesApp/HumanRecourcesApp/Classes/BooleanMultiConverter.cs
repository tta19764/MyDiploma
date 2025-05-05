using System;
using System.Globalization;
using System.Windows.Data;

namespace HumanResourcesApp.Classes
{
    public class BooleanMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // If any value is false, the result is false
            foreach (object value in values)
            {
                if (value is bool boolValue && !boolValue)
                {
                    return false;
                }
            }

            // All values are true
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}