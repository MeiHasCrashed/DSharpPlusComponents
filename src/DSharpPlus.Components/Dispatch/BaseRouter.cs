using DSharpPlus.Components.Context;
using DSharpPlus.Components.Routing;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.Components.Dispatch;

public abstract class BaseRouter<TRoute, TContext>(ILogger<BaseRouter<TRoute, TContext>> logger) 
    where TRoute : BaseRoute<TContext> where TContext : InteractionBaseContext
{
    protected readonly RouteTree<TRoute> Routes = new();
    
    public void RegisterRoute(string path, TRoute route)
        => Routes.Insert(path, route);

    protected async Task ExecuteAsync(TRoute route, TContext context, List<string> wildcards)
    {
        try
        {
            await route.ExecuteAsync(context, wildcards);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing component route for interaction ID: {InteractionId}", context.Interaction.Id);
        }
    }
    
    internal int RouteCount => Routes.Count;
}