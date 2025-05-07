using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HumanResourcesApp.Classes
{
    public class StringToBooleanConverter : IValueConverter
    {
        public required string TrueValue { get; set; }
        public bool Inverted { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Inverted;

            // Check if the current status equals the TrueValue (e.g., "Submitted")  
            bool isEqual = string.Equals(value.ToString(), TrueValue, StringComparison.OrdinalIgnoreCase);

            // Default result based on whether we want fields enabled (when NOT submitted) or disabled (when submitted)  
            bool result = !isEqual;

            if (parameter != null && parameter.ToString() == "Visibility")
            {
                // For visibility conversion  
                return result ? Visibility.Visible : Visibility.Collapsed;
            }

            if (parameter != null && parameter.ToString() == "Inverted")
            {
                // For inverted logic on IsReadOnly etc.  
                return !result;
            }

            // Handle standard boolean conversion  
            return Inverted ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}