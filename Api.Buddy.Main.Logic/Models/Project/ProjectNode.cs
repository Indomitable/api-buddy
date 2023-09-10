using System;
using ReactiveUI;

namespace Api.Buddy.Main.Logic.Models.Project;

public abstract class ProjectNode: ReactiveObject, INode
{
    private string name;

    protected ProjectNode(Guid id, string name, Project project, ProjectNode? parent = null)
    {
        Id = id;
        this.name = name;
        Project = project;
        Parent = parent;
    }
    
    public Guid Id { get; }
    public ProjectNode? Parent { get; }
    public abstract NodeType NodeType { get; }
    
    public Project Project { get; }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }
}
