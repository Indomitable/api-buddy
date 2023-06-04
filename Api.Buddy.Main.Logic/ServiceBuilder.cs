using Api.Buddy.Main.Logic.Services;
using Autofac;

namespace Api.Buddy.Main.Logic;

public class ServiceBuilder: Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Mapper>().As<IMapper>().SingleInstance();
        builder.RegisterType<RequestBuilder>().As<IRequestBuilder>().SingleInstance();
        builder.RegisterType<RequestExecutor>().As<IRequestExecutor>().SingleInstance();
    }
}
