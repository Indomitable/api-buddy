using System.Net.Http;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Api.Buddy.Main.UI.Controls.Request;
using Api.Buddy.Main.UI.Models;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IResponseViewModel
{
    int StatusCode { get; set; }
    IBody<object> Body { get; set; }
    Task SetResponse(HttpResponseMessage message, CancellationToken cancellationToken);
}

internal sealed class ResponseViewModel: ReactiveObject, IResponseViewModel
{

    public ResponseViewModel(IBodyDisplayDataTemplateSelector bodyDisplayDataTemplateSelector)
    {
        BodyDisplayDataTemplateSelector = bodyDisplayDataTemplateSelector;
    }
    
    private int statusCode;
    public int StatusCode
    {
        get => statusCode;
        set => this.RaiseAndSetIfChanged(ref statusCode, value);
    }

    private IBody<object> body = EmptyBody.Instance;
    public IBody<object> Body
    {
        get => body;
        set => this.RaiseAndSetIfChanged(ref body, value);
    }
    
    public IBodyDisplayDataTemplateSelector BodyDisplayDataTemplateSelector { get; }

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
                var content = await message.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                Body = bodyType == TextBodyType.Plain
                    ? new TextBody(content, true)
                    : new EnhancedTextBody(content, true, bodyType);
            }
            else
            {
                var data = await message.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                Body = new BinaryBody(data, true);
            }
        }
    }
}
