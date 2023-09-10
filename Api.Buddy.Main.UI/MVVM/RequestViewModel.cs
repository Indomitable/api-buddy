using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Services;
using Api.Buddy.Main.Logic.Storage;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IRequestViewModel: IReactiveObject, IDisposable
{
    ICommand SendCommand { get; }
}

internal sealed class RequestViewModel : ReactiveObject, IRequestViewModel
{
    private readonly IRequestBuilder requestBuilder;
    private readonly IRequestExecutor requestExecutor;
    private readonly CompositeDisposable disposable = new ();

    public RequestViewModel(IRequestBuilder requestBuilder, IRequestExecutor requestExecutor,
        IResponseViewModel responseViewModel, IStateManager stateManager, RequestNode requestNode)
    {
        this.requestBuilder = requestBuilder;
        this.requestExecutor = requestExecutor;
        RequestInit = new RequestInitViewModel(stateManager, requestNode).DisposeWith(disposable);
        Response = responseViewModel;
        SendCommand = ReactiveCommand.CreateFromTask(Send);
    }

    public RequestInitViewModel RequestInit { get; }

    public IResponseViewModel Response { get; }

    public ICommand SendCommand { get; }

    private async Task Send(CancellationToken cancellationToken)
    {
        var requestInit = requestBuilder
            .SetMethod(RequestInit.Request.Method)
            .SetUrl(RequestInit.Request.Url)
            .SetHeaders(RequestInit.Request.Headers)
            .Build();
        var responseMessage = await requestExecutor.Execute(requestInit);
        if (responseMessage is not null)
        {
            await Response.SetResponse(responseMessage, cancellationToken);
        }
    }

    public void Dispose()
    {
        disposable.Dispose();
    }
}
