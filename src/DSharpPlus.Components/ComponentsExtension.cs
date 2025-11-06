using System.Reflection;
using DSharpPlus.Components.Attributes;
using DSharpPlus.Components.Dispatch;
using DSharpPlus.Components.Util;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.Components;

public class ComponentsExtension
{
    private readonly ILogger<ComponentsExtension> _logger;
    private readonly DiscordClient _client;
    
    internal ComponentRouter ComponentRouter { get; }
    
    internal ModalRouter ModalRouter { get; }

    public ComponentsExtension(ILogger<ComponentsExtension> logger, DiscordClient client, ComponentRouter componentRouter, ModalRouter modalRouter)
    {
        _logger = logger;
        _client = client;
        ComponentRouter = componentRouter;
        ModalRouter = modalRouter;
    }

    public void AddComponents(Assembly assembly)
    {
        foreach (var methodInfo in
                 ReflectionUtil.ScanAssemblyForAttributedMethods<ComponentInteractionAttribute>(assembly))
        {
            try
            {
                var route = ComponentRoute.FromMethodInfo(methodInfo);
                ComponentRouter.RegisterRoute(route.RouteId, route);
                _logger.LogDebug("Registered component interaction. Method {MethodName} with Route ID: {RouteId}", 
                    methodInfo.Name, route.RouteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register component interaction. Method: {MethodName}", methodInfo.Name);
            }
        }
    }

    public void AddModals(Assembly assembly)
    {
        foreach (var methodInfo in
                 ReflectionUtil.ScanAssemblyForAttributedMethods<ModalInteractionAttribute>(assembly))
        {
            try
            {
                var route = ModalRoute.FromMethodInfo(methodInfo);
                ModalRouter.RegisterRoute(route.RouteId, route);
                _logger.LogDebug("Registered modal interaction. Method {MethodName} with Route ID: {RouteId}", 
                    methodInfo.Name, route.RouteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register component interaction. Method: {MethodName}", methodInfo.Name);
            }
        }
    }
    
    public void AddInteractions(Assembly assembly)
    {
        AddComponents(assembly);
        AddModals(assembly);
    }
}