using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Components.Attributes;
using DSharpPlus.Components.Context;
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

    [Command("modal")]
    public async Task ModalCommand(TextCommandContext ctx)
    {
        var builder = new DiscordMessageBuilder()
            .WithContent("Modal message")
            .AddActionRowComponent(new DiscordButtonComponent(DiscordButtonStyle.Primary, "button-modal-a", "Open Modal"));
        await ctx.RespondAsync(builder);
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

    [ComponentInteraction("button-modal-a")]
    public async Task ButtonModalHandler(ComponentContext ctx)
    {
        var modalBuilder = new DiscordModalBuilder()
            .WithTitle("Test Modal")
            .WithCustomId("modal-test-42")
            .AddTextDisplay("Hello World !");
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, modalBuilder);
    }

    [ModalInteraction("modal-test-*")]
    public async Task ModalTestHandler(ModalContext ctx, string wildcard)
    {
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Thank you!").AsEphemeral());
    }
}