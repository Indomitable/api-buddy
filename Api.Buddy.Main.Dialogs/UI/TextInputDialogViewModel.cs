using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace Api.Buddy.Main.Dialogs.UI;

public class TextInputDialogViewModel: ReactiveObject
{
    public TextInputDialogViewModel()
    {
        AcceptCommand = ReactiveCommand.Create(() => Input);
    }

    private string input = string.Empty;
    public string Input
    {
        get => input;
        set => this.RaiseAndSetIfChanged(ref input, value);
    }

    private string title = string.Empty;
    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    public ReactiveCommand<Unit, string> AcceptCommand { get; }
}
