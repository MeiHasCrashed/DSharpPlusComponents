using DSharpPlus.Components.Routing;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.Components.Handling;

public class ComponentRouter(ILogger<ComponentRouter> logger, IServiceProvider serviceProvider)
{
    private readonly RouteTree<ComponentRoute> _routes = new();
    
    public void RegisterRoute(string path, ComponentRoute route)
        => _routes.Insert(path, route);

    public async Task HandleInteractionAsync(ComponentInteractionCreatedEventArgs args)
    {
        var path = args.Id;
        var matchResult = _routes.Match(path);
        if (!matchResult.IsMatch)
        {
            logger.LogDebug("No route matched for component interaction with ID: {InteractionId}", args.Id);
            return;
        }
        await using var scope = serviceProvider.CreateAsyncScope();
        var route = matchResult.Value!;
        var context = new ComponentContext
        {
            ServiceScope = scope,
            Channel = args.Channel,
            Interaction = args.Interaction,
            User = args.User
        };

        try
        {
            await route.ExecuteAsync(context, matchResult.Wildcards);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing component route for interaction ID: {InteractionId}", args.Id);
        }
    }
}