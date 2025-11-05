namespace DSharpPlus.Components.Routing;

public struct RouteResult<T> where T : class
{
    public T? Value { get; init; }
    public bool IsMatch { get; init; }
    public List<string> Wildcards { get; init; }
}