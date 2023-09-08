using System;
using System.Collections.Generic;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.Logic.Models.Request;

namespace Api.Buddy.Main.Logic.Storage;

public record ProjectDto(Guid Id, string Name, IEnumerable<ProjectNodeDto> Nodes);

[System.Text.Json.Serialization.JsonDerivedType(typeof(FolderNodeDto), "folder")]
[System.Text.Json.Serialization.JsonDerivedType(typeof(RequestNodeDto), "request")]
public record ProjectNodeDto(string Name, NodeType NodeType, IEnumerable<ProjectNodeDto> Children);


public record FolderNodeDto(string Name, IEnumerable<ProjectNodeDto> Children)
    : ProjectNodeDto(Name, NodeType.Folder, Children);
public record RequestNodeDto(string Name, RequestDto Request)
    : ProjectNodeDto(Name, NodeType.Request, Array.Empty<ProjectNodeDto>());


public record RequestDto(HttpMethod Method, string Url, IEnumerable<IndexedKeyValueDto> QueryParams, IEnumerable<IndexedKeyValueDto> Headers);

public record IndexedKeyValueDto(int Index, string Name, string Value, bool Selected);
