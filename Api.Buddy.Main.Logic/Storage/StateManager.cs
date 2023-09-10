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
    Guid? ActiveProjectId { get; set; }
    
    void UpdateActiveProject(Guid? id);
    void AddProject(string name);
    void UpdateProject(Project project);
    void DeleteProject(Project project);
    FolderNode AddFolder(string name, Project project, FolderNode? parentFolder);
    void ChangeFolderName(FolderNode folder, string newName);
    void DeleteFolder(FolderNode folder);
    RequestNode AddRequestNode(string name, FolderNode parentFolder);
    void UpdateRequest(RequestNode request);
    void DeleteRequest(RequestNode request);
}

public class StateManager : ReactiveObject, IStateManager
{
    private readonly IStorageManager storageManager;

    private readonly SourceList<Project> projects;
    // private readonly CompositeDisposable disposables;
    // private IEnumerable<Project>? watchedProjects;
    // private Dictionary<Guid, IDisposable> wathables;

    public StateManager(IStorageManager storageManager)
    {
        this.storageManager = storageManager;
        var storage = storageManager.Load();
        projects = new SourceList<Project>();
        projects.AddRange(storage.Projects);
        ActiveProjectId = storage.SelectedProject.HasValue
            ? storage.Projects.FirstOrDefault(p => p.Id == storage.SelectedProject.Value)?.Id
            : storage.Projects.FirstOrDefault()?.Id;
    }

    private Guid? activeProjectId;
    public Guid? ActiveProjectId
    {
        get => activeProjectId;
        set
        {
            this.RaiseAndSetIfChanged(ref activeProjectId, value);
            UpdateStorage();
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
        ActiveProjectId = item.Id;
        UpdateStorage();
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
        UpdateStorage();
        return folder;
    }
    
    public void ChangeFolderName(FolderNode folder, string newName)
    {
        folder.Name = newName;
        UpdateStorage();
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
        UpdateStorage();
        return request;
    }
    
    public void UpdateRequest(RequestNode request)
    {
        
    }
    
    public void DeleteRequest(RequestNode request)
    {
        throw new NotImplementedException();
    }

    private void UpdateStorage()
    {
        var storage = new Storage(projects.Items, ActiveProjectId);
        storageManager.Save(storage);
    }

    // private IEnumerable<Project> AddProjectNode(IEnumerable<Project> projects, ProjectNode node)
    // {
    //     foreach (var project in projects)
    //     {
    //         if (node.Project.Id == project.Id)
    //         {
    //             yield return AddProjectNode(project, node);
    //         }
    //         else
    //         {
    //             yield return project;
    //         }
    //     }
    // }

    // private Project AddProjectNode(Project project, ProjectNode node)
    // {
    //     if (node.Parent is null)
    //     {
    //         project.Nodes.Add(node);
    //         return project;
    //     }
    //     else
    //     {
    //         var parent = FindParent(project.Nodes.OfType<FolderNode>(), node);
    //         if (parent is not null)
    //         {
    //             parent.Children
    //         }
    //     }
    // }

    // private FolderNode? FindParent(IEnumerable<FolderNode> nodes, ProjectNode node)
    // {
    //     foreach (var projectNode in nodes)
    //     {
    //         return node.Parent!.Id == projectNode.Id 
    //             ? projectNode 
    //             : FindParent(projectNode.Children.OfType<FolderNode>(), node);
    //     }
    //     return null;
    // }

    // public void Watch(IObservableCollection<Project> projects)
    // {
    //     watchedProjects = projects;
    //     WatchCollectionForChanges(projects, disposables);
    //     foreach (var project in projects)
    //     {
    //         WatchProject(project, disposables);
    //     }
    // }
    //
    // private void WatchCollectionForChanges<T>(IObservableCollection<T> nodes, CompositeDisposable parentDisposable)
    //     where T: INode
    // {
    //     nodes.ToObservableChangeSet<IObservableCollection<INode>, INode>().Subscribe(changes =>
    //     {
    //         foreach (var change in changes)
    //         {
    //             switch (change.Reason)
    //             {
    //                 case ListChangeReason.Add:
    //                     WatchNode(change.Item.Current, parentDisposable);
    //                     break;
    //                 case ListChangeReason.Remove:
    //                 {
    //                     var id = change.Item.Current.Id;
    //                     if (wathables.TryGetValue(id, out var d))
    //                     {
    //                         d.Dispose();
    //                     }
    //                     break;
    //                 }
    //             }
    //         }
    //     }).DisposeWith(parentDisposable);
    // }
    //
    // private void WatchNode(INode node, CompositeDisposable parentDisposable)
    // {
    //     switch (node)
    //     {
    //         case Project project:
    //             WatchProject(project, parentDisposable);
    //             break;
    //         case FolderNode folderNode:
    //             WatchFolderNode(folderNode, parentDisposable);
    //             break;
    //         case RequestNode requestNode:
    //             WatchRequestNode(requestNode, parentDisposable);
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(node));
    //     }
    // }
    //
    // private void WatchProject(Project project, CompositeDisposable parentDisposable)
    // {
    //     var disposable = new CompositeDisposable().DisposeWith(parentDisposable);
    //     project.WhenAnyValue(p => p.Name).Subscribe(_ => Save()).DisposeWith(disposable);
    //     WatchProjectNodes(project.Nodes, disposable);
    //     wathables.Add(project.Id, disposable);
    // }
    //
    // private void WatchProjectNodes(IObservableCollection<ProjectNode> nodes, CompositeDisposable parentDisposable)
    // {
    //     WatchCollectionForChanges(nodes, parentDisposable);
    //     foreach (var node in nodes)
    //     {
    //         switch (node)
    //         {
    //             case FolderNode fn:
    //                 WatchFolderNode(fn, parentDisposable);
    //                 break;
    //             case RequestNode rn:
    //                 WatchRequestNode(rn, parentDisposable);
    //                 break;
    //         }
    //     }
    // }
    //
    // private void WatchFolderNode(FolderNode fn, CompositeDisposable parentDisposable)
    // {
    //     var disposable = new CompositeDisposable().DisposeWith(parentDisposable);
    //     fn.WhenAnyValue(n => n.Name).Subscribe(_ => Save()).DisposeWith(disposable);
    //     WatchProjectNodes(fn.Children, disposable);
    //     wathables.Add(fn.Id, disposable);
    // }
    //
    // private void WatchRequestNode(RequestNode rn, CompositeDisposable parentDisposable)
    // {
    //     var disposable = new CompositeDisposable().DisposeWith(parentDisposable);
    //     rn.WhenAnyValue(n => n.Name).Subscribe(_ => Save()).DisposeWith(disposable);
    //     rn.WhenAnyValue(n => n.Method).Subscribe(_ => Save()).DisposeWith(disposable);
    //     rn.WhenAnyValue(n => n.Url).Subscribe(_ => Save()).DisposeWith(disposable);
    //     wathables.Add(rn.Id, disposable);
    // }
    //
    //
    // private void Save()
    // {
    //     if (watchedProjects is not null)
    //     {
    //         storageManager.UpdateProjects(watchedProjects);
    //     }
    // }
    //
    // public void Dispose()
    // {
    //     disposables.Dispose();
    // }
}