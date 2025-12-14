using KeyVaultTool.Features.Keys.Models;
using KeyVaultTool.Features.Keys.Services;

namespace KeyVaultTool.Tests.Features.Keys.Services;

public class KeyComparerTests
{
    [Fact]
    public void Compare_ReturnsMissingInTarget_WhenKeyMissingInTarget()
    {
        var source = new List<KeySnapshot> { new("key1", true, "RSA", null) };
        var target = new List<KeySnapshot>();
        var comparer = new KeyComparer();
        var result = comparer.Compare(source, target);
        Assert.Single(result.MissingInTarget);
        Assert.Empty(result.MissingInSource);
        Assert.Empty(result.AttributeDifferences);
        Assert.Equal("key1", result.MissingInTarget[0].Name);
    }

    [Fact]
    public void Compare_ReturnsMissingInSource_WhenKeyMissingInSource()
    {
        var source = new List<KeySnapshot>();
        var target = new List<KeySnapshot> { new("key2", true, "RSA", null) };
        var comparer = new KeyComparer();
        var result = comparer.Compare(source, target);
        Assert.Empty(result.MissingInTarget);
        Assert.Single(result.MissingInSource);
        Assert.Empty(result.AttributeDifferences);
        Assert.Equal("key2", result.MissingInSource[0].Name);
    }

    [Fact]
    public void Compare_ReturnsAttributeDifferences_WhenKeyAttributesDiffer()
    {
        var source = new List<KeySnapshot> { new("key3", true, "RSA", null) };
        var target = new List<KeySnapshot> { new("key3", false, "EC", null) };
        var comparer = new KeyComparer();
        var result = comparer.Compare(source, target);
        Assert.Empty(result.MissingInTarget);
        Assert.Empty(result.MissingInSource);
        Assert.Single(result.AttributeDifferences);
        Assert.Equal("key3", result.AttributeDifferences[0].Source.Name);
        Assert.Equal("key3", result.AttributeDifferences[0].Target.Name);
    }

    [Fact]
    public void Compare_ReturnsNoDifferences_WhenKeysAreIdentical()
    {
        var source = new List<KeySnapshot> { new("key4", true, "RSA", null) };
        var target = new List<KeySnapshot> { new("key4", true, "RSA", null) };
        var comparer = new KeyComparer();
        var result = comparer.Compare(source, target);
        Assert.Empty(result.MissingInTarget);
        Assert.Empty(result.MissingInSource);
        Assert.Empty(result.AttributeDifferences);
    }

    [Fact]
    public void Compare_IgnoresCase_WhenComparingKeyNames()
    {
        var source = new List<KeySnapshot> { new("KEY5", true, "RSA", null) };
        var target = new List<KeySnapshot> { new("key5", true, "RSA", null) };
        var comparer = new KeyComparer();
        var result = comparer.Compare(source, target);
        Assert.Empty(result.MissingInTarget);
        Assert.Empty(result.MissingInSource);
        Assert.Empty(result.AttributeDifferences);
    }
}
