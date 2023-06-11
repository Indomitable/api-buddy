using System;
using Api.Buddy.Main.UI.Controls.Request.Body;
using Api.Buddy.Main.UI.Models;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Api.Buddy.Main.UI.Controls.Request;

public interface IBodyDisplayDataTemplateSelector
{
    FuncDataTemplate<IBody<object>?> BodyTemplate { get; }
}

public class BodyDisplayDataTemplateSelector : IBodyDisplayDataTemplateSelector
{
    public BodyDisplayDataTemplateSelector(Func<EnhancedTextBody, RequestBodyEnhancedText> enhancedBodyCreator,
        Func<TextBody, RequestBodyText> textBodyCreator)
    {
        BodyTemplate = new(
            body => body is not null,
            body => body switch
            {
                EnhancedTextBody eb => enhancedBodyCreator(eb),
                TextBody tb => textBodyCreator(tb),
                EmptyBody => new TextBlock { Text = string.Empty },
                _ => new TextBlock { Text = "Body not supported!" }
            }
        );   
    }
    
    public FuncDataTemplate<IBody<object>?> BodyTemplate { get; }
}
