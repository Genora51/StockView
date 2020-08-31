using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace StockView.UI.View.Converters
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value:P1}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (decimal.TryParse(value.ToString().TrimEnd(new char[] { '%', ' ' }), out var result))
            {
                return Math.Round(result, 1) / 100M;
            } else
            {
                return new ValidationResult(false, "Invalid percentage format.");
            }
        }
    }
}
