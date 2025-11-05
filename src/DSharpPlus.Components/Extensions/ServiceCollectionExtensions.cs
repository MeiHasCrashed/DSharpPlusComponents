using DSharpPlus.Components.Handling;
using DSharpPlus.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.Components.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddComponentsExtension(this IServiceCollection services, Action<ComponentsExtension>? configure = null)
    {
        services
            .AddSingleton<ComponentRouter>()
            .AddSingleton(provider =>
            {
                var extension = ActivatorUtilities.CreateInstance<ComponentsExtension>(provider);
                configure?.Invoke(extension);
                return extension;
            })
            .ConfigureEventHandlers(b => b.AddEventHandlers<ComponentsEventHandler>());
        return services;
    }
}