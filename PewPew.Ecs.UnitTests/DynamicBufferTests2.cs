using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class DynamicBufferTests2
{
    private readonly DoubleSizeStrategy _resizeStrategy = new(10);
    private readonly StubEntityManager _stubEntityManager = new();

    private DynamicBuffer<DynamicEvent> CreateBuffer(int initialCapacity = 4)
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, initialCapacity, _resizeStrategy, _stubEntityManager);
        return set.AddBuffer(new EntityId(1, 1, 0));
    }

    [Fact]
    public void EmptyBuffer_ShouldBeEmpty()
    {
        var buffer = CreateBuffer();

        buffer.Count.Should().Be(0);
        buffer.IsEmpty.Should().BeTrue();
        buffer.Capacity.Should().Be(4);
    }

    [Fact]
    public void AddLast_ShouldAppend()
    {
        var buffer = CreateBuffer();

        buffer.AddLast(new DynamicEvent { Value = 10 });
        buffer.AddLast(new DynamicEvent { Value = 20 });

        buffer.Count.Should().Be(2);
        buffer.IsEmpty.Should().BeFalse();
        buffer.Components[0].Value.Should().Be(10);
        buffer.Components[1].Value.Should().Be(20);
    }

    [Fact]
    public void AddLast_ExceedsCapacity_ShouldAutoGrow()
    {
        var buffer = CreateBuffer(initialCapacity: 2);

        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.AddLast(new DynamicEvent { Value = 2 });
        buffer.AddLast(new DynamicEvent { Value = 3 }); // triggers grow

        buffer.Count.Should().Be(3);
        buffer.Capacity.Should().BeGreaterThan(2);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(2);
        buffer.Components[2].Value.Should().Be(3);
    }

    [Fact]
    public void RemoveLast_ShouldRemoveAndReturn()
    {
        var buffer = CreateBuffer();
        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.AddLast(new DynamicEvent { Value = 2 });

        var removed = buffer.RemoveLast();

        removed.Value.Should().Be(2);
        buffer.Count.Should().Be(1);
        buffer.Components[0].Value.Should().Be(1);
    }

    [Fact]
    public void SwapAndPop_First_LastShouldBecomeFirst()
    {
        var buffer = CreateBuffer();
        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.AddLast(new DynamicEvent { Value = 2 });
        buffer.AddLast(new DynamicEvent { Value = 3 });

        var removed = buffer.SwapAndPop(0);

        removed.Value.Should().Be(1);
        buffer.Count.Should().Be(2);
        buffer.Components[0].Value.Should().Be(3);
        buffer.Components[1].Value.Should().Be(2);
    }

    [Fact]
    public void SwapAndPop_Middle_LastShouldBecomeMiddle()
    {
        var buffer = CreateBuffer();
        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.AddLast(new DynamicEvent { Value = 2 });
        buffer.AddLast(new DynamicEvent { Value = 3 });

        var removed = buffer.SwapAndPop(1);

        removed.Value.Should().Be(2);
        buffer.Count.Should().Be(2);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(3);
    }

    [Fact]
    public void SwapAndPop_Last_ShouldJustPop()
    {
        var buffer = CreateBuffer();
        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.AddLast(new DynamicEvent { Value = 2 });
        buffer.AddLast(new DynamicEvent { Value = 3 });

        var removed = buffer.SwapAndPop(2);

        removed.Value.Should().Be(3);
        buffer.Count.Should().Be(2);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(2);
    }

    [Fact]
    public void Clear_ShouldResetCount()
    {
        var buffer = CreateBuffer();
        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.AddLast(new DynamicEvent { Value = 2 });

        buffer.Clear();

        buffer.Count.Should().Be(0);
        buffer.IsEmpty.Should().BeTrue();
        buffer.Capacity.Should().Be(4); // capacity unchanged
    }

    [Fact]
    public void EnsureCapacity_ShouldGrowArray()
    {
        var buffer = CreateBuffer(initialCapacity: 2);
        buffer.AddLast(new DynamicEvent { Value = 1 });

        buffer.EnsureCapacity(10);

        buffer.Count.Should().Be(1);
        buffer.Capacity.Should().BeGreaterOrEqualTo(10);
        buffer.Components[0].Value.Should().Be(1);
    }

    [Fact]
    public void EnsureCapacity_AlreadySufficient_ShouldNotChange()
    {
        var buffer = CreateBuffer(initialCapacity: 8);
        buffer.AddLast(new DynamicEvent { Value = 1 });

        buffer.EnsureCapacity(4); // less than current

        buffer.Capacity.Should().Be(8);
        buffer.Count.Should().Be(1);
    }

    [Fact]
    public void TrimExcess_ShouldShrinkToCount()
    {
        var buffer = CreateBuffer(initialCapacity: 2);
        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.AddLast(new DynamicEvent { Value = 2 });

        buffer.TrimExcess();

        buffer.Count.Should().Be(2);
        buffer.Capacity.Should().Be(2);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(2);
    }

    [Fact]
    public void TrimExcess_AfterClear_ShouldShrinkToOne()
    {
        var buffer = CreateBuffer(initialCapacity: 1);
        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.Clear();

        buffer.TrimExcess();

        buffer.Capacity.Should().Be(1);
        buffer.Count.Should().Be(0);
    }

    [Fact]
    public void AddLastAfterClear_ShouldWork()
    {
        var buffer = CreateBuffer();
        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.Clear();
        buffer.AddLast(new DynamicEvent { Value = 2 });

        buffer.Count.Should().Be(1);
        buffer.Components[0].Value.Should().Be(2);
    }

    [Fact]
    public void World_AddAndGetDynamicBuffer_ShouldWork()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitDynamicBuffer<DynamicEvent>();

        var entityId = world.CreateEntityId();
        var buffer = world.AddDynamicBuffer<DynamicEvent>(entityId);
        buffer.AddLast(new DynamicEvent { Value = 42 });

        var retrieved = world.GetDynamicBuffer<DynamicEvent>(entityId);

        retrieved.Count.Should().Be(1);
        retrieved.Components[0].Value.Should().Be(42);
    }

    [Fact]
    public void World_HasDynamicBuffer_ShouldReturnCorrectly()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitDynamicBuffer<DynamicEvent>();

        var entityId = world.CreateEntityId();
        world.HasDynamicBuffer<DynamicEvent>(entityId).Should().BeFalse();

        world.AddDynamicBuffer<DynamicEvent>(entityId);
        world.HasDynamicBuffer<DynamicEvent>(entityId).Should().BeTrue();
    }

    [Fact]
    public void World_DeleteDynamicBuffer_ShouldRemoveIt()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitDynamicBuffer<DynamicEvent>();

        var entityId = world.CreateEntityId();
        world.AddDynamicBuffer<DynamicEvent>(entityId);
        world.DeleteDynamicBuffer<DynamicEvent>(entityId);

        world.HasDynamicBuffer<DynamicEvent>(entityId).Should().BeFalse();
    }

    [Fact]
    public void World_DeleteEntityId_ShouldRemoveDynamicBuffer()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitDynamicBuffer<DynamicEvent>();

        var entityId = world.CreateEntityId();
        world.AddDynamicBuffer<DynamicEvent>(entityId);

        world.DeleteEntityId(entityId);

        var newEntityId = world.CreateEntityId();
        world.HasDynamicBuffer<DynamicEvent>(newEntityId).Should().BeFalse();
    }

    [Fact]
    public void World_TryGetDynamicBuffer_Found_ShouldReturnTrue()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitDynamicBuffer<DynamicEvent>();

        var entityId = world.CreateEntityId();
        world.AddDynamicBuffer<DynamicEvent>(entityId).AddLast(new DynamicEvent { Value = 7 });

        var found = world.TryGetDynamicBuffer<DynamicEvent>(entityId, out var buffer);

        found.Should().BeTrue();
        buffer.Count.Should().Be(1);
        buffer.Components[0].Value.Should().Be(7);
    }

    [Fact]
    public void World_TryGetDynamicBuffer_NotFound_ShouldReturnFalse()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitDynamicBuffer<DynamicEvent>();

        var entityId = world.CreateEntityId();
        var found = world.TryGetDynamicBuffer<DynamicEvent>(entityId, out _);

        found.Should().BeFalse();
    }

    [Fact]
    public void World_GetDynamicBuffers_ShouldReturnCollection()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitDynamicBuffer<DynamicEvent>();

        var e1 = world.CreateEntityId();
        var e2 = world.CreateEntityId();
        world.AddDynamicBuffer<DynamicEvent>(e1).AddLast(new DynamicEvent { Value = 1 });
        world.AddDynamicBuffer<DynamicEvent>(e2).AddLast(new DynamicEvent { Value = 2 });

        var collection = world.GetDynamicBuffers<DynamicEvent>();

        collection.BufferCount.Should().Be(2);
        collection.Entities.Length.Should().Be(2);
    }
}
