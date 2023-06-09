using System.Net.Http;
using System.Net.Mime;
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
                var bodyType = mediaType switch
                {
                    MediaTypeNames.Application.Json => TextBodyType.Json,
                    MediaTypeNames.Application.Xml => TextBodyType.XML,
                    "application/xhtml+xml" => TextBodyType.XML,
                    "text/xml" => TextBodyType.XML,
                    MediaTypeNames.Text.Html => TextBodyType.HTML,
                    "text/javascript" => TextBodyType.JavaScript,
                    "application/javascript" => TextBodyType.JavaScript,
                    "text/css" => TextBodyType.CSS,
                    _ => TextBodyType.Plain
                };
                var body = await message.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                Content = bodyType == TextBodyType.Plain
                    ? new TextBody(body, true)
                    : new EnhancedTextBody(body, true, bodyType);
            }
            else
            {
                var data = await message.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                Content = new BinaryBody(data, true);
            }
        }
    }
}
