using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Services;

namespace Api.Buddy.Main.UI.MVVM;

public interface IRequestViewModelFactory
{
    IRequestViewModel Create(RequestNode requestNode);
}

internal sealed class RequestViewModelFactory : IRequestViewModelFactory
{
    private readonly IRequestBuilder requestBuilder;
    private readonly IRequestExecutor requestExecutor;
    private readonly IResponseViewModel responseViewModel;

    public RequestViewModelFactory(IRequestBuilder requestBuilder, IRequestExecutor requestExecutor, IResponseViewModel responseViewModel)
    {
        this.requestBuilder = requestBuilder;
        this.requestExecutor = requestExecutor;
        this.responseViewModel = responseViewModel;
    }

    public IRequestViewModel Create(RequestNode requestNode)
    {
        var viewModel = new RequestViewModel(requestBuilder, requestExecutor, responseViewModel)
        {
            RequestInit =
            {
                Method = requestNode.Request.Method,
                Url = requestNode.Request.Url
            }
        };
        viewModel.RequestInit.Headers.AddRange(requestNode.Request.Headers);
        return viewModel;
    }
}
