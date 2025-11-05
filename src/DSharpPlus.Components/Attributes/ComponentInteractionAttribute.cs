using JetBrains.Annotations;

namespace DSharpPlus.Components.Attributes;

[MeansImplicitUse, PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class ComponentInteractionAttribute : Attribute
{
    public string CustomId { get; }

    public ComponentInteractionAttribute(string customId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customId);
        CustomId = customId;
    }
}