using System;
using System.Linq;
using Api.Buddy.Main.Logic.Models.Project;
using Api.Buddy.Main.UI.MVVM;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Api.Buddy.Main.UI.Controls.Project;

public partial class ProjectView : ReactiveUserControl<ProjectViewModel>
{
    public ProjectView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        this.WhenActivated(d =>
        {
            if (ViewModel is not null)
            {
                d(ViewModel.NodeCreated.Subscribe(NodeCreated));
            }
        });
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        var treeView = this.GetControl<TreeView>("ProjectTree");
        if (treeView.Items.Count > 0)
        {
            // TODO: expand last used.
            var item = treeView.TreeContainerFromItem(treeView.Items.First()!);
            if (item is TreeViewItem treeViewItem)
            {
                treeView.ExpandSubTree(treeViewItem);
            }
        }
        base.OnLoaded(e);
    }

    private void NodeCreated(ProjectNode projectNode)
    {
        var treeView = this.GetControl<TreeView>("ProjectTree");
        if (projectNode is { Parent: FolderNode parentNode })
        {
            var item = treeView.TreeContainerFromItem(parentNode);
            if (item is TreeViewItem treeViewItem)
            {
                treeView.ExpandSubTree(treeViewItem);
            }
        }
    }
}
