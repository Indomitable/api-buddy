using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Api.Buddy.Main.UI.Controls.Project;

public partial class ProjectView : UserControl
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
}
