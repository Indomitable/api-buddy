namespace Api.Buddy.Main.Logic.Models.Project;

public sealed class FolderNode: ProjectNode
{
    public FolderNode()
    {
        NodeType = NodeType.Folder;
    }
}
