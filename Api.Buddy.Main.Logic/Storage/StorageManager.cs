using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Models.Request;

namespace Api.Buddy.Main.Logic.Storage;

public sealed record Storage(IEnumerable<Project> Projects, Guid? SelectedProject);

public interface IStorageManager
{
    void Save(Storage storage);
    Storage Load();
}

internal sealed class StorageManager : IStorageManager
{
    private readonly string folderName;
    private readonly string projectSave;
    private readonly JsonSerializerOptions options;
    
    sealed class StorageDto
    {
        public required IEnumerable<ProjectDto> Projects { get; set; }
        public Guid? SelectedProject { get; set; }
    }

    public StorageManager()
    {
        folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "api-buddy");
        if (!Directory.Exists(folderName))
        {
            Directory.CreateDirectory(folderName);
        }
        projectSave = Path.Combine(folderName, "projects.json");
        options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(),
            }
        };
    }

   
    // public void UpdateRequest(RequestNode request)
    // {
    //     var parents = new Stack<string>();
    //     for (var parent = request.Parent; parent != null; parent = parent.Parent)
    //     {
    //         parents.Push(parent.Name);
    //     }
    //     var dto = Read();
    //     var project = dto.Projects.FirstOrDefault(p => p.Id == request.Project.Id) 
    //                   ?? new ProjectDto { Id = request.Project.Id, Name = "Unknown project", Nodes = new List<ProjectNodeDto>() };
    //     var nodes = project.Nodes;
    //     while (parents.Count > 0)
    //     {
    //         var parent = parents.Pop();
    //         var parentNode = nodes.OfType<FolderNodeDto>().FirstOrDefault(n => n.Name == parent);
    //         if (parentNode is null)
    //         {
    //             parentNode = new FolderNodeDto
    //             {
    //                 Name = parent,
    //                 Children = new List<ProjectNodeDto>()
    //             };
    //             nodes.Add(parentNode);
    //         }
    //         nodes = parentNode.Children;
    //     }
    //     var found = nodes.OfType<RequestNodeDto>().FirstOrDefault(s => string.Equals(s.Name, request.Name, StringComparison.Ordinal));
    //     if (found is null)
    //     {
    //         nodes.Add(ToDto(request));
    //     }
    //     else
    //     {
    //         found.Name = request.Name;
    //         found.Url = request.Url;
    //         found.Method = request.Method;
    //         found.QueryParams = request.QueryParams
    //             .Select(p => new IndexedKeyValueDto(p.Index, p.Name, p.Value, p.Selected)).ToList();
    //         found.Headers = request.Headers.Select(p => new IndexedKeyValueDto(p.Index, p.Name, p.Value, p.Selected))
    //             .ToList();
    //     }
    //     Write(dto);
    // }

    public void Save(Storage storage)
    {
        var dto = new StorageDto { Projects = ToDto(storage.Projects), SelectedProject = storage.SelectedProject };
        Write(dto);
    }
    
    public Storage Load()
    {
        var dto = Read();
        return new Storage(ToDomain(dto?.Projects), dto?.SelectedProject);
    }

    private StorageDto Read()
    {
        try
        {
            using var file = File.OpenRead(projectSave);
            return JsonSerializer.Deserialize<StorageDto>(file, options) 
                   ?? new StorageDto { Projects = Enumerable.Empty<ProjectDto>() };
        }
        catch
        {
            // no file found.
            Console.WriteLine($"Unable to load projects from {projectSave}");
            return new StorageDto { Projects = Enumerable.Empty<ProjectDto>() };
        }
    }
    
    private void Write(StorageDto storageDto)
    {
        try
        {
            var projectsJson = JsonSerializer.Serialize(storageDto, options);
            File.WriteAllText(projectSave, projectsJson, Encoding.UTF8);
        }
        catch
        {
            Console.WriteLine($"Unable to save projects to {projectSave}");
        }
    }

    private static IEnumerable<ProjectDto> ToDto(IEnumerable<Project> projects)
    {
        return projects.Select(p => new ProjectDto(p.Id, p.Name, ToDto(p.Nodes)));
    }
    
    private static IEnumerable<ProjectNodeDto> ToDto(IEnumerable<ProjectNode> nodes)
    {
        foreach (var node in nodes)
        {
            yield return node switch
            {
                FolderNode fn => new FolderNodeDto(fn.Id, fn.Name, ToDto(fn.Children)),
                RequestNode rn => ToDto(rn),
                _ => throw new ArgumentOutOfRangeException(nameof(node))
            };
        }
    }

    private static RequestNodeDto ToDto(RequestNode rn)
    {
        return new RequestNodeDto(rn.Id, rn.Name, rn.Method, rn.Url, 
            rn.QueryParams.Select(p => new IndexedKeyValueDto(p.Index, p.Name, p.Value, p.Selected)),
            rn.Headers.Select(p => new IndexedKeyValueDto(p.Index, p.Name, p.Value, p.Selected))
        );
    }
    
    private IEnumerable<Project> ToDomain(IEnumerable<ProjectDto>? dtoProjects)
    {
        if (dtoProjects is null)
        {
            yield break;
        }

        foreach (var dp in dtoProjects)
        {
            var project = new Project(dp.Id, dp.Name);
            project.Nodes.AddRange(ToDomain(project, dp.Nodes, null));
            yield return project;
        }
    }
    
    private static IEnumerable<ProjectNode> ToDomain(Project project, IEnumerable<ProjectNodeDto> dtoNodes, ProjectNode? parent)
    {
        foreach (var dn in dtoNodes)
        {
            switch (dn)
            {
                case FolderNodeDto fn:
                    var folderNode = new FolderNode(fn.Id, fn.Name, project, parent);
                    folderNode.Children.AddRange(ToDomain(project, fn.Children, folderNode));
                    yield return folderNode;
                    break;
                case RequestNodeDto rn:
                    var requestNode = new RequestNode(rn.Id, rn.Name, project, parent)
                    {
                        Method = rn.Method,
                        Url = rn.Url
                    };
                    requestNode.QueryParams.AddRange(rn.QueryParams.Select(qp => new QueryParam(qp.Index, qp.Name, qp.Value, qp.Selected)));
                    requestNode.Headers.AddRange(rn.Headers.Select(h => new Header
                    {
                        Index = h.Index,
                        Name = h.Name,
                        Value = h.Value,
                        Selected = h.Selected
                    }));
                    yield return requestNode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dn));
            }
        }
    }
}