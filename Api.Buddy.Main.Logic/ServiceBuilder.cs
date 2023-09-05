using Api.Buddy.Main.Logic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Buddy.Main.Logic;

public static class ServiceBuilder
{
    public static IServiceCollection AddLogic(this IServiceCollection services)
    {
        services.AddSingleton<IMapper, Mapper>();
        services.AddSingleton<IRequestBuilder, RequestBuilder>();
        services.AddSingleton<IRequestExecutor, RequestExecutor>();
        return services;
    }
}
