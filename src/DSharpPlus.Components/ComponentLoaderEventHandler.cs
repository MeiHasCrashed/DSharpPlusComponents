using DSharpPlus.EventArgs;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.Components;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class ComponentLoaderEventHandler(ILogger<ComponentLoaderEventHandler> logger, ComponentsExtension components) : IEventHandler<ClientStartedEventArgs>
{
    public Task HandleEventAsync(DiscordClient sender, ClientStartedEventArgs eventArgs)
    {
        logger.LogDebug("Loaded {} component interaction routes.", components.Router.RouteCount);
        return Task.CompletedTask;
    }
}