using System.Collections.Generic;
using System.Linq;
using Api.Buddy.Main.UI.Models;
using Autofac.Features.Indexed;
using AvaloniaEdit.Document;
using AvaloniaEdit.Folding;

namespace Api.Buddy.Main.UI.Controls.Request.Body;

public interface IFoldingHandlerResolver
{
    IFoldingHandler? Get(TextBodyType bodyType);
}

internal sealed class FoldingHandlerResolver : IFoldingHandlerResolver
{
    private readonly IIndex<TextBodyType, IFoldingHandler> foldingHandlers;

    public FoldingHandlerResolver(IIndex<TextBodyType, IFoldingHandler> foldingHandlers)
    {
        this.foldingHandlers = foldingHandlers;
    }

    public IFoldingHandler? Get(TextBodyType bodyType)
    {
        if (foldingHandlers.TryGetValue(bodyType, out var handler))
        {
            return handler;
        }
        return null;
    }
}

public interface IFoldingHandler
{
    void UpdateFoldings(FoldingManager foldingManager, TextDocument document);
}

internal sealed class JsonFoldingHandler : IFoldingHandler
{
    public void UpdateFoldings(FoldingManager foldingManager, TextDocument document)
    {
        var foldings = CreateFolding(document).OrderBy(x => x.StartOffset);
        foldingManager.UpdateFoldings(foldings, -1);
    }

    private IEnumerable<NewFolding> CreateFolding(TextDocument document)
    {
        var text = document.Text!;
        var objectStarts = new Stack<int>();
        var arrayStarts = new Stack<int>();
        for (var i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '{':
                    objectStarts.Push(i);
                    break;
                case '[':
                    arrayStarts.Push(i);
                    break;
                case '}':
                {
                    yield return new NewFolding(objectStarts.Pop(), i + 1)
                    {
                        Name = "{...}"
                    };
                    break;
                }
                case ']':
                {
                    yield return new NewFolding(arrayStarts.Pop(), i + 1)
                    {
                        Name = "[...]"
                    };
                    break;
                }
            }
        }
    }
}

internal sealed class XmlFoldingHandler : IFoldingHandler
{
    private readonly XmlFoldingStrategy strategy;

    public XmlFoldingHandler()
    {
        strategy = new XmlFoldingStrategy();
    }
    
    public void UpdateFoldings(FoldingManager foldingManager, TextDocument document)
    {
        strategy.UpdateFoldings(foldingManager, document);
    }
}