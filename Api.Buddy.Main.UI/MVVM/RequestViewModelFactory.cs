using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Services;

namespace Api.Buddy.Main.UI.MVVM;

internal interface IRequestViewModelFactory
{
    IRequestViewModel Create(RequestNode requestNode);
}

internal sealed class RequestViewModelFactory : IRequestViewModelFactory
{
    private readonly IRequestBuilder requestBuilder;
    private readonly IRequestExecutor requestExecutor;

    public RequestViewModelFactory(IRequestBuilder requestBuilder, IRequestExecutor requestExecutor)
    {
        this.requestBuilder = requestBuilder;
        this.requestExecutor = requestExecutor;
    }

    public IRequestViewModel Create(RequestNode requestNode)
    {
        var viewModel = new RequestViewModel(requestBuilder, requestExecutor)
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
