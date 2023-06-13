using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Api.Buddy.Main.UI.TemplateControls;

public class ReadonlyMenuItem : TemplatedControl
{
    private TextBlock? iconTextBlock;
    private TextBlock? textTextBlock;
    
    public static readonly DirectProperty<ReadonlyMenuItem, string> IconProperty =
        AvaloniaProperty.RegisterDirect<ReadonlyMenuItem, string>(nameof(Icon), o => o.Icon, (o, v) => o.Icon = v, string.Empty);

    private string icon = string.Empty;
    public string Icon
    {
        get => icon;
        set => SetAndRaise(IconProperty, ref icon, value);
    }
    
    public static readonly DirectProperty<ReadonlyMenuItem, string> TextProperty =
        AvaloniaProperty.RegisterDirect<ReadonlyMenuItem, string>(nameof(Text), o => o.Text, (o, v) => o.Text = v, string.Empty);

    private string text = string.Empty;
    public string Text
    {
        get => text;
        set => SetAndRaise(TextProperty, ref text, value);
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
        iconTextBlock = e.NameScope.Find<TextBlock>("PART_ReadonlyMenuItem_Icon")!;
        textTextBlock = e.NameScope.Find<TextBlock>("PART_ReadonlyMenuItem_Text")!;
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
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Return)
        {
            e.Handled = true;
        }
    }
}