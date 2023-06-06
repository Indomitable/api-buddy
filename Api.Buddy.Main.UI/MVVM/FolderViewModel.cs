using System.Collections.Generic;
using Api.Buddy.Main.Logic.Models.Project;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public sealed record PathSegment(string Name, bool IsLast);

internal sealed class FolderViewModel: ReactiveObject
{
    private readonly FolderNode folderNode;

    public FolderViewModel(FolderNode folderNode)
    {
        this.folderNode = folderNode;
        PathSegments = BuildPath(folderNode);
    }

    public Stack<PathSegment> PathSegments { get; }

    public string Name
    {
        get => folderNode.Name;
        set => folderNode.Name = value;
    }

    private static Stack<PathSegment> BuildPath(FolderNode folderNode)
    {
        ProjectNode? node = folderNode;
        var path = new Stack<PathSegment>();
        var isLast = true;
        do
        {
            path.Push(new PathSegment(node.Name, isLast));
            isLast = false;
            node = node.Parent;
        } while (node != null);
        return path;
    }
}
