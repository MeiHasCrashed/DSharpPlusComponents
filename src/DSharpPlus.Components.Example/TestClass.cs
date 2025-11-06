namespace DSharpPlus.Components.Example;

public class TestClass
{
    public ValueTask ExecuteSomethingAsync(string key, string key2)
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