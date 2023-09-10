using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Buddy.Main.Logic.Extensions;
using Api.Buddy.Main.Logic.Models.Request;

namespace Api.Buddy.Main.Logic.Services;

public interface IRequestExecutor
{
    Task<HttpResponse?> Execute(RequestInit requestInit);
}

public record HttpResponse(HttpResponseMessage Message, TimeSpan Duration);
    

internal sealed class RequestExecutor : IRequestExecutor
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IMapper mapper;

    public RequestExecutor(IHttpClientFactory httpClientFactory, IMapper mapper)
    {
        this.httpClientFactory = httpClientFactory;
        this.mapper = mapper;
    }

    public async Task<HttpResponse?> Execute(RequestInit requestInit)
    {
        try
        {
            using var client = httpClientFactory.CreateClient();
            var message = new HttpRequestMessage();
            message.Method = mapper.ToSystemHttpMethod(requestInit.Method);
            message.RequestUri = requestInit.BuildUri();
            message.Headers.AddRange(requestInit.Headers);
            message.Version = new Version(3, 0);
            var stopwatch = Stopwatch.StartNew();
            var response = await client.SendAsync(message);
            stopwatch.Stop();
            return new HttpResponse(response, stopwatch.Elapsed);
        }
        catch (Exception)
        {
            // TODO: create an execution log and how the exception there. 
            return null;
        }
    }
}
