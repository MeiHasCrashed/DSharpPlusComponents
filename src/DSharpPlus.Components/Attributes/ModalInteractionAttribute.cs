using JetBrains.Annotations;

namespace DSharpPlus.Components.Attributes;

[MeansImplicitUse, PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class ModalInteractionAttribute : Attribute
{
    public string CustomId { get; init; }
    
    public ModalInteractionAttribute(string customId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customId);
        CustomId = customId;
    }
}