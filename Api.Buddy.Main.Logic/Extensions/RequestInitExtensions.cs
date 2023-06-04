using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Api.Buddy.Main.Logic.Models;

namespace Api.Buddy.Main.Logic.Extensions;

internal static class RequestInitExtensions
{
    public static void AddRange(this HttpRequestHeaders requestHeaders, IEnumerable<Header> headers)
    {
        var group = headers
            .Where(h => h.Selected)
            .OrderBy(h => h.Index)
            .GroupBy(h => h.Name)
            .Select(gr => (Name: gr.Key, Values: gr.Select(h => h.Value)));

        foreach (var (name, values) in group)
        {
            requestHeaders.Add(name, values);
        }
    }
}
