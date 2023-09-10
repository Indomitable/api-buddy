using System;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Buddy.Main.Logic.Extensions;
using Api.Buddy.Main.Logic.Models.Request;

namespace Api.Buddy.Main.Logic.Services;

public interface IRequestExecutor
{
    Task<HttpResponseMessage?> Execute(RequestInit requestInit);
}

internal sealed class RequestExecutor : IRequestExecutor
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IMapper mapper;

    public RequestExecutor(IHttpClientFactory httpClientFactory, IMapper mapper)
    {
        this.httpClientFactory = httpClientFactory;
        this.mapper = mapper;
    }

    public async Task<HttpResponseMessage?> Execute(RequestInit requestInit)
    {
        try
        {
            using var client = httpClientFactory.CreateClient();
            var message = new HttpRequestMessage();
            message.Method = mapper.ToSystemHttpMethod(requestInit.Method);
            message.RequestUri = requestInit.BuildUri();
            message.Headers.AddRange(requestInit.Headers);
            var response = await client.SendAsync(message);
            return response; // TODO: return custom object
        }
        catch (Exception)
        {
            // TODO: create an execution log and how the exception there. 
            return null;
        }
    }
}
