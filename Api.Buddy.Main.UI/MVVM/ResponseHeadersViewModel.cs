using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Api.Buddy.Main.UI.MVVM;

public class ResponseHeadersViewModel
{
    public record HeaderView(string Key, string Value);

    public ResponseHeadersViewModel()
    {
        Headers = Array.Empty<HeaderView>();
        HasTrailingHeaders = false;
    }
    
    public ResponseHeadersViewModel(HttpResponseMessage httpResponseMessage)
    {
        Headers = GetHeaders(httpResponseMessage.Headers.Union(httpResponseMessage.Content.Headers));
        HasTrailingHeaders = httpResponseMessage.TrailingHeaders.Any();
        if (HasTrailingHeaders)
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Trailer
            TrailingHeaders = GetHeaders(httpResponseMessage.TrailingHeaders);
        }
    }
    public IReadOnlyList<HeaderView> Headers { get; }
    public bool HasTrailingHeaders { get; }
    public IReadOnlyList<HeaderView> TrailingHeaders { get; } = Array.Empty<HeaderView>();
    private IReadOnlyList<HeaderView> GetHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
    {
        return headers.SelectMany(kv => kv.Value.Select(v => new HeaderView(kv.Key, v)))
            .ToList();
    }
}