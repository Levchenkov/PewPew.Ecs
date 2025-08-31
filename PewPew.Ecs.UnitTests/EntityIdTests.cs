using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class EntityIdTests
{
    [Fact]
    public void Constructor_SetsValueCorrectly()
    {
        int index = 12345;
        ushort generation = 678;
        ushort worldId = 90;
        var entity = new EntityId(index, generation, worldId);

        Assert.Equal((ulong)((ulong)worldId << 48 | (ulong)generation << 32 | (uint)index), entity.Value);
    }

    [Fact]
    public void Index_ReturnsCorrectValue()
    {
        int index = 12345;
        ushort generation = 678;
        ushort worldId = 90;
        var entity = new EntityId(index, generation, worldId);

        Assert.Equal(index, entity.Index);
    }

    [Fact]
    public void Generation_ReturnsCorrectValue()
    {
        int index = 12345;
        ushort generation = 678;
        ushort worldId = 90;
        var entity = new EntityId(index, generation, worldId);

        Assert.Equal(generation, entity.Generation);
    }

    [Fact]
    public void WorldId_ReturnsCorrectValue()
    {
        int index = 12345;
        ushort generation = 678;
        ushort worldId = 90;
        var entity = new EntityId(index, generation, worldId);

        Assert.Equal(worldId, entity.WorldId);
    }

    [Fact]
    public void MaxValues_HandlesCorrectly()
    {
        int maxIndex = int.MaxValue;
        ushort maxGeneration = ushort.MaxValue;
        ushort maxWorldId = ushort.MaxValue;
        var entity = new EntityId(maxIndex, maxGeneration, maxWorldId);

        Assert.Equal(maxIndex, entity.Index);
        Assert.Equal(maxGeneration, entity.Generation);
        Assert.Equal(maxWorldId, entity.WorldId);
    }
}