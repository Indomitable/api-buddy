using System;
using Api.Buddy.Main.Logic.Models.Request;
using DynamicData.Binding;
using ReactiveUI;

namespace Api.Buddy.Main.Logic.Models.Project;

public class RequestNode: ProjectNode
{
    private HttpMethod method = HttpMethod.GET;
    private string url = string.Empty;
    private ObservableCollectionExtended<QueryParam> queryParams = new();
    private ObservableCollectionExtended<Header> headers = new();
    
    public RequestNode(Guid id, string name, Project project, ProjectNode? parent = null)
        : base(id, name, project, parent)
    {
    }

    public HttpMethod Method
    {
        get => method;
        set => this.RaiseAndSetIfChanged(ref method, value);
    }

    public string Url
    {
        get => url;
        set => this.RaiseAndSetIfChanged(ref url, value);
    }

    public override NodeType NodeType => NodeType.Request;

    public ObservableCollectionExtended<QueryParam> QueryParams
    {
        get => queryParams;
        set => this.RaiseAndSetIfChanged(ref queryParams, value);
    }

    public ObservableCollectionExtended<Header> Headers
    {
        get => headers;
        set => this.RaiseAndSetIfChanged(ref headers, value);
    }
}
