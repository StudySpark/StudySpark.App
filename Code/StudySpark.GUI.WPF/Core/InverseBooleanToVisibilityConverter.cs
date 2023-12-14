using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StudySpark.GUI.WPF.Core {
    public class InverseBooleanToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool boolValue) {
                return !boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed; // Default to collapsed if the value is not a boolean
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Visibility visibility) {
                return visibility != Visibility.Visible;
            }

            return false; // Default to false if the value is not a Visibility enumeration
        }
    }
}