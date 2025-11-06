using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.Components.Context;

public class InteractionBaseContext
{
    public required IServiceScope ServiceScope { get; init; }
    
    public required DiscordInteraction Interaction { get; init; }
    
    public required DiscordUser User { get; init; }
    
    public required DiscordChannel Channel { get; init; }
}