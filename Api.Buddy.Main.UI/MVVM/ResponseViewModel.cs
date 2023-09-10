using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api.Buddy.Main.Logic.Services;
using Api.Buddy.Main.UI.Controls.Request;
using Api.Buddy.Main.UI.Models;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IResponseViewModel
{
    int StatusCode { get; set; }
    IBody<object> Body { get; set; }
    Task SetResponse(HttpResponse response, CancellationToken cancellationToken);
}

internal sealed class ResponseViewModel: ReactiveObject, IResponseViewModel
{
    public ResponseViewModel(IBodyDisplayDataTemplateSelector bodyDisplayDataTemplateSelector)
    {
        BodyDisplayDataTemplateSelector = bodyDisplayDataTemplateSelector;
        hasResponse = false;
    }
    
    private bool hasResponse;
    public bool HasResponse
    {
        get => hasResponse;
        set => this.RaiseAndSetIfChanged(ref hasResponse, value);
    }
    
    private int statusCode;
    public int StatusCode
    {
        get => statusCode;
        set => this.RaiseAndSetIfChanged(ref statusCode, value);
    }
    
    private string version;
    public string Version
    {
        get => version;
        set => this.RaiseAndSetIfChanged(ref version, value);
    }
    
    private TimeSpan duration;
    public TimeSpan Duration
    {
        get => duration;
        set => this.RaiseAndSetIfChanged(ref duration, value);
    }

    private long? contentSize;
    public long? ContentSize
    {
        get => contentSize;
        set => this.RaiseAndSetIfChanged(ref contentSize, value);
    }
    
    private IBody<object> body = EmptyBody.Instance;
    public IBody<object> Body
    {
        get => body;
        set => this.RaiseAndSetIfChanged(ref body, value);
    }

    private ResponseHeadersViewModel responseHeadersViewModel = new ();
    public ResponseHeadersViewModel ResponseHeadersViewModel
    {
        get => responseHeadersViewModel;
        set => this.RaiseAndSetIfChanged(ref responseHeadersViewModel, value);
    }
    
    public IBodyDisplayDataTemplateSelector BodyDisplayDataTemplateSelector { get; }

    public async Task SetResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        var (message, d) = response;
        StatusCode = (int)message.StatusCode;
        Duration = d;
        var versionBuilder = new StringBuilder($"HTTP/{message.Version.Major}");
        if (message.Version.Minor > 0)
        {
            versionBuilder.Append($".{message.Version.Minor}");
        }
        Version = versionBuilder.ToString();
        ResponseHeadersViewModel = new ResponseHeadersViewModel(message);
        if (message.Content.Headers.ContentLength > 0)
        {
            ContentSize = message.Content.Headers.ContentLength;
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
        HasResponse = true;
    }
}
