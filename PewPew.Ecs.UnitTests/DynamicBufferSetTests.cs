using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class DynamicBufferSetTests
{
    private readonly DoubleSizeStrategy _resizeStrategy = new(2);
    private readonly StubEntityManager _stubEntityManager = new();

    [Fact]
    public void AddBuffer_AddMoreThanInitialSetCapacity_ShouldResizeSet()
    {
        var set = new DynamicBufferSet<DynamicDamage>(10, 1, 2, _resizeStrategy, _stubEntityManager);

        set.AddBuffer(new EntityId(0, 1, 0));
        set.AddBuffer(new EntityId(1, 1, 0));

        set.BufferCount.Should().Be(2);
        set.HasBuffer(new EntityId(0, 1, 0)).Should().BeTrue();
        set.HasBuffer(new EntityId(1, 1, 0)).Should().BeTrue();
    }

    [Fact]
    public void DeleteBuffer_SwapAndPop_ShouldKeepRemainingBufferData()
    {
        var set = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(0, 1, 0);
        var second = new EntityId(1, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicDamage { Value = 10 });
        set.AddBuffer(second).AddLast(new DynamicDamage { Value = 20 });

        set.DeleteBuffer(first);

        set.HasBuffer(first).Should().BeFalse();
        set.HasBuffer(second).Should().BeTrue();
        set.GetBuffer(second).Components[0].Value.Should().Be(20);
    }

    [Fact]
    public void DeleteBuffer_ComponentDoesNotExist_ShouldBeOk()
    {
        var set = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);

        var action = () => set.DeleteBuffer(new EntityId(3, 1, 0));

        action.Should().NotThrow();
    }

    [Fact]
    public void Clear_ShouldDeleteAllBuffers()
    {
        var set = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(0, 1, 0);
        var second = new EntityId(1, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicDamage { Value = 1 });
        set.AddBuffer(second).AddLast(new DynamicDamage { Value = 2 });

        set.Clear();

        set.BufferCount.Should().Be(0);
        set.HasBuffer(first).Should().BeFalse();
        set.HasBuffer(second).Should().BeFalse();
    }

    [Fact]
    public void TryGetBuffer_Missing_ShouldReturnFalse()
    {
        var set = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);

        var result = set.TryGetBuffer(new EntityId(5, 1, 0), out _);

        result.Should().BeFalse();
    }

    [Fact]
    public void AddAfterDelete_ShouldReturnEmptyBuffer()
    {
        var set = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(0, 1, 0);

        set.AddBuffer(entityId).AddLast(new DynamicDamage { Value = 100 });
        set.DeleteBuffer(entityId);

        var readded = set.AddBuffer(entityId);

        readded.Count.Should().Be(0);
    }

    [Fact]
    public void AddBuffer_SameEntityTwice_ShouldReturnExisting()
    {
        var set = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(0, 1, 0);

        var first = set.AddBuffer(entityId);
        first.AddLast(new DynamicDamage { Value = 33 });

        var second = set.AddBuffer(entityId);

        second.Count.Should().Be(1);
        second.Components[0].Value.Should().Be(33);
        set.BufferCount.Should().Be(1);
    }

    [Fact]
    public void TryGetBuffer_Existing_ShouldReturnTrue()
    {
        var set = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(0, 1, 0);

        set.AddBuffer(entityId).AddLast(new DynamicDamage { Value = 99 });

        var result = set.TryGetBuffer(entityId, out var buffer);

        result.Should().BeTrue();
        buffer.Count.Should().Be(1);
        buffer.Components[0].Value.Should().Be(99);
    }
}



