using DSharpPlus.EventArgs;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.Components;

[UsedImplicitly]
internal sealed class ComponentsEventHandler(ILogger<ComponentsEventHandler> logger, ComponentsExtension extension) 
    : IEventHandler<ComponentInteractionCreatedEventArgs>, IEventHandler<ModalSubmittedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs eventArgs)
    {
        logger.LogDebug("Received component interaction: {InteractionId} from user {UserTag}", eventArgs.Id, eventArgs.User.Username);
        await extension.ComponentRouter.HandleInteractionAsync(eventArgs);
    }

    public async Task HandleEventAsync(DiscordClient sender, ModalSubmittedEventArgs eventArgs)
    {
        logger.LogDebug("Received modal submission: {InteractionId} from user {UserTag}", eventArgs.Id, eventArgs.Interaction.User.Username);
        await extension.ModalRouter.HandleInteractionAsync(eventArgs);
    }
}