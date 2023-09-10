using ReactiveUI;

namespace Api.Buddy.Main.Logic.Models.Request;

public class KeyValueParam : ReactiveObject
{
    public KeyValueParam(int index, string name, string value, bool selected)
    {
        Index = index;
        this.name = name;
        this.value = value;
        this.selected = selected;
    }
    
    public int Index { get; }

    private string name;
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    private string value;
    public string Value
    {
        get => value;
        set => this.RaiseAndSetIfChanged(ref this.value, value);
    }

    private bool selected;
    public bool Selected
    {
        get => selected;
        set => this.RaiseAndSetIfChanged(ref selected, value);
    }
}

