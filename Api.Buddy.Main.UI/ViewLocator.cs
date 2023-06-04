using System.Reflection;
using Autofac;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;

namespace Api.Buddy.Main.UI;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        var controlAttribute = data!.GetType().GetCustomAttribute<ControlAttribute>()!;
        return (Control)Container.Provider.Resolve(controlAttribute.Control);
    }

    public bool Match(object? data)
    {
        return data is ReactiveObject
            && data.GetType().GetCustomAttribute<ControlAttribute>() != null;
    }
}
