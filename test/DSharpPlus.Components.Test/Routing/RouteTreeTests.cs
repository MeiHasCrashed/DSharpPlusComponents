using DSharpPlus.Components.Routing;

namespace DSharpPlus.Components.Test.Routing;

public class RouteTreeTests
{
    [Fact]
    public void TestRouteTreeEmpty()
    {
        var routeTree = new RouteTree<string>('-');

        var tree = routeTree.GetRoot();
        Assert.NotNull(tree);
        Assert.Equal("*", tree.Name);
        Assert.Empty(tree.Children);
    }

    [Fact]
    public void TestRouteTreeBasic()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-b-c", "value1");
        
        var result = routeTree.Search("a-b-c");
        Assert.True(result.IsMatch);
        Assert.Equal("value1", result.Value);
        Assert.Empty(result.Wildcards);
    }

    [Fact]
    public void TestRouteTreeWithWildcard()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-c", "value2");
        
        var result = routeTree.Search("a-b-c");
        Assert.True(result.IsMatch);
        Assert.Equal("value2", result.Value);
        Assert.Single(result.Wildcards);
        Assert.Equal("b", result.Wildcards[0]);
    }

    [Fact]
    public void TestRouteTreeNoMatch()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-b-c", "value3");
        var result = routeTree.Search("a-x-c");
        Assert.False(result.IsMatch);
        Assert.Null(result.Value);
        Assert.Empty(result.Wildcards);
    }

    [Fact]
    public void TestRouteTreeMultipleWildcards()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-*-d", "value4");
        var result = routeTree.Search("a-b-c-d");
        Assert.True(result.IsMatch);
        Assert.Equal("value4", result.Value);
        Assert.Equal(2, result.Wildcards.Count);
        Assert.Equal("b", result.Wildcards[0]);
        Assert.Equal("c", result.Wildcards[1]);
    }

    [Fact]
    public void TestRouteTreeMultipleWildcardsNoMatch()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-*-d", "value5");
        var result = routeTree.Search("a-b-c-e");
        Assert.False(result.IsMatch);
        Assert.Null(result.Value);
        Assert.Empty(result.Wildcards);
    }
    
    [Fact]
    public void TestRouteTreeOverlappingRoutes()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-b-c", "value7");
        routeTree.Insert("a-*-c", "value6");
        
        var result1 = routeTree.Search("a-b-c");
        Assert.True(result1.IsMatch);
        Assert.Equal("value7", result1.Value);
        Assert.Empty(result1.Wildcards);
        
        var result2 = routeTree.Search("a-x-c");
        Assert.True(result2.IsMatch);
        Assert.Equal("value6", result2.Value);
        Assert.Single(result2.Wildcards);
        Assert.Equal("x", result2.Wildcards[0]);
    }

    [Fact]
    public void TestRouteTreeOverlappingWildcards()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-c", "value8");
        routeTree.Insert("a-*-*", "value9");
        
        var result = routeTree.Search("a-b-c");
        Assert.True(result.IsMatch);
        Assert.Equal("value8", result.Value);
        Assert.Single(result.Wildcards);
        Assert.Equal("b", result.Wildcards[0]);
    }

    [Fact]
    public void TestRouteTreeNoLeadingSeparator()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("-a-b-c", "value10");
        var result = routeTree.Search("-a-b-c");
        Assert.True(result.IsMatch);
        Assert.Equal("value10", result.Value);
        Assert.Empty(result.Wildcards);
    }
    
    [Fact]
    public void TestRouteTreePrecedence()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-b-c", "value11");
        routeTree.Insert("a-b-d", "value12");
        routeTree.Insert("a-b", "value13");
        routeTree.Insert("a-*-e", "value14");
        
        var result1 = routeTree.Search("a-b-c");
        Assert.True(result1.IsMatch);
        Assert.Equal("value11", result1.Value);
        Assert.Empty(result1.Wildcards);
        
        var result2 = routeTree.Search("a-b-d");
        Assert.True(result2.IsMatch);
        Assert.Equal("value12", result2.Value);
        Assert.Empty(result2.Wildcards);
        
        var result3 = routeTree.Search("a-b");
        Assert.True(result3.IsMatch);
        Assert.Equal("value13", result3.Value);
        Assert.Empty(result3.Wildcards);
        
        var result4 = routeTree.Search("a-x-e");
        Assert.True(result4.IsMatch);
        Assert.Equal("value14", result4.Value);
        Assert.Single(result4.Wildcards);
        Assert.Equal("x", result4.Wildcards[0]);
    }
}