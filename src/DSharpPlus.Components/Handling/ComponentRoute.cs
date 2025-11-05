using System.Reflection;
using DSharpPlus.Components.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.Components.Handling;

public class ComponentRoute
{
    public string RouteId { get; init; }

    private readonly MethodInfo _methodInfo;
    private readonly Type _declaringType;
    private readonly bool _canBeInstantiated;
    private readonly bool _isValueTask;

    private ComponentRoute(string routeId, MethodInfo methodInfo)
    {
        RouteId = routeId;
        _methodInfo = methodInfo;
        _declaringType = methodInfo.DeclaringType ?? throw new InvalidOperationException("Method must have a declaring type.");
        _canBeInstantiated = !_declaringType.IsAbstract || !_declaringType.IsSealed;
    }

    public async Task ExecuteAsync(ComponentContext context, List<string> wildcardValues)
    {
        object? commandObject = null;
        if (_canBeInstantiated)
        {
            commandObject = ActivatorUtilities.CreateInstance(context.ServiceScope.ServiceProvider, _declaringType);
        }

        var values = wildcardValues.ToArray();
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
        
        if(attribute.CustomId.Count(c => c == '*') != parameters.Length - 1)
            throw new InvalidOperationException("Number of wildcards must match number of parameters after ComponentContext.");

        return new ComponentRoute(attribute.CustomId, methodInfo);
    }
}