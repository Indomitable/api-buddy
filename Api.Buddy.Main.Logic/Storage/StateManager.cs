using System;
using System.Linq;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Models.Request;
using DynamicData;
using ReactiveUI;

namespace Api.Buddy.Main.Logic.Storage;

public interface IStateManager
{
    IObservable<IChangeSet<Project>> ConnectToProjects();
    Project? ActiveProject { get; set; }
    
    void UpdateActiveProject(Guid? id);
    void AddProject(string name);
    void UpdateProject(Project project);
    void DeleteProject(Project project);
    FolderNode AddFolder(string name, Project project, FolderNode? parentFolder);
    void ChangeFolderName(FolderNode folder, string newName);
    void DeleteFolder(FolderNode folder);
    RequestNode AddRequestNode(string name, FolderNode parentFolder);
    void DeleteRequest(RequestNode request);
    void Persist();
}

public class StateManager : ReactiveObject, IStateManager
{
    private readonly IStorageManager storageManager;

    private readonly SourceList<Project> projects;

    public StateManager(IStorageManager storageManager)
    {
        this.storageManager = storageManager;
        var storage = storageManager.Load();
        projects = new SourceList<Project>();
        projects.AddRange(storage.Projects);
        ActiveProject = storage.SelectedProject.HasValue
            ? projects.Items.FirstOrDefault(p => p.Id == storage.SelectedProject.Value)
            : projects.Items.FirstOrDefault();
    }

    private Project? activeProject;
    public Project? ActiveProject
    {
        get => activeProject;
        set
        {
            this.RaiseAndSetIfChanged(ref activeProject, value);
            Persist();
        }
    }

    public IObservable<IChangeSet<Project>> ConnectToProjects()
    {
        return projects.Connect();
    }

    public void UpdateActiveProject(Guid? id)
    {
        var storage = storageManager.Load() with
        {
            SelectedProject = id
        };
        storageManager.Save(storage);
    }

    public void AddProject(string name)
    {
        var item = new Project(Guid.NewGuid(), name);
        projects.Add(item);
        ActiveProject = item;
        Persist();
    }
    
    public void UpdateProject(Project project)
    {
        throw new NotImplementedException();
    }
    
    public void DeleteProject(Project project)
    {
        throw new NotImplementedException();
    }
    
    public FolderNode AddFolder(string name, Project project, FolderNode? parentFolder)
    {
        var folder = new FolderNode(Guid.NewGuid(), name, project, parentFolder);
        if (parentFolder is null)
        {
            project.Nodes.Add(folder);
        }
        else
        {
            parentFolder.Children.Insert(folder.GetIndex(folder), folder);
        }
        Persist();
        return folder;
    }
    
    public void ChangeFolderName(FolderNode folder, string newName)
    {
        folder.Name = newName;
        Persist();
    }
    
    public void DeleteFolder(FolderNode folder)
    {
        throw new NotImplementedException();
    }
    
    public RequestNode AddRequestNode(string name, FolderNode parentFolder)
    {
        var request = new RequestNode(Guid.NewGuid(), name, parentFolder.Project, parentFolder)
        {
            Method = RequestHelper.TryGuessMethod(name),
            Url = "https://",
        };
        parentFolder.Children.Insert(parentFolder.GetIndex(request), request);
        Persist();
        return request;
    }
    
    public void DeleteRequest(RequestNode request)
    {
        throw new NotImplementedException();
    }

    public void Persist()
    {
        var storage = new Storage(projects.Items, ActiveProject?.Id);
        storageManager.Save(storage);
    }
}