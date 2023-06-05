using System;
using System.Collections.Generic;
using Api.Buddy.Main.Logic.Models;
using Avalonia.Collections;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IRequestInitViewModel
{
    HttpMethod Method { get; set; }
    IReadOnlyList<HttpMethod> Methods { get; }
    string Url { get; set; }
}

public class RequestInitViewModel : ReactiveObject, IRequestInitViewModel
{
    public RequestInitViewModel()
    {
        Methods = Enum.GetValues<HttpMethod>();
        Headers = new AvaloniaList<Header>();
    }

    private HttpMethod method = HttpMethod.GET;

    public HttpMethod Method
    {
        get => method;
        set => this.RaiseAndSetIfChanged(ref method, value);
    }

    public IReadOnlyList<HttpMethod> Methods { get; }

    public AvaloniaList<Header> Headers { get; }

    private string url = string.Empty;

    public string Url
    {
        get => url;
        set => this.RaiseAndSetIfChanged(ref url, value);
    }

    public void AddHeader()
    {
        Headers.Add(new Header
        {
            Index = Headers.Count,
            Name = string.Empty,
            Value = string.Empty,
            Selected = true
        });
    }

    public void RemoveHeader(Header header)
    {
        Headers.Remove(header);
    }
}
