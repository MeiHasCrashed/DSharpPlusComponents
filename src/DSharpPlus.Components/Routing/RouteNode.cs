namespace DSharpPlus.Components.Routing;

public class RouteNode<T> where T : class
{
    public required string Name { get; init; }
    public Dictionary<string, RouteNode<T>> Children { get; init; } = [];
    
    public T? Value { get; set; }
}