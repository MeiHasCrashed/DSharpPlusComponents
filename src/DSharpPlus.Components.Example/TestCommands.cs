using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Components.Attributes;
using DSharpPlus.Entities;

namespace DSharpPlus.Components.Example;

public class TestCommands
{
    [Command("button")]
    public async Task ButtonCommand(TextCommandContext ctx)
    {
        var builder = new DiscordMessageBuilder()
            .WithContent("Example button message:")
            .AddActionRowComponent(new DiscordButtonComponent(DiscordButtonStyle.Primary, "button-test-12345-34as", "Click Me!"));
        await ctx.Channel.SendMessageAsync(builder);
    }

    [ComponentInteraction("button-test-*")]
    public async Task ButtonTestHandler(ComponentContext ctx, string wildcard)
    {
        var builder = new DiscordInteractionResponseBuilder()
            .WithContent("Button clicked! Wildcard: " + wildcard)
            .AsEphemeral();
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }

    [ComponentInteraction("button-test-12345-34as")]
    public async Task SpecificButtonTestHandler(ComponentContext ctx)
    {
        var builder = new DiscordInteractionResponseBuilder()
            .WithContent("Specific button clicked!")
            .AsEphemeral();
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }
}