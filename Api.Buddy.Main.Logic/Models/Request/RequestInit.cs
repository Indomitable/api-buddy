using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Buddy.Main.Logic.Models.Request;

public sealed record RequestInit
(
    HttpMethod Method,
    string Url,
    IReadOnlyList<QueryParam> QueryParams,
    IReadOnlyList<Header> Headers
)
{
    // public HttpMethod Method { get; internal set; } = HttpMethod.GET;
    // public string Url { get; internal set; } = string.Empty;
    // public IReadOnlyList<QueryParam> QueryParams { get; internal set; } = Array.Empty<QueryParam>();
    // public IReadOnlyList<Header> Headers { get; internal set; } = Array.Empty<Header>();

    internal Uri BuildUri()
    {
        var parameters = QueryParams.Where(q => q.Selected)
            .OrderBy(q => q.Index)
            .Select(q => $"{HttpUtility.UrlEncode(q.Name)}={HttpUtility.UrlEncode(q.Value)}");
        var query = string.Join('&', parameters);
        if (string.IsNullOrEmpty(query))
        {
            return new Uri(Url);
        }
        return new Uri($"{Url}?{query}");
    }

    public static RequestInit Empty = new RequestInit
    (
        HttpMethod.GET,
        string.Empty,
        new List<QueryParam>(),
        new List<Header>()
    );
}
