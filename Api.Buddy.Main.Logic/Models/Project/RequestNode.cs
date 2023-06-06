using Api.Buddy.Main.Logic.Models.Request;

namespace Api.Buddy.Main.Logic.Models.Project;

public class RequestNode: ProjectNode
{
    public RequestNode()
    {
        NodeType = NodeType.Request;
        Request = RequestInit.Empty;
    }

    public RequestInit Request { get; set; }
}
