using System;
using System.Reactive;
using Api.Buddy.Main.Logic.Models.Project;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IMainViewModel: IDisposable
{
    IReactiveObject? CurrentPage { get; }
}

public sealed class MainViewModel: ReactiveObject, IMainViewModel
{
    public MainViewModel(IRequestViewModelFactory requestViewModelFactory, IProjectViewModel projectViewModel)
    {
        ProjectViewModel = projectViewModel;
        currentPage = new EmptyViewModel();
        ProjectViewModel.ObservableForProperty(vm => vm.SelectedNode)
            .Subscribe(new AnonymousObserver<IObservedChange<IProjectViewModel, ProjectNode?>>((change) =>
            {
                switch (change.Value)
                {
                    case RequestNode requestNode:
                        CurrentPage = requestViewModelFactory.Create(requestNode);
                        break;
                    case FolderNode folderNode:
                        CurrentPage = new FolderViewModel(folderNode);
                        break;
                    default:
                        CurrentPage = new EmptyViewModel();
                        break;
                }
            }));
    }

    private IReactiveObject currentPage;
    public IReactiveObject CurrentPage
    {
        get => currentPage;
        set => this.RaiseAndSetIfChanged(ref currentPage, value);
    }

    public IProjectViewModel ProjectViewModel { get; }

    public void Dispose()
    {
        ProjectViewModel.Dispose();
    }
}
