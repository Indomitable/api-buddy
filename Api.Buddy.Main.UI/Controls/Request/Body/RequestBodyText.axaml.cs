using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Api.Buddy.Main.UI.Controls.Request.Body;

public partial class RequestBodyText : UserControl
{
    public RequestBodyText()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}