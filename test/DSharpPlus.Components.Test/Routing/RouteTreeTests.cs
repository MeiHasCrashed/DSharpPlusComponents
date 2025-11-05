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
        
        var (value, wildcards) = routeTree.Search("a-b-c");
        Assert.Equal("value1", value);
        Assert.Empty(wildcards);
    }

    [Fact]
    public void TestRouteTreeWithWildcard()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-c", "value2");
        
        var (value, wildcards) = routeTree.Search("a-b-c");
        Assert.Equal("value2", value);
        Assert.Single(wildcards);
        Assert.Equal("b", wildcards[0]);
    }

    [Fact]
    public void TestRouteTreeNoMatch()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-b-c", "value3");
        var (value, wildcards) = routeTree.Search("a-x-c");
        Assert.Null(value);
        Assert.Empty(wildcards);
    }

    [Fact]
    public void TestRouteTreeMultipleWildcards()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-*-d", "value4");
        var (value, wildcards) = routeTree.Search("a-b-c-d");
        Assert.Equal("value4", value);
        Assert.Equal(2, wildcards.Count);
        Assert.Equal("b", wildcards[0]);
        Assert.Equal("c", wildcards[1]);
    }

    [Fact]
    public void TestRouteTreeMultipleWildcardsNoMatch()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-*-d", "value5");
        var (value, wildcards) = routeTree.Search("a-b-c-e");
        Assert.Null(value);
        Assert.Empty(wildcards);
    }
    
    [Fact]
    public void TestRouteTreeOverlappingRoutes()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-b-c", "value7");
        routeTree.Insert("a-*-c", "value6");
        
        var (value1, wildcards1) = routeTree.Search("a-b-c");
        Assert.Equal("value7", value1);
        Assert.Empty(wildcards1);
        
        var (value2, wildcards2) = routeTree.Search("a-x-c");
        Assert.Equal("value6", value2);
        Assert.Single(wildcards2);
        Assert.Equal("x", wildcards2[0]);
    }

    [Fact]
    public void TestRouteTreeOverlappingWildcards()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-c", "value8");
        routeTree.Insert("a-*-*", "value9");
        
        var (value1, wildcards1) = routeTree.Search("a-b-c");
        Assert.Equal("value8", value1);
        Assert.Single(wildcards1);
        Assert.Equal("b", wildcards1[0]);
    }

    [Fact]
    public void TestRouteTreeNoLeadingSeparator()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("-a-b-c", "value10");
        var (value, wildcards) = routeTree.Search("-a-b-c");
        Assert.Equal("value10", value);
        Assert.Empty(wildcards);
    }
    
    [Fact]
    public void TestRouteTreePrecedence()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-b-c", "value11");
        routeTree.Insert("a-b-d", "value12");
        routeTree.Insert("a-b", "value13");
        routeTree.Insert("a-*-e", "value14");
        
        var (value1, wildcards1) = routeTree.Search("a-b-c");
        Assert.Equal("value11", value1);
        Assert.Empty(wildcards1);
        
        var (value2, wildcards2) = routeTree.Search("a-b-d");
        Assert.Equal("value12", value2);
        Assert.Empty(wildcards2);
        
        var (value3, wildcards3) = routeTree.Search("a-b");
        Assert.Equal("value13", value3);
        Assert.Empty(wildcards3);
        
        var (value4, wildcards4) = routeTree.Search("a-x-e");;
        Assert.Equal("value14", value4);
        Assert.Single(wildcards4);
        Assert.Equal("x", wildcards4[0]);
    }
}