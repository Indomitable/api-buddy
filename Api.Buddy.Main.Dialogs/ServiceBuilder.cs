using Api.Buddy.Main.Dialogs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Buddy.Main.Dialogs;

public static class ServiceBuilder
{
    public static IServiceCollection AddDialogs(this IServiceCollection collection)
    {
        collection.AddSingleton<ITextInputDialogService, TextInputDialogService>();
        return collection;
    }
}
