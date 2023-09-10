using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Buddy.Main.Logic.Models.Request;

public static class RequestHelper
{
    public static HttpMethod TryGuessMethod(string name)
    {
        var dictionary = new Dictionary<HttpMethod, List<string>>
        {
            [HttpMethod.POST] = new(){ "Create", "Insert" },
            [HttpMethod.PUT] = new(){ "Update", "Set", "Modify" },
            [HttpMethod.DELETE] = new(){ "Delete", "Remove" }
        };
        foreach (var (key, value) in dictionary)
        {
            if (value.Any(v => name.StartsWith(v, StringComparison.OrdinalIgnoreCase)))
            {
                return key;
            }
        }
        return HttpMethod.GET;
    }
}