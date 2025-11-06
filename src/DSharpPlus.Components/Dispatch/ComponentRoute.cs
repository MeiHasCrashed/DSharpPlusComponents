using System.Reflection;
using DSharpPlus.Components.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.Components.Dispatch;

public class ComponentRoute
{
    public string RouteId { get; init; }

    private readonly MethodInfo _methodInfo;
    private readonly Type _declaringType;
    private readonly bool _canBeInstantiated;
    private readonly int _wildcardParameterCount;

    private ComponentRoute(string routeId, MethodInfo methodInfo)
    {
        RouteId = routeId;
        _methodInfo = methodInfo;
        _declaringType = methodInfo.DeclaringType ?? throw new InvalidOperationException("Method must have a declaring type.");
        _canBeInstantiated = !_declaringType.IsAbstract || !_declaringType.IsSealed;
        _wildcardParameterCount = methodInfo.GetParameters().Length - 1; // Exclude ComponentContext
    }

    public async Task ExecuteAsync(ComponentContext context, List<string> wildcardValues)
    {
        object? commandObject = null;
        if (_canBeInstantiated)
        {
            commandObject = ActivatorUtilities.CreateInstance(context.ServiceScope.ServiceProvider, _declaringType);
        }

        var values = wildcardValues.ToArray();
        if (values.Length > _wildcardParameterCount)
        {
            var directWildcards = values.AsSpan(0, _wildcardParameterCount - 1);
            var remainingWildcards = values.AsSpan(_wildcardParameterCount - 1);
            var joined = string.Join('-', remainingWildcards!);
            values = [..directWildcards, joined];
        }
        var maybeTask = _methodInfo.Invoke(commandObject, [context, ..values]);
        switch (maybeTask)
        {
            case Task task:
                await task;
                break;
            case ValueTask valueTask:
                await valueTask;
                break;
            default:
                throw new InvalidOperationException("Method did not return Task or ValueTask.");
        }
    }

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