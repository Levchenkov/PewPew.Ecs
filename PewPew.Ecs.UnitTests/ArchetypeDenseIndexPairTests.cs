using PewPew.Ecs.Hybrid;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.UnitTests;

public class ArchetypeDenseIndexPairTests
{
    [Fact]
    public void Constructor_SetsValueCorrectly()
    {
        int index = 12345;
        int archetypeId = 678;
        var pair = new ArchetypeDenseIndexPair(archetypeId, index);

        Assert.Equal((ulong)archetypeId << 32 | (uint)index, pair.Value);
    }

    [Fact]
    public void DenseIndex_ReturnsCorrectValue()
    {
        int index = 12345;
        int archetypeId = 678;
        var pair = new ArchetypeDenseIndexPair(archetypeId, index);

        Assert.Equal(index, pair.DenseIndex);
    }

    [Fact]
    public void ArchetypeId_ReturnsCorrectValue()
    {
        int index = 12345;
        int archetypeId = 678;
        var pair = new ArchetypeDenseIndexPair(archetypeId, index);

        Assert.Equal(archetypeId, pair.ArchetypeId);
    }

    [Fact]
    public void MaxValues_HandlesCorrectly()
    {
        int maxIndex = int.MaxValue;
        int maxArchetypeId = int.MaxValue;
        var pair = new ArchetypeDenseIndexPair(maxArchetypeId, maxIndex);

        Assert.Equal(maxIndex, pair.DenseIndex);
        Assert.Equal(maxArchetypeId, pair.ArchetypeId);
    }
}