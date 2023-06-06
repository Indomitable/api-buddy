using Api.Buddy.Main.UI.Controls;
using Api.Buddy.Main.UI.MVVM;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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
        builder.BuildAutofac();
        return builder.Build();
    }

    private static void BuildServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient();
    }

    private static void BuildAutofac(this ContainerBuilder builder)
    {
        builder.RegisterModule<Logic.ServiceBuilder>();

        builder.RegisterType<RequestViewModelFactory>().As<IRequestViewModelFactory>().SingleInstance();
        builder.RegisterType<MainViewModel>().As<IMainViewModel>().SingleInstance();
        builder.RegisterType<RequestView>().SingleInstance();

        builder.RegisterType<ProjectViewModel>().As<IProjectViewModel>().SingleInstance();
    }

    public static readonly IContainer Provider = CreateServiceProvider();
}
