using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Api.Buddy.Main.UI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;

namespace Api.Buddy.Main.UI.Controls.Request.Body;

public partial class RequestBodyEnhancedText : UserControl
{
    public RequestBodyEnhancedText()
    {
        InitializeComponent();
        // var editor = this.GetControl<TextEditor>("TextEditor");
        // editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Json");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

public class TextDocumentConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            try
            {
                var jsonNode = JsonNode.Parse(str);
                if (jsonNode is null)
                {
                    return new TextDocument(str);
                }
                var jsonString = jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
                return new TextDocument(jsonString);
            }
            catch
            {
                return new TextDocument(str);
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

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
