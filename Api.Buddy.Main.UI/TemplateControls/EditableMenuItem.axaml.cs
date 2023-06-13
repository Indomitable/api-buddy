using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Api.Buddy.Main.UI.TemplateControls;

public class EditableMenuItem : TemplatedControl
{
    // private Grid? container;
    private TextBlock? iconTextBlock;
    private TextBlock? textTextBlock;
    private TextBox? textValueTextBox;
    
    public static readonly DirectProperty<EditableMenuItem, string> IconProperty =
        AvaloniaProperty.RegisterDirect<EditableMenuItem, string>(nameof(Icon), o => o.Icon, (o, v) => o.Icon = v, string.Empty);

    private string icon = string.Empty;
    public string Icon
    {
        get => icon;
        set => SetAndRaise(IconProperty, ref icon, value);
    }
    
    public static readonly DirectProperty<EditableMenuItem, string> TextProperty =
        AvaloniaProperty.RegisterDirect<EditableMenuItem, string>(nameof(Text), o => o.Text, (o, v) => o.Text = v, string.Empty);

    private string text = string.Empty;
    public string Text
    {
        get => text;
        set => SetAndRaise(TextProperty, ref text, value);
    }

    private bool readOnly = true;

    public static readonly DirectProperty<EditableMenuItem, bool> ReadOnlyProperty = AvaloniaProperty.RegisterDirect<EditableMenuItem, bool>(
        "ReadOnly", o => o.ReadOnly, (o, v) => o.ReadOnly = v);

    public bool ReadOnly
    {
        get => readOnly;
        set => SetAndRaise(ReadOnlyProperty, ref readOnly, value);
    }

    private string textValue = string.Empty;

    public static readonly DirectProperty<EditableMenuItem, string> TextValueProperty = AvaloniaProperty.RegisterDirect<EditableMenuItem, string>(
        "TextValue", o => o.TextValue, (o, v) => o.TextValue = v);

    public string TextValue
    {
        get => textValue;
        set => SetAndRaise(TextValueProperty, ref textValue, value);
    }

    private ICommand? editCommand;

    public static readonly DirectProperty<EditableMenuItem, ICommand?> EditCommandProperty = AvaloniaProperty.RegisterDirect<EditableMenuItem, ICommand?>(
        "OnEditCommand", o => o.EditCommand, (o, v) => o.EditCommand = v);

    public ICommand? EditCommand
    {
        get => editCommand;
        set => SetAndRaise(EditCommandProperty, ref editCommand, value);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
      
        iconTextBlock = e.NameScope.Find<TextBlock>("PART_EditableMenuItem_Icon")!;
        textTextBlock = e.NameScope.Find<TextBlock>("PART_EditableMenuItem_Text")!;
        if (textValueTextBox is not null)
        {
            textValueTextBox.LostFocus -= OnTextValueBlur;
            textValueTextBox.KeyDown -= OnTextValueKeyDown;
        }
        textValueTextBox = e.NameScope.Find<TextBox>("PART_EditableMenuItem_TextValue")!;
        if (textValueTextBox is not null)
        {
            textValueTextBox.LostFocus += OnTextValueBlur;
            textValueTextBox.KeyDown += OnTextValueKeyDown;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty && textTextBlock is not null)
        {
            textTextBlock.Text = Text;
        }
        if (change.Property == IconProperty && iconTextBlock is not null)
        {
            iconTextBlock.Text = Icon;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Handled = true;
        SwitchToEditMode();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Return)
        {
            e.Handled = true;
            SwitchToEditMode();
        }
    }

    // private void OnItemKeyDown(object? sender, KeyEventArgs args)
    // {
    //     if (args.Key == Key.Return)
    //     {
    //         args.Handled = true;
    //         SwitchToEditMode();
    //     }
    // }

    private void OnTextValueBlur(object? sender, RoutedEventArgs args)
    {
        SwitchToTextMode();
    }
    
    private void OnTextValueKeyDown(object? sender, KeyEventArgs args)
    {
        if (args.Key == Key.Return)
        {
            EditCommand?.Execute(TextValue);
            SwitchToTextMode();
            args.Handled = true;
        }
    }

    private void SwitchToEditMode()
    {
        TextValue = string.Empty;
        ReadOnly = false;
        textValueTextBox?.Focus();
    }
    
    private void SwitchToTextMode()
    {
        ReadOnly = true;
        TextValue = string.Empty;
    }
}