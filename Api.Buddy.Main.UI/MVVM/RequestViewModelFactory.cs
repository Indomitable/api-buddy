using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Services;
using Api.Buddy.Main.Logic.Storage;

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
    private readonly IStateManager stateManager;

    public RequestViewModelFactory(IRequestBuilder requestBuilder, IRequestExecutor requestExecutor, IResponseViewModel responseViewModel, 
        IStateManager stateManager)
    {
        this.requestBuilder = requestBuilder;
        this.requestExecutor = requestExecutor;
        this.responseViewModel = responseViewModel;
        this.stateManager = stateManager;
    }

    public IRequestViewModel Create(RequestNode requestNode)
    {
        return new RequestViewModel(requestBuilder, requestExecutor, responseViewModel, stateManager, requestNode);
    }
}
