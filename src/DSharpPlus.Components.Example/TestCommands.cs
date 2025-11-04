using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;

namespace DSharpPlus.Components.Example;

public class TestCommands
{
    [Command("button")]
    public async Task ButtonCommand(TextCommandContext ctx)
    {
        var builder = new DiscordMessageBuilder()
            .WithContent("Example button message:")
            .AddActionRowComponent(new DiscordButtonComponent(DiscordButtonStyle.Primary, "example_button-245a-48mf", "Click Me!"));
        await ctx.Channel.SendMessageAsync(builder);
    }
}