using DSharpPlus.Components.Context;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.Components.Dispatch;

public class ModalRouter(ILogger<ModalRouter> logger, IServiceProvider serviceProvider) : BaseRouter<ModalRoute, ModalContext>(logger)
{
    public async Task HandleInteractionAsync(ModalSubmittedEventArgs args)
    {
        var path = args.Id;
        var matchResult = Routes.Match(path);
        if (!matchResult.IsMatch)
        {
            logger.LogDebug("No route matched for modal interaction with ID: {InteractionId}", args.Id);
            return;
        }
        await using var scope = serviceProvider.CreateAsyncScope();
        var route = matchResult.Value!;
        var context = new ModalContext
        {
            ServiceScope = scope,
            Channel = args.Interaction.Channel,
            Interaction = args.Interaction,
            User = args.Interaction.User,
            Values = args.Values
        };
        await ExecuteAsync(route, context, matchResult.Wildcards);
    }
}