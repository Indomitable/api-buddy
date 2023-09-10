using Api.Buddy.Main.Logic.Models.Project;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace Api.Buddy.Main.UI.Controls.Project;

public class ProjectNodeDataTemplateSelector: IDataTemplate
{
    public Control? Build(object? param)
    {
        switch (param)
        {
            case FolderNode folderNode:
                return CreateFolderNode(folderNode);
            case RequestNode requestNode:
                return CreateRequestNode(requestNode);
            default:
                return null;
        }
    }

    public bool Match(object? data)
    {
        return data is ProjectNode;
    }
    //
    // public InstancedBinding? ItemsSelector(object item)
    // {
    //     var projectNode = (ProjectNode)item;
    //     var subject = new Subject<ProjectNode>();
    //     projectNode.Children.CollectionChanged += (sender, args) =>
    //     {
    //         foreach (var child in projectNode.Children)
    //         {
    //             subject.OnNext(child);
    //         }
    //     };
    //     return InstancedBinding.OneWay(subject, BindingPriority.Style);
    // }
    //
    private Control? CreateRequestNode(RequestNode requestNode)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                new TextBlock { Text = requestNode.Method.ToString() },
                new TextBlock { Text = requestNode.Name }
            }
        };
    }

    private Control? CreateFolderNode(FolderNode folderNode)
    {
        return new TextBlock
        {
            Text = folderNode.Name
        };
    }

}
