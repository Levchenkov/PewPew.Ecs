using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class DynamicBufferSetTests2
{
    private readonly DoubleSizeStrategy _resizeStrategy = new(10);
    private readonly StubEntityManager _stubEntityManager = new();

    [Fact]
    public void HasBuffer_EmptySet_ShouldBeFalse()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);

        set.HasBuffer(new EntityId(1, 1, 0)).Should().BeFalse();
    }

    [Fact]
    public void HasBuffer_ComponentDoesntExist_ShouldBeFalse()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        set.AddBuffer(new EntityId(1, 1, 0));

        set.HasBuffer(new EntityId(2, 1, 0)).Should().BeFalse();
    }

    [Fact]
    public void HasBuffer_ComponentExists_ShouldBeTrue()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        set.AddBuffer(new EntityId(1, 1, 0));

        set.HasBuffer(new EntityId(1, 1, 0)).Should().BeTrue();
    }

    [Fact]
    public void AddBuffer_AddTwiceTheSameEntity_ShouldReturnExisting()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var buffer = set.AddBuffer(new EntityId(3, 1, 0));
        buffer.AddLast(new DynamicEvent { Value = 42 });

        var buffer2 = set.AddBuffer(new EntityId(3, 1, 0));

        buffer2.Count.Should().Be(1);
        buffer2.Components[0].Value.Should().Be(42);
    }

    [Fact]
    public void AddBuffer_ExceedInitialCapacity_ShouldAutoResize()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        var buffer = set.AddBuffer(new EntityId(1, 1, 0));

        buffer.AddLast(new DynamicEvent { Value = 1 });
        buffer.AddLast(new DynamicEvent { Value = 2 });
        buffer.AddLast(new DynamicEvent { Value = 3 }); // exceeds initial capacity of 2

        buffer.Count.Should().Be(3);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(2);
        buffer.Components[2].Value.Should().Be(3);
    }

    [Fact]
    public void Entities_TwoBuffersExist_ShouldBeTwo()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        set.AddBuffer(new EntityId(3, 1, 0));
        set.AddBuffer(new EntityId(5, 1, 0));

        set.Entities.Length.Should().Be(2);
        set.Entities[0].Index.Should().Be(3);
        set.Entities[1].Index.Should().Be(5);
    }

    [Fact]
    public void Entities_EmptySet_ShouldBeEmpty()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);

        set.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void GetBuffer_ComponentDoesntExist_ShouldThrow()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        set.AddBuffer(new EntityId(0, 1, 0));

        Action action = () => set.GetBuffer(new EntityId(1, 1, 0));

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void TryGetBuffer_ComponentExists_ShouldReturnTrue()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);
        var added = set.AddBuffer(entityId);
        added.AddLast(new DynamicEvent { Value = 99 });

        var found = set.TryGetBuffer(entityId, out var buffer);

        found.Should().BeTrue();
        buffer.Count.Should().Be(1);
        buffer.Components[0].Value.Should().Be(99);
    }

    [Fact]
    public void TryGetBuffer_ComponentDoesntExist_ShouldReturnFalse()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);

        var found = set.TryGetBuffer(new EntityId(1, 1, 0), out _);

        found.Should().BeFalse();
    }

    [Fact]
    public void DeleteBuffer_ComponentDoesNotExist_ShouldBeOk()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);

        set.DeleteBuffer(new EntityId(3, 1, 0));
    }

    [Fact]
    public void DeleteBuffer_ComponentExists_ShouldBeOk()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);
        set.AddBuffer(entityId);
        set.DeleteBuffer(entityId);

        set.HasBuffer(entityId).Should().BeFalse();
    }

    [Fact]
    public void DeleteBuffer_TwoBuffersDeleteFirst_ShouldBeOk()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicEvent { Value = 42 });
        set.AddBuffer(second).AddLast(new DynamicEvent { Value = 69 });

        set.DeleteBuffer(first);

        set.HasBuffer(first).Should().BeFalse();
        set.HasBuffer(second).Should().BeTrue();
        set.GetBuffer(second).Components[0].Value.Should().Be(69);
    }

    [Fact]
    public void DeleteBuffer_TwoBuffersDeleteSecond_ShouldBeOk()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicEvent { Value = 42 });
        set.AddBuffer(second).AddLast(new DynamicEvent { Value = 69 });

        set.DeleteBuffer(second);

        set.HasBuffer(first).Should().BeTrue();
        set.HasBuffer(second).Should().BeFalse();
        set.GetBuffer(first).Components[0].Value.Should().Be(42);
    }

    [Fact]
    public void DeleteBuffer_ThreeBuffersDeleteMiddle_ShouldBeOk()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 3, 4, _resizeStrategy, _stubEntityManager);
        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);
        var third = new EntityId(7, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicEvent { Value = 42 });
        set.AddBuffer(second).AddLast(new DynamicEvent { Value = 69 });
        set.AddBuffer(third).AddLast(new DynamicEvent { Value = 1024 });

        set.DeleteBuffer(second);

        set.HasBuffer(first).Should().BeTrue();
        set.HasBuffer(second).Should().BeFalse();
        set.HasBuffer(third).Should().BeTrue();
        set.GetBuffer(first).Components[0].Value.Should().Be(42);
        set.GetBuffer(third).Components[0].Value.Should().Be(1024);
    }

    [Fact]
    public void AddBuffer_AddAfterDeletingFirst_ShouldBeOk()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicEvent { Value = 42 });
        set.AddBuffer(second).AddLast(new DynamicEvent { Value = 69 });

        set.DeleteBuffer(first);

        set.AddBuffer(first).AddLast(new DynamicEvent { Value = 42 });

        set.BufferCount.Should().Be(2);
        set.GetBuffer(first).Count.Should().Be(1);
        set.GetBuffer(first).Components[0].Value.Should().Be(42);
        set.GetBuffer(second).Components[0].Value.Should().Be(69);
    }

    [Fact]
    public void Clear_ShouldResetAll()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicEvent { Value = 42 });
        set.AddBuffer(second).AddLast(new DynamicEvent { Value = 69 });

        set.Clear();

        set.BufferCount.Should().Be(0);
        set.HasBuffer(first).Should().BeFalse();
        set.HasBuffer(second).Should().BeFalse();
    }

    [Fact]
    public void SlotExpansion_ExceedInitialSlotCount_ShouldResize()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 1, 4, _resizeStrategy, _stubEntityManager);

        for (var i = 0; i < 5; i++)
        {
            set.AddBuffer(new EntityId(i, 1, 0)).AddLast(new DynamicEvent { Value = i });
        }

        set.BufferCount.Should().Be(5);
        for (var i = 0; i < 5; i++)
        {
            set.GetBuffer(new EntityId(i, 1, 0)).Components[0].Value.Should().Be(i);
        }
    }

    [Fact]
    public void Handle_AfterDelete_AddLast_ShouldThrow()
    {
        var set = new DynamicBufferSet<DynamicEvent>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(1, 1, 0);

        var handle = set.AddBuffer(entityId);
        set.DeleteBuffer(entityId); // invalidates the underlying DynamicBufferInstance

        var threw = false;
        try { handle.AddLast(new DynamicEvent { Value = 1 }); }
        catch { threw = true; }

        threw.Should().BeTrue();
    }
}
