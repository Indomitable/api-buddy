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
        ProjectViewModel.WhenAnyValue(vm => vm.SelectedNode)
            .Subscribe(node =>
            {
                CurrentPage = node switch
                {
                    RequestNode requestNode => requestViewModelFactory.Create(requestNode),
                    FolderNode folderNode => new FolderViewModel(folderNode),
                    _ => new EmptyViewModel()
                };
            });
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
