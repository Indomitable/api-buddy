using Api.Buddy.Main.UI.MVVM;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Buddy.Main.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Container.Provider.GetRequiredService<IMainViewModel>(),
                Width = 1600,
                Height = 900,
                // Topmost = true
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
