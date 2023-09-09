using System;
using Avalonia.Input;
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

    protected override void OnOpened(EventArgs e)
    {
        InputBox.Focus();
        InputBox.SelectAll();
        base.OnOpened(e);
    }

    private void OnClose(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void InputBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            Close(InputBox.Text);
        }
    }
}
