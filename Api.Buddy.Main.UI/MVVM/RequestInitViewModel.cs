using System;
using System.Collections.Generic;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Models.Request;
using Api.Buddy.Main.Logic.Storage;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IRequestInitViewModel
{
    RequestNode Request { get; }
    IReadOnlyList<HttpMethod> Methods { get; }
}

public class RequestInitViewModel : ReactiveObject, IRequestInitViewModel
{
    public RequestInitViewModel(IStateManager stateManager, RequestNode request)
    {
        this.request = request;
        this.request.WhenAnyPropertyChanged(nameof(RequestNode.Method), nameof(RequestNode.Url))
            .Subscribe(_ => stateManager.Persist());
        this.request.Headers.ToObservableChangeSet<ObservableCollectionExtended<Header>, Header>()
            .AutoRefresh()
            .Subscribe(_ => stateManager.Persist());
        this.request.QueryParams.ToObservableChangeSet<ObservableCollectionExtended<QueryParam>, QueryParam>()
            .AutoRefresh()
            .Subscribe(_ => stateManager.Persist());
    }
    
    public IReadOnlyList<HttpMethod> Methods { get; } = Enum.GetValues<HttpMethod>();

    private RequestNode request;
    
    public RequestNode Request
    {
        get => request;
        set => this.RaiseAndSetIfChanged(ref request, value);
    }
    

    public void AddHeader()
    { 
        Request.Headers.Add(new Header(Request.Headers.Count, string.Empty, string.Empty, true));
    }

    public void RemoveHeader(Header header)
    {
        Request.Headers.Remove(header);
    }
}
