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
    private readonly string projectSave;
    private readonly JsonSerializerOptions options;
    
    sealed class StorageDto
    {
        public required IEnumerable<ProjectDto> Projects { get; set; }
        public Guid? SelectedProject { get; set; }
    }

    public StorageManager()
    {
        var folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "api-buddy");
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

    public void Save(Storage storage)
    {
        var dto = new StorageDto { Projects = ToDto(storage.Projects), SelectedProject = storage.SelectedProject };
        Write(dto);
    }
    
    public Storage Load()
    {
        var dto = Read();
        return new Storage(ToDomain(dto.Projects), dto?.SelectedProject);
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
                    requestNode.Headers.AddRange(rn.Headers.Select(h => new Header(h.Index, h.Name, h.Value, h.Selected)));
                    yield return requestNode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dn));
            }
        }
    }
}