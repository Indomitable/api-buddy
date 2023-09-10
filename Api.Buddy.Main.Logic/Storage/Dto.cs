using System;
using System.Collections.Generic;
using Api.Buddy.Main.Logic.Models.Request;

namespace Api.Buddy.Main.Logic.Storage;

public record ProjectDto(Guid Id, string Name, IEnumerable<ProjectNodeDto> Nodes);

[System.Text.Json.Serialization.JsonDerivedType(typeof(FolderNodeDto), "folder")]
[System.Text.Json.Serialization.JsonDerivedType(typeof(RequestNodeDto), "request")]
public abstract record ProjectNodeDto(Guid Id, string Name);

public record FolderNodeDto(Guid Id, string Name, IEnumerable<ProjectNodeDto> Children) : ProjectNodeDto(Id, Name);

public record RequestNodeDto(Guid Id, string Name, HttpMethod Method, string Url,
        IEnumerable<IndexedKeyValueDto> QueryParams,
        IEnumerable<IndexedKeyValueDto> Headers)
    : ProjectNodeDto(Id, Name);

public record IndexedKeyValueDto(int Index, string Name, string Value, bool Selected);
