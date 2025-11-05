using System.Reflection;

namespace DSharpPlus.Components.Util;

public class ReflectionUtil
{
    public static IEnumerable<MethodInfo> ScanAssemblyForAttributedMethods<TAttribute>(Assembly assembly)
        where TAttribute : Attribute
    {
        foreach (var type in assembly.GetTypes())
        {
            foreach (var method in type.GetMethods())
            {
                if (method.GetCustomAttribute<TAttribute>() != null)
                {
                    yield return method;
                }
            }
        }
    }
}