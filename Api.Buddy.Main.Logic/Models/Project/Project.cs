using Avalonia.Collections;
using ReactiveUI;

namespace Api.Buddy.Main.Logic.Models.Project;

public sealed class Project: ReactiveObject
{
    public Project()
    {
        Nodes = new AvaloniaList<ProjectNode>();
    }

    private string name = string.Empty;
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public AvaloniaList<ProjectNode> Nodes { get; }
}