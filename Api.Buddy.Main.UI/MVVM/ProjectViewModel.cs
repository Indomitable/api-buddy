using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using Api.Buddy.Main.Dialogs.Services;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Models.Request;
using Avalonia.Collections;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IProjectViewModel: IDisposable
{
    AvaloniaList<Project> Projects { get; }
    Project? Project { get; set; }
    ProjectNode? SelectedNode { get; set; }
}

public sealed class ProjectViewModel: ReactiveObject, IProjectViewModel
{
    private readonly ITextInputDialogService textInputDialogService;
    private readonly Subject<ProjectNode> nodeCreated;

    public ProjectViewModel(ITextInputDialogService textInputDialogService)
    {
        this.textInputDialogService = textInputDialogService;
        var tdAdminNode = new FolderNode { Name = "Countries" };
        var capitalsNode = new FolderNode { Name = "Capitals", Parent = tdAdminNode };
        var getCapitals = new RequestNode
        {
            Name = "Get Capitals", Parent = capitalsNode,
            Request = RequestInit.Empty with
            {
                Url = "https://countriesnow.space/api/v0.1/countries/capital"
            }
        };
        capitalsNode.Children.Add(getCapitals);

        var currenciesNode = new FolderNode { Name = "Currencies", Parent = tdAdminNode };
        var getCurrencies = new RequestNode {
            Name = "Get Currencies",
            Parent = capitalsNode,
            Request = RequestInit.Empty with
            {
                Url = "https://countriesnow.space/api/v0.1/countries/currency"
            }
        };
        currenciesNode.Children.Add(getCurrencies);

        tdAdminNode.Children.Add(capitalsNode);
        tdAdminNode.Children.Add(currenciesNode);

        Projects = new AvaloniaList<Project>
        {
            new()
            {
                Name = "My Project",
                Nodes = { tdAdminNode }
            },
            new ()
            {
                Name = "Second Project",
                Nodes =
                {
                    new RequestNode
                    {
                        Name = "Google",
                        Request = RequestInit.Empty
                    }
                }
            }
        };
        project = Projects.First();
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

    public AvaloniaList<Project> Projects { get; }

    private Project? project;
    public Project? Project
    {
        get => project;
        set => this.RaiseAndSetIfChanged(ref project, value);
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
            var item = new Project { Name = input };
            Projects.Add(item);
            Project = item;
        }
    }

    private async Task OnCreateTopFolder()
    {
        var input = await textInputDialogService.GetInput("Enter folder name:");
        if (!string.IsNullOrEmpty(input))
        {
            Project?.Nodes.Add(new FolderNode { Name = input, Parent = null });
        }
    }
    
    private async Task OnCreateFolder(FolderNode folder)
    {
        var input = await textInputDialogService.GetInput("Enter folder name:");
        if (!string.IsNullOrEmpty(input))
        {
            var child = new FolderNode { Name = input, Parent = folder, };
            folder.Children.Insert(folder.GetIndex(child), child);
            nodeCreated.OnNext(child);
        }
    }

    private async Task OnCreateRequest(FolderNode folder)
    {
        var input = await textInputDialogService.GetInput("Enter request name:");
        if (!string.IsNullOrEmpty(input))
        {
            var child = new RequestNode
            {
                Name = input,
                Parent = folder,
                Request = RequestInit.Empty with
                {
                    Method = TryGuessMethod(input)
                }
            };
            folder.Children.Insert(folder.GetIndex(child), child);
            nodeCreated.OnNext(child);
        }
    }

    private async Task OnRenameFolder(FolderNode folder)
    {
        var input = await textInputDialogService.GetInput("Enter new name:", folder.Name);
        if (!string.IsNullOrEmpty(input))
        {
            folder.Name = input;
        }
    }

    private HttpMethod TryGuessMethod(string name)
    {
        var dictionary = new Dictionary<HttpMethod, List<string>>
        {
            [HttpMethod.POST] = new(){ "Create", "Insert" },
            [HttpMethod.PUT] = new(){ "Update", "Set", "Modify" },
            [HttpMethod.DELETE] = new(){ "Delete", "Remove" }
        };
        foreach (var (key, value) in dictionary)
        {
            if (value.Any(v => name.StartsWith(v, StringComparison.OrdinalIgnoreCase)))
            {
                return key;
            }
        }
        return HttpMethod.GET;
    }

    public void Dispose()
    {
    }
}
