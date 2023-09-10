using System;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using Api.Buddy.Main.Dialogs.Services;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Storage;
using DynamicData;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IProjectViewModel: IDisposable
{
    ReadOnlyObservableCollection<Project> Projects { get; }
    Project? ActiveProject { get; set; }
    ProjectNode? SelectedNode { get; set; }
}

public sealed class ProjectViewModel: ReactiveObject, IProjectViewModel
{
    private readonly ITextInputDialogService textInputDialogService;
    private readonly IStateManager stateManager;
    private readonly Subject<ProjectNode> nodeCreated;

    public ProjectViewModel(ITextInputDialogService textInputDialogService, IStateManager stateManager)
    {
        this.textInputDialogService = textInputDialogService;
        this.stateManager = stateManager;
        // var tdAdminNode = new FolderNode { Name = "Countries" };
        // var capitalsNode = new FolderNode { Name = "Capitals", Parent = tdAdminNode };
        // var getCapitals = new RequestNode
        // {
        //     Name = "Get Capitals", Parent = capitalsNode,
        //     Request = RequestInit.Empty with
        //     {
        //         Url = "https://countriesnow.space/api/v0.1/countries/capital",
        //         Headers = new []
        //         {
        //             new Header
        //             {
        //                 Index = 0, Name = "Content-Type", Selected = false, Value = "application/json"
        //             }
        //         },
        //         QueryParams = new []
        //         {
        //             new QueryParam(Index: 0, Name: "test", Value: "1", Selected: true)
        //         }
        //     }
        // };
        // capitalsNode.Children.Add(getCapitals);
        //
        // var currenciesNode = new FolderNode { Name = "Currencies", Parent = tdAdminNode };
        // var getCurrencies = new RequestNode {
        //     Name = "Get Currencies",
        //     Parent = capitalsNode,
        //     Request = RequestInit.Empty with
        //     {
        //         Url = "https://countriesnow.space/api/v0.1/countries/currency"
        //     }
        // };
        // currenciesNode.Children.Add(getCurrencies);
        //
        // tdAdminNode.Children.Add(capitalsNode);
        // tdAdminNode.Children.Add(currenciesNode);
        //
        // var proj = new AvaloniaList<Project>
        // {
        //     new()
        //     {
        //         Name = "My Project",
        //         Nodes = { tdAdminNode }
        //     },
        //     new ()
        //     {
        //         Name = "Second Project",
        //         Nodes =
        //         {
        //             new RequestNode
        //             {
        //                 Name = "Google",
        //                 Request = RequestInit.Empty
        //             }
        //         }
        //     }
        // };

        stateManager.ConnectToProjects()
            .Bind(out projects)
            .Subscribe();
        
        CreateProjectCommand = ReactiveCommand.CreateFromTask(OnCreateProject);
        CreateTopFolderCommand = ReactiveCommand.CreateFromTask(OnCreateTopFolder);
        CreateFolderCommand = ReactiveCommand.CreateFromTask<FolderNode>(OnCreateFolder);
        CreateRequestCommand = ReactiveCommand.CreateFromTask<FolderNode>(OnCreateRequest);
        RenameFolderCommand = ReactiveCommand.CreateFromTask<FolderNode>(OnRenameFolder);
        nodeCreated = new Subject<ProjectNode>();
    }

    public ICommand CreateProjectCommand { get; }
    
    public ICommand CreateTopFolderCommand { get; }
    
    public ICommand CreateFolderCommand { get; }

    public ICommand CreateRequestCommand { get; }

    public ICommand RenameFolderCommand { get; }

    public IObservable<ProjectNode> NodeCreated => nodeCreated;

    private readonly ReadOnlyObservableCollection<Project> projects;
    public ReadOnlyObservableCollection<Project> Projects => projects;

    public Project? ActiveProject
    {
        get => stateManager.ActiveProject;
        set
        {
            this.RaisePropertyChanging();
            stateManager.ActiveProject = value;
            this.RaisePropertyChanged();
        }
    }

    private ProjectNode? selectedNode;

    public ProjectNode? SelectedNode
    {
        get => selectedNode;
        set => this.RaiseAndSetIfChanged(ref selectedNode, value);
    }

    private async Task OnCreateProject()
    {
        var input = await textInputDialogService.GetInput("Enter project name:");
        if (!string.IsNullOrEmpty(input))
        {
            stateManager.AddProject(input);
            this.RaisePropertyChanged(nameof(ActiveProject));
        }
    }

    private async Task OnCreateTopFolder()
    {
        var input = await textInputDialogService.GetInput("Enter folder name:");
        if (!string.IsNullOrEmpty(input) && ActiveProject is { } p)
        {
            var folder = stateManager.AddFolder(input, p, null);
            nodeCreated.OnNext(folder);
        }
    }
    
    private async Task OnCreateFolder(FolderNode folder)
    {
        var input = await textInputDialogService.GetInput("Enter folder name:");
        if (!string.IsNullOrEmpty(input))
        {
            var child = stateManager.AddFolder(input, folder.Project, folder);
            nodeCreated.OnNext(child);
        }
    }

    private async Task OnCreateRequest(FolderNode folder)
    {
        var input = await textInputDialogService.GetInput("Enter request name:");
        if (!string.IsNullOrEmpty(input))
        {
            var request = stateManager.AddRequestNode(input, folder); 
            nodeCreated.OnNext(request);
        }
    }

    private async Task OnRenameFolder(FolderNode folder)
    {
        var input = await textInputDialogService.GetInput("Enter new name:", folder.Name);
        if (!string.IsNullOrEmpty(input))
        {
            stateManager.ChangeFolderName(folder, input);
        }
    }


    public void Dispose()
    {
        nodeCreated.Dispose();
    }
}
