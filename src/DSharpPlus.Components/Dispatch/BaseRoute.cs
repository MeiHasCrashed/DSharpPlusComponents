using System.Reflection;
using DSharpPlus.Components.Attributes;
using DSharpPlus.Components.Context;
using DSharpPlus.Components.IL;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.Components.Dispatch;

public class BaseRoute<TContext> where TContext : InteractionBaseContext
{
    public string RouteId { get; init; }

    private readonly MethodInfo _methodInfo;
    private readonly Type _declaringType;
    private readonly bool _canBeInstantiated;
    private readonly int _wildcardParameterCount;
    private readonly InvocationGenerator.InvokeHandlerDelegate _invokeWrapper;

    protected BaseRoute(string routeId, MethodInfo methodInfo)
    {
        RouteId = routeId;
        _methodInfo = methodInfo;
        _declaringType = methodInfo.DeclaringType ?? throw new InvalidOperationException("Method must have a declaring type.");
        _canBeInstantiated = !_declaringType.IsAbstract || !_declaringType.IsSealed;
        _wildcardParameterCount = methodInfo.GetParameters().Length - 1; // Exclude ComponentContext
        _invokeWrapper = InvocationGenerator.GenerateDelegate(methodInfo);
    }

    public async Task ExecuteAsync(TContext context, List<string> wildcardValues)
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

        await _invokeWrapper(commandObject, [context, ..values]);
    }
}