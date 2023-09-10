using System;
using DynamicData.Binding;

namespace Api.Buddy.Main.Logic.Models.Project;

public sealed class FolderNode: ProjectNode
{
    public FolderNode(Guid id, string name, Project project, ProjectNode? parent = null)
        : base(id, name, project, parent)
    {
    }

    public override NodeType NodeType => NodeType.Folder;
    public ObservableCollectionExtended<ProjectNode> Children { get; } = new();
    
    /// <summary>
    /// Get insert index so the elements to be sorted by  name
    /// </summary>
    public int GetIndex<T>(T child)
        where T: ProjectNode
    {
        int index = 0;
        using var enumerator = Children.GetEnumerator();
        while (enumerator.MoveNext() 
               && enumerator.Current is T n
               && string.CompareOrdinal(n.Name, child.Name) < 0)
        {
            index++;
        }
        return index;
    }
}
