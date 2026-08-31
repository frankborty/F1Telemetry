using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace F1Telemetry.App.Converters
{
    public sealed class BoolToBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value is true
                ? Brushes.LimeGreen
                : Brushes.Gray;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
