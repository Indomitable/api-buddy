using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Api.Buddy.Main.Logic.Models.Request;
using ReactiveUI;

namespace Api.Buddy.Main.Logic.Models.Project;

public sealed class FolderNode: ProjectNode
{
    public FolderNode()
    {
        NodeType = NodeType.Folder;
        CreateFolderCommand = ReactiveCommand.Create<string>(OnCreateFolder);
        CreateRequestCommand = ReactiveCommand.Create<string>(OnCreateRequest);
    }

    public ICommand CreateFolderCommand { get; }
    
    public ICommand CreateRequestCommand { get; }

    private void OnCreateFolder(string value)
    {
        var child = new FolderNode { Name = value, Parent = this, };
        Children.Insert(GetIndex(child), child);
    }
    
    private void OnCreateRequest(string value)
    {
        var child = new RequestNode
        {
            Name = value, 
            Parent = this,
            Request = RequestInit.Empty with
            {
                Method = TryGuessMethod(value)
            }
        };
        Children.Insert(GetIndex(child), child);
    }

    /// <summary>
    /// Get insert index so the elements to be sorted by  name
    /// </summary>
    private int GetIndex<T>(T child)
        where T: ProjectNode
    {
        int index = 0;
        using var enumerator = Children.GetEnumerator();
        while (enumerator.MoveNext() 
               && enumerator.Current is T n
               && string.CompareOrdinal(n.Name, child.Name) < 0)
        {
            index++;
        }
        return index;
    }

    private HttpMethod TryGuessMethod(string name)
    {
        var dictionary = new Dictionary<HttpMethod, List<string>>
        {
            [HttpMethod.POST] = new(){ "Create", "Insert" },
            [HttpMethod.PUT] = new(){ "Update" },
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
}
