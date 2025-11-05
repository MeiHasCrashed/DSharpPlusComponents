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
        Assert.Null(tree.Value);
        Assert.Empty(tree.Children);
        
        var result = routeTree.Match("a-b-c");
        Assert.False(result.IsMatch);
        Assert.Null(result.Value);
        Assert.Empty(result.Wildcards);
    }

    [Fact]
    public void TestRouteTreeBasic()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-b-c", "value1");
        
        var result = routeTree.Match("a-b-c");
        Assert.True(result.IsMatch);
        Assert.Equal("value1", result.Value);
        Assert.Empty(result.Wildcards);
    }

    [Fact]
    public void TestRouteTreeWithWildcard()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-c", "value2");
        
        var result = routeTree.Match("a-b-c");
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
        var result = routeTree.Match("a-x-c");
        Assert.False(result.IsMatch);
        Assert.Null(result.Value);
        Assert.Empty(result.Wildcards);
    }

    [Fact]
    public void TestRouteTreeMultipleWildcards()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-*-*-d", "value4");
        var result = routeTree.Match("a-b-c-d");
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
        var result = routeTree.Match("a-b-c-e");
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
        
        var result1 = routeTree.Match("a-b-c");
        Assert.True(result1.IsMatch);
        Assert.Equal("value7", result1.Value);
        Assert.Empty(result1.Wildcards);
        
        var result2 = routeTree.Match("a-x-c");
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
        
        var result = routeTree.Match("a-b-c");
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
        var result = routeTree.Match("-a-b-c");
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
        
        var result1 = routeTree.Match("a-b-c");
        Assert.True(result1.IsMatch);
        Assert.Equal("value11", result1.Value);
        Assert.Empty(result1.Wildcards);
        
        var result2 = routeTree.Match("a-b-d");
        Assert.True(result2.IsMatch);
        Assert.Equal("value12", result2.Value);
        Assert.Empty(result2.Wildcards);
        
        var result3 = routeTree.Match("a-b");
        Assert.True(result3.IsMatch);
        Assert.Equal("value13", result3.Value);
        Assert.Empty(result3.Wildcards);
        
        var result4 = routeTree.Match("a-x-e");
        Assert.True(result4.IsMatch);
        Assert.Equal("value14", result4.Value);
        Assert.Single(result4.Wildcards);
        Assert.Equal("x", result4.Wildcards[0]);
    }

    [Fact]
    public void TestRouteTreeCatchAll()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a-b-*", "value15");
        routeTree.Insert("a-b-d", "value16");
        
        var result1 = routeTree.Match("a-b-c");
        Assert.True(result1.IsMatch);
        Assert.Equal("value15", result1.Value);
        Assert.Single(result1.Wildcards);
        Assert.Equal("c", result1.Wildcards[0]);
        
        var result2 = routeTree.Match("a-b-d");
        Assert.True(result2.IsMatch);
        Assert.Equal("value16", result2.Value);
        Assert.Empty(result2.Wildcards);

        var result3 = routeTree.Match("a-b-fge-1234");
        Assert.True(result3.IsMatch);
        Assert.Equal("value15", result3.Value);
        Assert.Equal(2, result3.Wildcards.Count);
        Assert.Equal("fge", result3.Wildcards[0]);
        Assert.Equal("1234", result3.Wildcards[1]);
    }

    [Fact]
    public void TestRouteTreeEmptyPath()
    {
        var routeTree = new RouteTree<string>('-');
        Assert.Throws<ArgumentException>(() => routeTree.Insert("", "value"));
        Assert.Throws<ArgumentException>(() => routeTree.Match(""));

        Assert.Throws<ArgumentNullException>(() => routeTree.Match(null!));
        Assert.Throws<ArgumentNullException>(() => routeTree.Insert(null!, "value"));
    }

    [Fact]
    public void TestRouteTreeBacktrack()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("button-test-*", "value17");
        routeTree.Insert("button-test-12345-34as", "value18");
        
        var result1 = routeTree.Match("button-test-12345-34as");
        Assert.True(result1.IsMatch);
        Assert.Equal("value18", result1.Value);
        Assert.Empty(result1.Wildcards);
        
        var result2 = routeTree.Match("button-test-12345");
        Assert.True(result2.IsMatch);
        Assert.Equal("value17", result2.Value);
        Assert.Single(result2.Wildcards);
        Assert.Equal("12345", result2.Wildcards[0]);
    }

    [Fact]
    public void TestRouteTreeEmptySegments()
    {
        var routeTree = new RouteTree<string>('-');
        routeTree.Insert("a--b-c", "value19");
        var result = routeTree.Match("a--b-c");
        Assert.True(result.IsMatch);
        Assert.Equal("value19", result.Value);
        Assert.Empty(result.Wildcards);
        
        var result2 = routeTree.Match("a-b-c");
        Assert.True(result2.IsMatch);
        Assert.Equal("value19", result2.Value);
        Assert.Empty(result2.Wildcards);
    }
}