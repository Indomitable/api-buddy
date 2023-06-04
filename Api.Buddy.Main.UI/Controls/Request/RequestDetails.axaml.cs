using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Api.Buddy.Main.UI.Controls.Request;

public partial class RequestDetails : UserControl
{
    public RequestDetails()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static readonly StyledProperty<ICommand?> SendCommandProperty =
        AvaloniaProperty.Register<RequestDetails, ICommand?>(nameof(SendCommand), enableDataValidation: true);

    public ICommand? SendCommand
    {
        get { return GetValue(SendCommandProperty); }
        set { SetValue(SendCommandProperty, value); }
    }
}
