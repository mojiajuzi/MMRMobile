using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MMRMobile.Converters;

public class BoolToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isTrue && parameter is string texts)
        {
            var textParts = texts.Split(',');
            if (textParts.Length == 2)
            {
                return isTrue ? textParts[0] : textParts[1];
            }
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 