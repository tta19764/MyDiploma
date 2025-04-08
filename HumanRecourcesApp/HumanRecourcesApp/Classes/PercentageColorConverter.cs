using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HumanResourcesApp.Classes
{
    public class PercentageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double percentage)
            {
                if (percentage > 0)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ecc71")); // Green for positive
                else if (percentage < 0)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c")); // Red for negative
                else
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#777777")); // Grey for zero
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#777777"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}