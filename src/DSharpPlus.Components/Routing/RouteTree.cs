namespace DSharpPlus.Components.Routing;

public class RouteTree<T>(char separator = '-') where T : class
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly RouteNode<T> _root = new() { Name = "*"};
    private int _routeCount;

    public void Insert(string path, T value)
    {
        try
        {
            _lock.EnterWriteLock();
            var segments = SegmentPath(path);
            var currentNode = _root;
            foreach (var item in segments)
            {
                if (!currentNode.Children.TryGetValue(item, out var childNode))
                {
                    childNode = new RouteNode<T> { Name = item };
                    currentNode.Children[item] = childNode;
                }

                currentNode = childNode;
            }

            if (currentNode.Value is not null)
            {
                throw new InvalidOperationException($"A value is already registered for the path '{path}'.");
            }

            currentNode.Value = value;
            _routeCount++;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
    
    public RouteResult<T> Match(string path)
    {
        try
        {
            _lock.EnterReadLock();
            var segments = SegmentPath(path);

            var currentNode = _root;
            List<string> wildcards = [];
            RouteNode<T>? backtrackNode = null;
            List<string> backtrackWildcards = [];
            foreach (var item in segments)
            {
                if (currentNode.Children.TryGetValue("*", out var maybeBacktrackNode) && maybeBacktrackNode is
                        { Value: not null, Children.Count: 0 })
                {
                    backtrackNode = maybeBacktrackNode;
                    backtrackWildcards.Clear();
                    backtrackWildcards.AddRange(wildcards);
                }

                if (currentNode.Children.TryGetValue(item, out var childNode))
                {
                    currentNode = childNode;
                    if (backtrackNode is not null)
                    {
                        backtrackWildcards.Add(item);
                    }
                }
                else if (currentNode.Children.TryGetValue("*", out var wildcardNode))
                {
                    currentNode = wildcardNode;
                    wildcards.Add(item);
                }
                else if (currentNode is { IsWildcard: true, Children.Count: 0 })
                {
                    wildcards.Add(item);
                }
                else
                {
                    return new RouteResult<T> { IsMatch = false, Value = null, Wildcards = [] };
                }
            }

            if (currentNode.Value is null && backtrackNode is null)
            {
                return new RouteResult<T> { IsMatch = false, Value = null, Wildcards = [] };
            }

            if (currentNode.Value is not null)
            {
                return new RouteResult<T> { IsMatch = true, Value = currentNode.Value, Wildcards = wildcards };
            }

            return new RouteResult<T> { IsMatch = true, Value = backtrackNode!.Value, Wildcards = backtrackWildcards };

        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
    
    public RouteNode<T> GetRoot() => _root;

    public int Count
    {
        get
        {
            try
            {
                _lock.EnterReadLock();
                return _routeCount;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    private string[] SegmentPath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        // If the path starts with a separator, remove it, since the root node represents the starting point.
        if(path[0] == separator)
            path = path[1..];
        if(path[^1] == separator)
            path = path[..^1];
        return path.Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }
}