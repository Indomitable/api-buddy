using Avalonia.Collections;
using ReactiveUI;

namespace Api.Buddy.Main.Logic.Models.Project;

public abstract class ProjectNode: ReactiveObject
{
    protected ProjectNode()
    {
        Children = new AvaloniaList<ProjectNode>();
    }

    public ProjectNode? Parent { get; set; }

    public AvaloniaList<ProjectNode> Children { get; }

    public NodeType NodeType { get; protected set; }

    private string name = string.Empty;
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }
}
