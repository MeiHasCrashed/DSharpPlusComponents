using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.Components;

public class ComponentContext
{
    public required IServiceScope ServiceScope { get; init; }
    
    public required DiscordInteraction Interaction { get; init; }
    
    public required DiscordUser User { get; init; }
    
    public required DiscordChannel Channel { get; init; }
}