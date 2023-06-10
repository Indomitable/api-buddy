using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Api.Buddy.Main.UI.Models;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;

namespace Api.Buddy.Main.UI.Controls.Request.Body;

public partial class RequestBodyEnhancedText : UserControl
{
    private FoldingManager? foldingManager = null;
    private readonly TextEditor editor; 
    
    public RequestBodyEnhancedText()
    {
        InitializeComponent();
        editor = this.GetControl<TextEditor>("TextEditor");
        editor.DocumentChanged += DocumentChanged;
        
        DetachedFromVisualTree += (sender, args) =>
        {
            editor.DocumentChanged -= DocumentChanged;
            if (foldingManager is not null)
            {
                foldingManager.Clear();
                FoldingManager.Uninstall(foldingManager);
            }
        };
        // editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Json");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void DocumentChanged(object? sender, DocumentChangedEventArgs args)
    {
        if (foldingManager is not null)
        {
            foldingManager.Clear();
            FoldingManager.Uninstall(foldingManager);
        }
        var text = args.NewDocument.Text;
        if (text is null)
        {
            return;
        }
        foldingManager = FoldingManager.Install(editor.TextArea);
        
        // if json
        var objectStarts = new Stack<int>();
        var arrayStarts = new Stack<int>();
        for (var i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '{':
                    objectStarts.Push(i);
                    break;
                case '[':
                    arrayStarts.Push(i);
                    break;
                case '}':
                {
                    var section = foldingManager.CreateFolding(objectStarts.Pop(), i + 1);
                    section.Title = "{...}";
                    break;
                }
                case ']':
                {
                    var section = foldingManager.CreateFolding(arrayStarts.Pop(), i + 1);
                    section.Title = "[...]";
                    break;
                }
            }
        }
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
