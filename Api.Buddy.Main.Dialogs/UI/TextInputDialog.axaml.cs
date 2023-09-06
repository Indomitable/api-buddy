using System;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Api.Buddy.Main.Dialogs.UI;

public partial class TextInputDialog : ReactiveWindow<TextInputDialogViewModel>
{
    public TextInputDialog()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            if (ViewModel is not null)
            {
                d(ViewModel.AcceptCommand.Subscribe(Close));
            }
        });
    }

    private void OnClose(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
