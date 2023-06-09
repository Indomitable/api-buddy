using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Api.Buddy.Main.UI.Converters;

public class EnumToStringConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Enum @enum)
        {
            return Enum.GetName(@enum.GetType(), @enum);
        }

        if (value is string str && Enum.TryParse(targetType, str, true, out var res))
        {
            return res;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
