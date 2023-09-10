using System;
using DynamicData.Binding;
using ReactiveUI;

namespace Api.Buddy.Main.Logic.Models.Project;

public sealed class Project: ReactiveObject, INode
{
    public Project(Guid id, string name)
    {
        Id = id;
        this.name = name;
    }
    
    public Guid Id { get; }
    
    private string name;
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public ObservableCollectionExtended<ProjectNode> Nodes { get; } = new();
}
