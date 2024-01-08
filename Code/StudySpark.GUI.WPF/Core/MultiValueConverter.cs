using System;
using System.Globalization;
using System.Windows.Data;

namespace StudySpark.GUI.WPF.Core {
    public class MultiValueConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            // Combine multiple values into a single parameter
            return values.Clone(); // Clone to avoid reference issues
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
