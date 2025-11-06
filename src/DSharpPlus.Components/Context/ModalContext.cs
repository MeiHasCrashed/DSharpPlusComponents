using DSharpPlus.EventArgs;

namespace DSharpPlus.Components.Context;

public class ModalContext : InteractionBaseContext
{
    public required IReadOnlyDictionary<string, IModalSubmission> Values { get; init; }
}