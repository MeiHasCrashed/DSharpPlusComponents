using System.Reflection;
using DSharpPlus.Components.Attributes;
using DSharpPlus.Components.Context;

namespace DSharpPlus.Components.Dispatch;

public class ComponentRoute : BaseRoute<ComponentContext>
{
    private ComponentRoute(string routeId, MethodInfo methodInfo) : base(routeId, methodInfo) { }

    public static ComponentRoute FromMethodInfo(MethodInfo methodInfo)
    {
        var attribute = methodInfo.GetCustomAttribute<ComponentInteractionAttribute>();
        if (attribute == null)
            throw new InvalidOperationException("Method does not have a ComponentInteractionAttribute.");

        if (methodInfo.ReturnType != typeof(Task) && methodInfo.ReturnType != typeof(ValueTask))
            throw new InvalidOperationException("Method must return Task or ValueTask.");

        var parameters = methodInfo.GetParameters();
        if (parameters.Length == 0 || parameters[0].ParameterType != typeof(ComponentContext))
            throw new InvalidOperationException("First parameter must be of type ComponentContext.");
        
        var wildcards = attribute.CustomId.Count(c => c == '*');
        if(wildcards > 0 && parameters.Length < 2)
            throw new InvalidOperationException("At least one parameter must be present after ComponentContext when using wildcards.");
        if(wildcards > parameters.Length - 1)
            throw new InvalidOperationException("Number of parameters must at least match number of wildcards.");

        return new ComponentRoute(attribute.CustomId, methodInfo);
    }
}