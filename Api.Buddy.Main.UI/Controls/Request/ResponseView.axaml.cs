using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Api.Buddy.Main.UI.Controls.Request;

public partial class ResponseView : UserControl
{
    public ResponseView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}