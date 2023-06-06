using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Api.Buddy.Main.Logic.Models;
using Api.Buddy.Main.Logic.Models.Request;

namespace Api.Buddy.Main.Logic.Extensions;

internal static class RequestInitExtensions
{
    public static void AddRange(this HttpRequestHeaders requestHeaders, IReadOnlyList<Header> headers)
    {
        var group = headers
            .Where(h => h.Selected)
            .OrderBy(h => h.Index)
            .GroupBy(h => h.Name)
            .Select(gr => (Name: gr.Key, Values: gr.Select(h => h.Value).ToList()));


        foreach (var (name, values) in group)
        {
            if (values.Count == 1)
            {
                requestHeaders.Add(name, values[0]);
            }
            else
            {
                requestHeaders.Add(name, values);
            }

        }
    }
}
