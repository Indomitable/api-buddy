using System;
using Api.Buddy.Main.UI.Controls;
using Api.Buddy.Main.UI.Controls.Request;
using Api.Buddy.Main.UI.Controls.Request.Body;
using Api.Buddy.Main.UI.Models;
using Api.Buddy.Main.UI.MVVM;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using RequestView = Api.Buddy.Main.UI.Controls.Request.RequestView;

namespace Api.Buddy.Main.UI;

public static class Container
{
    private static IContainer CreateServiceProvider()
    {
        var providerFactory = new AutofacServiceProviderFactory();
        var serviceCollection = new ServiceCollection();
        serviceCollection.BuildServices();
        var builder = providerFactory.CreateBuilder(serviceCollection);
        builder.RegisterServices();
        builder.RegisterViewModels();
        builder.RegisterControls();
        return builder.Build();
    }

    private static void BuildServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient();
    }

    private static void RegisterServices(this ContainerBuilder builder)
    {
        builder.RegisterModule<Logic.ServiceBuilder>();
        builder.RegisterType<FoldingHandlerResolver>()
            .As<IFoldingHandlerResolver>()
            .SingleInstance();
        builder.RegisterType<JsonFoldingHandler>()
            .Keyed<IFoldingHandler>(TextBodyType.Json)
            .SingleInstance();
        builder.RegisterType<XmlFoldingHandler>()
            .Keyed<IFoldingHandler>(TextBodyType.XML)
            .SingleInstance();
        builder.RegisterType<BodyDisplayDataTemplateSelector>().As<IBodyDisplayDataTemplateSelector>().SingleInstance();
    }

    private static void RegisterViewModels(this ContainerBuilder builder)
    {
        builder.RegisterType<MainViewModel>().As<IMainViewModel>().InstancePerDependency();
        builder.RegisterType<ProjectViewModel>().As<IProjectViewModel>().InstancePerDependency();
        builder.RegisterType<RequestViewModelFactory>().As<IRequestViewModelFactory>().InstancePerDependency();
        builder.RegisterType<ResponseViewModel>().As<IResponseViewModel>().InstancePerDependency();
    }

    private static void RegisterControls(this ContainerBuilder builder)
    {
        builder.Register<Func<EnhancedTextBody, RequestBodyEnhancedText>>(c =>
            {
                var resolver = c.Resolve<IFoldingHandlerResolver>();
                return body => new RequestBodyEnhancedText
                {
                    FoldingHandlerResolver = resolver,
                    DataContext = body,
                };
            })
            .AsSelf().InstancePerDependency();
        builder.Register<Func<TextBody, RequestBodyText>>(_ => body => new RequestBodyText { DataContext = body })
            .AsSelf().InstancePerDependency();
    }
    

    public static readonly IContainer Provider = CreateServiceProvider();
}
