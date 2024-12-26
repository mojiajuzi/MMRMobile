using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia;
using Avalonia.Styling;

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
                var resourceKey = isTrue ? colorParts[0].Trim() : colorParts[1].Trim();
                if (Application.Current?.Resources.TryGetResource(resourceKey, null, out var resource) == true)
                {
                    return resource as IBrush ?? new SolidColorBrush(Colors.Black);
                }
            }
        }
        return new SolidColorBrush(Colors.Black);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 