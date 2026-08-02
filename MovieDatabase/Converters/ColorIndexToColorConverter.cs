using System.Globalization;
using Microsoft.Maui.Controls;

namespace MovieDatabase.Converters
{
    public class ColorIndexToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            int index = value is int i ? i : 0;
            return TagPalette.GetColor(index);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
