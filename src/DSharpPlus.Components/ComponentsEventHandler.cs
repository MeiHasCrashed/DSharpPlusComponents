using DSharpPlus.EventArgs;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.Components;

[UsedImplicitly]
internal sealed class ComponentsEventHandler(ILogger<ComponentsEventHandler> logger, ComponentsExtension extension) 
    : IEventHandler<ComponentInteractionCreatedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs eventArgs)
    {
        logger.LogDebug("Received component interaction: {InteractionId} from user {UserTag}", eventArgs.Id, eventArgs.User.Username);
        await extension.HandleInteractionAsync(eventArgs);
    }
}