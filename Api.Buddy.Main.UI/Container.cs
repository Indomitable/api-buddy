using System;
using Api.Buddy.Main.Logic;
using Api.Buddy.Main.Dialogs;
using Api.Buddy.Main.UI.Controls.Request;
using Api.Buddy.Main.UI.Controls.Request.Body;
using Api.Buddy.Main.UI.Models;
using Api.Buddy.Main.UI.MVVM;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Buddy.Main.UI;

public static class Container
{
    private static IServiceProvider? provider = null;

    static Container()
    {
        Services = new ServiceCollection()
            .AddHttpClient()
            .AddLogic()
            .AddServices()
            .AddViewModels()
            .AddControls()
            .AddDialogs();
    }

    private static IServiceCollection AddViewModels(this IServiceCollection collection)
    {
        collection.AddTransient<IMainViewModel, MainViewModel>();
        collection.AddTransient<IProjectViewModel, ProjectViewModel>();
        collection.AddTransient<IRequestViewModelFactory, RequestViewModelFactory>();
        collection.AddTransient<IResponseViewModel, ResponseViewModel>();
        return collection;
    }

    private static IServiceCollection AddServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IFoldingHandlerResolver, FoldingHandlerResolver>();
        collection.AddSingleton<Func<TextBodyType, IFoldingHandler?>>(bodyType => bodyType switch
        {
            TextBodyType.Json => new JsonFoldingHandler(),
            TextBodyType.XML => new XmlFoldingHandler(),
            _ => null
        });
        collection.AddSingleton<IBodyDisplayDataTemplateSelector, BodyDisplayDataTemplateSelector>();
        return collection;
    }

    private static IServiceCollection AddControls(this IServiceCollection collection)
    {
        collection.AddTransient<Func<EnhancedTextBody, RequestBodyEnhancedText>>(c =>
            {
                var resolver = c.GetRequiredService<IFoldingHandlerResolver>();
                return body => new RequestBodyEnhancedText
                {
                    FoldingHandlerResolver = resolver,
                    DataContext = body,
                };
            });
        collection.AddTransient<Func<TextBody, RequestBodyText>>(_ => body => new RequestBodyText { DataContext = body });
        return collection;
    }

    public static IServiceProvider Provider => provider ??= Services.BuildServiceProvider();
    public static IServiceCollection Services { get; }
}
