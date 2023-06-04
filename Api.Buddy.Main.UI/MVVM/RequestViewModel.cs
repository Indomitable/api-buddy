using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Api.Buddy.Main.Logic.Models;
using Api.Buddy.Main.Logic.Services;
using Api.Buddy.Main.UI.Controls;
using Avalonia.Collections;
using ReactiveUI;
using HttpMethod = Api.Buddy.Main.Logic.Models.HttpMethod;
using RequestView = Api.Buddy.Main.UI.Controls.Request.RequestView;

namespace Api.Buddy.Main.UI.MVVM;

public interface IRequestViewModel: IReactiveObject
{
    ICommand SendCommand { get; }
}

[Control(typeof(RequestView))]
internal sealed class RequestViewModel : ReactiveObject, IRequestViewModel
{
    private readonly IRequestBuilder requestBuilder;
    private readonly IRequestExecutor requestExecutor;

    public RequestViewModel(IRequestBuilder requestBuilder, IRequestExecutor requestExecutor)
    {
        this.requestBuilder = requestBuilder;
        this.requestExecutor = requestExecutor;
        RequestInit = new RequestInitViewModel();
        Response = new ResponseViewModel();
        SendCommand = ReactiveCommand.CreateFromTask(Send);
    }

    public RequestInitViewModel RequestInit { get; }

    public ResponseViewModel Response { get; }

    public ICommand SendCommand { get; }

    private async Task Send(CancellationToken cancellationToken)
    {
        var requestInit = requestBuilder
            .SetMethod(RequestInit.Method)
            .SetUrl(RequestInit.Url)
            .SetHeaders(RequestInit.Headers)
            .Build();
        var responseMessage = await requestExecutor.Execute(requestInit);
        await Response.SetResponse(responseMessage, cancellationToken);
    }
}
