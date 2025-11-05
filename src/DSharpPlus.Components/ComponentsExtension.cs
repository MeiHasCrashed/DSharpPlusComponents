using System.Reflection;
using DSharpPlus.Components.Attributes;
using DSharpPlus.Components.Handling;
using DSharpPlus.Components.Util;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.Components;

public class ComponentsExtension
{
    private readonly ILogger<ComponentsExtension> _logger;
    private readonly DiscordClient _client;
    private readonly ComponentRouter _router;
    
    public ComponentsExtension(ILogger<ComponentsExtension> logger, DiscordClient client, ComponentRouter router)
    {
        _logger = logger;
        _client = client;
        _router = router;
    }

    public void AddComponents(Assembly assembly)
    {
        foreach (var methodInfo in
                 ReflectionUtil.ScanAssemblyForAttributedMethods<ComponentInteractionAttribute>(assembly))
        {
            try
            {
                var route = ComponentRoute.FromMethodInfo(methodInfo);
                _router.RegisterRoute(route.RouteId, route);
                _logger.LogDebug("Registered component interaction method: {MethodName} with Route ID: {RouteId}", 
                    methodInfo.Name, route.RouteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register component interaction method: {MethodName}", methodInfo.Name);
            }
        }
    }
    
    internal async Task HandleInteractionAsync(ComponentInteractionCreatedEventArgs args)
    {
        await _router.HandleInteractionAsync(args);
    }
}