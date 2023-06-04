using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Api.Buddy.Main.UI.Controls.Request;

public partial class RequestView : UserControl
{
    public RequestView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
