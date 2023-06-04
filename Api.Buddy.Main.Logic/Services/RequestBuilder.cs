using System.Collections.Generic;
using Api.Buddy.Main.Logic.Models;
using HttpMethod = Api.Buddy.Main.Logic.Models.HttpMethod;

namespace Api.Buddy.Main.Logic.Services;

public interface IRequestBuilder
{
    IRequestBuilder SetMethod(HttpMethod method);
    IRequestBuilder SetUrl(string url);
    IRequestBuilder SetHeaders(IReadOnlyList<Header> headers);
    RequestInit Build();
}

internal sealed class RequestBuilder : IRequestBuilder
{
    private readonly RequestInit requestInit;

    public RequestBuilder()
    {
        requestInit = new RequestInit();
    }

    public IRequestBuilder SetMethod(HttpMethod method)
    {
        requestInit.Method = method;
        return this;
    }
    
    public IRequestBuilder SetUrl(string url)
    {
        requestInit.Url = url;
        return this;
    }

    public IRequestBuilder SetHeaders(IReadOnlyList<Header> headers)
    {
        requestInit.Headers = headers;
        return this;
    }

    public RequestInit Build()
    {
        return requestInit;
    }
}
