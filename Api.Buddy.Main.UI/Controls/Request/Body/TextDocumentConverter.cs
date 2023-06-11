using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Api.Buddy.Main.UI.Models;
using Avalonia.Data.Converters;
using AvaloniaEdit.Document;

namespace Api.Buddy.Main.UI.Controls.Request.Body;

public class TextDocumentConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EnhancedTextBody body)
        {
            var content = body.BodyType switch
            {
                TextBodyType.Json => FormatJson(body.Content),
                _ => body.Content 
            };
            return new TextDocument(content);
        }
        return null;
    }

    private string FormatJson(string json)
    {
        try
        {
            var jsonNode = JsonNode.Parse(json);
            if (jsonNode is not null)
            {
                return jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            }
        }
        catch
        {
            // ignored
        }
        return json;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}