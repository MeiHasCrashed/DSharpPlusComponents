// We fully disable warnings here, since this files only purpose is for me to see the correct il of those method calls.
// ReSharper disable all
namespace DSharpPlus.Components.Example;

public class TestClass
{
#pragma warning disable CA1822
    public ValueTask ExecuteSomethingAsync(string key, string key2)
#pragma warning restore CA1822
    {
        return ValueTask.CompletedTask;
    }

    public static ValueTask ExecuteSomethingElse(TestClass instance, string[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("Args must contain at least two elements.", nameof(args));
        }
        return instance.ExecuteSomethingAsync(args[0], args[1]);
    }
}