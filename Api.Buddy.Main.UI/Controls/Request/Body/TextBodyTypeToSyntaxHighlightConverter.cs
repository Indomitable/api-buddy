using System;
using System.Globalization;
using Api.Buddy.Main.UI.Models;
using Avalonia.Data.Converters;
using AvaloniaEdit.Highlighting;

namespace Api.Buddy.Main.UI.Controls.Request.Body;

public class TextBodyTypeToSyntaxHighlightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TextBodyType textBodyType)
        {
            return HighlightingManager.Instance.GetDefinition(Enum.GetName(textBodyType));
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}