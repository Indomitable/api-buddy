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
    void Save(IEnumerable<Project> projects, Project? selectedProject);
    Storage Load();
}

internal sealed class StorageManager : IStorageManager
{
    private readonly string folderName;
    private readonly string projectSave;
    private readonly JsonSerializerOptions options;
    
    sealed record StorageDto(IEnumerable<ProjectDto> Projects, Guid? SelectedProject);

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

    public void Save(IEnumerable<Project> projects, Project? selectedProject)
    {
        try
        {
            var dto = ToDto(projects);
            var storage = new StorageDto(dto, selectedProject?.Id);
            var projectsJson = JsonSerializer.Serialize(storage, options);
            File.WriteAllText(projectSave, projectsJson, Encoding.UTF8);
        }
        catch
        {
            Console.WriteLine($"Unable to save projects to {projectSave}");
        }
    }

    public Storage Load()
    {
        try
        {
            using var file = File.OpenRead(projectSave);
            var dto = JsonSerializer.Deserialize<StorageDto>(file, options);
            return new Storage(ToDomain(dto?.Projects), dto?.SelectedProject);
        }
        catch
        {
            // no file found.
            Console.WriteLine($"Unable to load projects from {projectSave}");
            return new Storage(Enumerable.Empty<Project>(), null);
        }
    }

    private static IEnumerable<ProjectDto> ToDto(IEnumerable<Project> projects)
    {
        return projects.Select(project => new ProjectDto(project.Id, project.Name, ToDto(project.Nodes)));
    }
    
    private static IEnumerable<ProjectNodeDto> ToDto(IEnumerable<ProjectNode> nodes)
    {
        foreach (var node in nodes)
        {
            yield return node switch
            {
                FolderNode fn => new FolderNodeDto(fn.Name, ToDto(fn.Children)),
                RequestNode rn => new RequestNodeDto(rn.Name,
                    new RequestDto(rn.Request.Method, rn.Request.Url,
                        rn.Request.QueryParams.Select(p =>
                            new IndexedKeyValueDto(p.Index, p.Name, p.Value, p.Selected)),
                        rn.Request.Headers.Select(p => new IndexedKeyValueDto(p.Index, p.Name, p.Value, p.Selected)))),
                _ => throw new ArgumentOutOfRangeException(nameof(node))
            };
        }
    }
    
    private IEnumerable<Project> ToDomain(IEnumerable<ProjectDto>? dtoProjects)
    {
        if (dtoProjects is null)
        {
            yield break;
        }

        foreach (var dp in dtoProjects)
        {
            var project = new Project
            {
                Id = dp.Id,
                Name = dp.Name
            };
            project.Nodes.AddRange(ToDomain(dp.Nodes, null));
            yield return project;
        }
    }
    
    private static IEnumerable<ProjectNode> ToDomain(IEnumerable<ProjectNodeDto> dtoNodes, ProjectNode? parent)
    {
        foreach (var dn in dtoNodes)
        {
            switch (dn)
            {
                case FolderNodeDto fn:
                    var folderNode = new FolderNode { Name = fn.Name };
                    folderNode.Children.AddRange(ToDomain(fn.Children, folderNode));
                    folderNode.Parent = parent;
                    yield return folderNode;
                    break;
                case RequestNodeDto rn:
                    var requestNode = new RequestNode
                    {
                        Name = rn.Name,
                        Parent = parent,
                        Request = new RequestInit(rn.Request.Method, rn.Request.Url,
                            rn.Request.QueryParams.Select(qp => new QueryParam(qp.Index, qp.Name, qp.Value, qp.Selected)).ToList(),
                            rn.Request.Headers.Select(h => new Header
                            {
                                Index = h.Index,
                                Name = h.Name,
                                Value = h.Value,
                                Selected = h.Selected
                            }).ToList())
                    };
                    yield return requestNode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dn));
            }
        }
    }
}