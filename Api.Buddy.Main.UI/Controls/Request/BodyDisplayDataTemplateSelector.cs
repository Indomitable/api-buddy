using System;
using Api.Buddy.Main.UI.Controls.Request.Body;
using Api.Buddy.Main.UI.Models;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Api.Buddy.Main.UI.Controls.Request;

public static class BodyDisplayDataTemplateSelector
{
    public static FuncDataTemplate<IBody<object>?> BodyTemplate = new(
        bodyType => bodyType is not null,
        bodyType => bodyType switch
        {
            TextBody => new RequestBodyText { DataContext = bodyType },
            EmptyBody => new TextBlock { Text = string.Empty },
            _ => new TextBlock { Text = "Body not supported!" }
        }
    );
}
