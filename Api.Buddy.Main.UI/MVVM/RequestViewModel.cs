using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Services;
using Api.Buddy.Main.Logic.Storage;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IRequestViewModel: IReactiveObject
{
    ICommand SendCommand { get; }
}

internal sealed class RequestViewModel : ReactiveObject, IRequestViewModel
{
    private readonly IRequestBuilder requestBuilder;
    private readonly IRequestExecutor requestExecutor;

    public RequestViewModel(IRequestBuilder requestBuilder, IRequestExecutor requestExecutor,
        IResponseViewModel responseViewModel, IStorageManager storageManager, RequestNode requestNode)
    {
        this.requestBuilder = requestBuilder;
        this.requestExecutor = requestExecutor;
        RequestInit = new RequestInitViewModel(storageManager, requestNode);
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
}
