using System.Collections.Generic;
using Api.Buddy.Main.Logic.Models.Request;
using HttpMethod = Api.Buddy.Main.Logic.Models.Request.HttpMethod;

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
    private RequestInit requestInit = RequestInit.Empty;

    public IRequestBuilder SetMethod(HttpMethod method)
    {
        requestInit = requestInit with
        {
            Method = method
        };
        return this;
    }
    
    public IRequestBuilder SetUrl(string url)
    {
        requestInit = requestInit with
        {
            Url = url
        };
        return this;
    }

    public IRequestBuilder SetHeaders(IReadOnlyList<Header> headers)
    {
        requestInit = requestInit with
        {
            Headers = headers
        };
        return this;
    }

    public RequestInit Build()
    {
        return requestInit;
    }
}
