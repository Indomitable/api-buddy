using Api.Buddy.Main.UI.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Folding;

namespace Api.Buddy.Main.UI.Controls.Request.Body;

public partial class RequestBodyEnhancedText : UserControl
{
    private FoldingManager? foldingManager;
    private readonly TextEditor editor; 
    
    public RequestBodyEnhancedText()
    {
        InitializeComponent();
        editor = this.GetControl<TextEditor>("TextEditor");
        editor.DocumentChanged += DocumentChanged;
        
        DetachedFromVisualTree += (_, _) =>
        {
            editor.DocumentChanged -= DocumentChanged;
            if (foldingManager is not null)
            {
                foldingManager.Clear();
                FoldingManager.Uninstall(foldingManager);
            }
        };
    }
    
    public required IFoldingHandlerResolver FoldingHandlerResolver { get; init; }

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
        var body = (EnhancedTextBody)DataContext!;
        var foldingHandler = FoldingHandlerResolver.Get(body.BodyType);
        if (foldingHandler is not null)
        {
            foldingManager = FoldingManager.Install(editor.TextArea);
            foldingHandler.UpdateFoldings(foldingManager, args.NewDocument);
        }
    }
}