using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IMainViewModel
{
    IReactiveObject CurrentPage { get; }
}

internal sealed class MainViewModel: ReactiveObject, IMainViewModel
{
    public MainViewModel(IRequestViewModel requestViewModel)
    {
        CurrentPage = requestViewModel;
    }

    public IReactiveObject CurrentPage { get; }
}
