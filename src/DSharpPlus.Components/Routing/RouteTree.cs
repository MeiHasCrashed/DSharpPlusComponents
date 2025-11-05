namespace DSharpPlus.Components.Routing;

public class RouteTree<T>(char separator) where T : class
{
    private readonly RouteNode<T> _root = new() { Name = "*"};

    public void Insert(string path, T value)
    {
        // If the path starts with a separator, remove it, since the root node represents the starting point.
        if(path[0] == separator)
            path = path[1..];
        var split = path.Split(separator);
        
        var currentNode = _root;
        foreach (var item in split)
        {
            if (!currentNode.Children.TryGetValue(item, out var childNode))
            {
                childNode = new RouteNode<T> { Name = item };
                currentNode.Children[item] = childNode;
            }

            currentNode = childNode;
        }
        currentNode.Value = value;
    }
    
    public RouteResult<T> Match(string path)
    {
        if(path[0] == separator)
            path = path[1..];
        var split = path.Split(separator);
        
        var currentNode = _root;
        List<string> wildcards = [];
        foreach (var item in split)
        {
            if (currentNode.Children.TryGetValue(item, out var childNode))
            {
                currentNode = childNode;
            }
            else if (currentNode.Children.TryGetValue("*", out var wildcardNode))
            {
                currentNode = wildcardNode;
                wildcards.Add(item);
            }
            else
            {
                return new RouteResult<T> { IsMatch = false, Value = null, Wildcards = [] };
            }
        }
        
        return new RouteResult<T>{ IsMatch = true, Value = currentNode.Value, Wildcards = wildcards };
    }
    
    public RouteNode<T> GetRoot() => _root;
}