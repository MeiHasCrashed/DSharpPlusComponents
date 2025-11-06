using System.Reflection;
using System.Reflection.Emit;

namespace DSharpPlus.Components.IL;

// Mostly taken from https://github.com/DSharpPlus/DSharpPlus/blob/a162daeebf91f3ec70f65d399d95bceb9d77f326/DSharpPlus.Commands/Invocation/CommandEmitUtil.cs
// licensed under MIT License (https://github.com/DSharpPlus/DSharpPlus/blob/a162daeebf91f3ec70f65d399d95bceb9d77f326/LICENSE)
// Thank you !
public static class InvocationGenerator
{
    public delegate ValueTask InvokeHandlerDelegate(object? instance, object[] parameters);

    public static InvokeHandlerDelegate GenerateDelegate(MethodInfo methodInfo)
    {
        if (methodInfo.ReturnType == typeof(ValueTask))
        {
             return GenerateValueTaskDelegate(methodInfo);
        }
        if (methodInfo.ReturnType == typeof(Task))
        {
             return GenerateTaskDelegate(methodInfo);
        }
        
        throw new InvalidOperationException("Method must return Task or ValueTask.");
    }
    
    private static InvokeHandlerDelegate GenerateValueTaskDelegate(MethodInfo methodInfo)
    {
        var method = new DynamicMethod($"{methodInfo.Name}-ValueTask-Wrapper", typeof(ValueTask),
            [typeof(object), typeof(object[])]);
        EmitMethodWrapper(method.GetILGenerator(), methodInfo);

        var wrapper = method.CreateDelegate<Func<object?, object[], ValueTask>>();
        var parameterCount = methodInfo.GetParameters().Length;

        return async (instance, parameters) =>
        {
            ArgumentNullException.ThrowIfNull(parameters);
            if (parameters.Length != parameterCount)
            {
                throw new TargetParameterCountException("Incorrect number of parameters supplied. " +
                                                        $"Got {parameters.Length}, expected {parameterCount}.");
            }

            await wrapper(instance, parameters);
        };
    }
    
    private static InvokeHandlerDelegate GenerateTaskDelegate(MethodInfo methodInfo)
    {
        var method = new DynamicMethod($"{methodInfo.Name}-Task-Wrapper", typeof(Task),
            [typeof(object), typeof(object[])]);
        EmitMethodWrapper(method.GetILGenerator(), methodInfo);

        var wrapper = method.CreateDelegate<Func<object?, object[], Task>>();
        var parameterCount = methodInfo.GetParameters().Length;

        return async (instance, parameters) =>
        {
            ArgumentNullException.ThrowIfNull(parameters);
            if (parameters.Length != parameterCount)
            {
                throw new TargetParameterCountException("Incorrect number of parameters supplied. " +
                                                        $"Got {parameters.Length}, expected {parameterCount}.");
            }

            await wrapper(instance, parameters);
        };
    }

    private static void EmitMethodWrapper(ILGenerator generator, MethodInfo methodInfo)
    {
        // If the method is not static, load the instance (this) on arg 0 to the evaluation stack
        if (!methodInfo.IsStatic)
        {
            generator.Emit(OpCodes.Ldarg_0);
            
            // Unbox the instance if it's a value type
            if (methodInfo.DeclaringType!.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, methodInfo.DeclaringType);
            }
        }
        var parameters = methodInfo.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            // Load parameter array
            generator.Emit(OpCodes.Ldarg_1);
            
            // Load array (and thus parameter) index
            generator.Emit(OpCodes.Ldc_I4, i);
            
            // Load element at index
            generator.Emit(OpCodes.Ldelem_Ref);

            if (parameters[i].ParameterType.IsValueType)
            {
                // Unbox value types
                generator.Emit(OpCodes.Unbox_Any, parameters[i].ParameterType);
            }
        }
        
        // If the method is virtual, we need to use Callvirt (which is slower),
        // otherwise we trust the method exists, and use Call
        generator.Emit(methodInfo.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, methodInfo);
        
        // Return variable from method
        generator.Emit(OpCodes.Ret);
    }
}