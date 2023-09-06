using System.Threading.Tasks;
using Api.Buddy.Main.Dialogs.UI;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Api.Buddy.Main.Dialogs.Services;

public interface ITextInputDialogService
{
    Task<string> GetInput(string title, string? input = null);
}

internal sealed class TextInputDialogService : ITextInputDialogService
{
    public async Task<string> GetInput(string title, string? input = null)
    {
        var viewModel = new TextInputDialogViewModel
        {
            Title = title,
        };
        if (!string.IsNullOrEmpty(input))
        {
            viewModel.Input = input;
        }
        var dialog = new TextInputDialog
        {
            DataContext = viewModel
        };
        var mainWindow = ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
        return await dialog.ShowDialog<string>(mainWindow);
    }
}
