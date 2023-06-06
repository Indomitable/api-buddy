using ReactiveUI;

namespace Api.Buddy.Main.Logic.Models.Request;

public class Header : ReactiveObject
{
    public required int Index { get; init; }

    private string name = string.Empty;
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    private string value = string.Empty;
    public string Value
    {
        get => value;
        set => this.RaiseAndSetIfChanged(ref this.value, value);
    }

    private bool selected = true;
    public bool Selected
    {
        get => selected;
        set => this.RaiseAndSetIfChanged(ref selected, value);
    }
}

