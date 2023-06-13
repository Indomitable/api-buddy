using System.Linq;
using System.Windows.Input;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Models.Request;
using Avalonia.Collections;
using ReactiveUI;

namespace Api.Buddy.Main.UI.MVVM;

public interface IProjectViewModel
{
    AvaloniaList<Project> Projects { get; }
    Project? Project { get; set; }
    ProjectNode? SelectedNode { get; set; }
}

internal sealed class ProjectViewModel: ReactiveObject, IProjectViewModel
{
    public ProjectViewModel()
    {
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

        CreateProjectCommand = ReactiveCommand.Create<string>(OnCreateProject);
        CreateTopFolderCommand = ReactiveCommand.Create<string>(OnCreateTopFolder);
        CreateFolderCommand = ReactiveCommand.Create<string>(OnCreateFolder);
    }

    public ICommand CreateProjectCommand { get; }
    
    public ICommand CreateTopFolderCommand { get; }
    
    public ICommand CreateFolderCommand { get; }

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

    private void OnCreateProject(string projectName)
    {
        var item = new Project { Name = projectName };
        Projects.Add(item);
        Project = item;
    }

    private void OnCreateTopFolder(string folderName)
    {
        Project?.Nodes.Add(new FolderNode { Name = folderName, Parent = null });
    }
    
    private void OnCreateFolder(string folderName)
    {
        Project?.Nodes.Add(new FolderNode { Name = folderName, Parent = null });
    }
}
