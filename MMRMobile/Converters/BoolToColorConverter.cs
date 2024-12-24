using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MMRMobile.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isTrue && parameter is string colors)
        {
            var colorParts = colors.Split(',');
            if (colorParts.Length == 2)
            {
                var color = isTrue ? colorParts[0] : colorParts[1];
                return SolidColorBrush.Parse(color);
            }
        }
        return new SolidColorBrush(Colors.Black);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 