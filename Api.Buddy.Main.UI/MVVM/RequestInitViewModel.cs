using System;
using System.Collections.Generic;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Models.Request;
using Api.Buddy.Main.Logic.Storage;
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
    public RequestInitViewModel(IStorageManager storageManager, RequestNode request)
    {
        this.request = request;
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
        Request.Headers.Add(new Header
        {
            Index = Request.Headers.Count,
            Name = string.Empty,
            Value = string.Empty,
            Selected = true
        });
    }

    public void RemoveHeader(Header header)
    {
        Request.Headers.Remove(header);
    }
}
