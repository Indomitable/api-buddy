using System.Linq;
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
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        var treeView = this.GetControl<TreeView>("ProjectTree");
        var item = (TreeViewItem)treeView.TreeContainerFromItem(treeView.Items.First()!)!;
        treeView.ExpandSubTree(item);
        base.OnLoaded(e);
    }

    private void MenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
    }
}
