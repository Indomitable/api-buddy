using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Api.Buddy.Main.UI.Models;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public class ResponseViewModel: ReactiveObject
{
    private int statusCode;
    public int StatusCode
    {
        get => statusCode;
        set => this.RaiseAndSetIfChanged(ref statusCode, value);
    }

    private IBody<object> content = EmptyBody.Instance;
    public IBody<object> Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public async Task SetResponse(HttpResponseMessage message, CancellationToken cancellationToken)
    {
        StatusCode = (int)message.StatusCode;
        if (message.Content.Headers.ContentLength > 0)
        {
            if (message.Content.Headers.ContentType is { MediaType: { } mediaType })
            {
                if (string.Equals(mediaType, System.Net.Mime.MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase)
                    || mediaType.StartsWith("text/"))
                {
                    // TODO: better handle the content type
                    var body = await message.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    Content = new TextBody(body, true);
                    return;
                }
            }

            var data = await message.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            Content = new BinaryBody(data, true);
            return;
        }
    }
}
