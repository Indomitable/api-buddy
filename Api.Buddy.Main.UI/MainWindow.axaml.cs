using Api.Buddy.Main.UI.MVVM;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Api.Buddy.Main.UI;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!));
    }
}
