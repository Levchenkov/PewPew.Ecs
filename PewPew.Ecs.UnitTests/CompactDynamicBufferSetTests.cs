using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.UnitTests;

public class CompactDynamicBufferSetTests
{
    private readonly DoubleSizeStrategy _resizeStrategy = new(2);
    private readonly StubEntityManager _stubEntityManager = new();

    [Fact]
    public void HasBuffer_EmptySet_ShouldBeFalse()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);

        set.HasBuffer(new EntityId(1, 1, 0)).Should().BeFalse();
    }

    [Fact]
    public void HasBuffer_ComponentDoesntExist_ShouldBeFalse()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        set.AddBuffer(new EntityId(1, 1, 0));

        set.HasBuffer(new EntityId(2, 1, 0)).Should().BeFalse();
    }

    [Fact]
    public void HasBuffer_ComponentExists_ShouldBeTrue()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        set.AddBuffer(new EntityId(1, 1, 0));

        set.HasBuffer(new EntityId(1, 1, 0)).Should().BeTrue();
    }

    [Fact]
    public void AddBuffer_AddMoreThanInitialSetCapacity_ShouldResizeSet()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 1, 2, _resizeStrategy, _stubEntityManager);

        set.AddBuffer(new EntityId(0, 1, 0));
        set.AddBuffer(new EntityId(1, 1, 0));

        set.BufferCount.Should().Be(2);
        set.HasBuffer(new EntityId(0, 1, 0)).Should().BeTrue();
        set.HasBuffer(new EntityId(1, 1, 0)).Should().BeTrue();
    }

    [Fact]
    public void AddBuffer_AddTwiceTheSameEntity_ShouldReturnExisting()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var buffer = set.AddBuffer(new EntityId(3, 1, 0));
        buffer.AddLast(new DynamicDamage { Value = 42 });

        var buffer2 = set.AddBuffer(new EntityId(3, 1, 0));

        buffer2.Count.Should().Be(1);
        buffer2.Components[0].Value.Should().Be(42);
        set.BufferCount.Should().Be(1);
    }

    [Fact]
    public void AddBuffer_ExceedInitialCapacity_ShouldAutoResizeBuffer()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        var buffer = set.AddBuffer(new EntityId(1, 1, 0));

        buffer.AddLast(new DynamicDamage { Value = 1 });
        buffer.AddLast(new DynamicDamage { Value = 2 });
        buffer.AddLast(new DynamicDamage { Value = 3 }); // exceeds initial capacity of 2

        buffer.Count.Should().Be(3);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(2);
        buffer.Components[2].Value.Should().Be(3);
    }

    [Fact]
    public void GetBuffer_ComponentDoesntExist_ShouldThrow()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        set.AddBuffer(new EntityId(0, 1, 0));

        Action action = () => set.GetBuffer(new EntityId(1, 1, 0));

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void GetBuffer_ComponentExists_ShouldGet()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);
        set.AddBuffer(entityId).AddLast(new DynamicDamage { Value = 99 });

        var buffer = set.GetBuffer(entityId);

        buffer.Count.Should().Be(1);
        buffer.Components[0].Value.Should().Be(99);
    }

    [Fact]
    public void TryGetBuffer_Missing_ShouldReturnFalse()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);

        var result = set.TryGetBuffer(new EntityId(5, 1, 0), out _);

        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetBuffer_Existing_ShouldReturnTrue()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);
        set.AddBuffer(entityId).AddLast(new DynamicDamage { Value = 99 });

        var result = set.TryGetBuffer(entityId, out var buffer);

        result.Should().BeTrue();
        buffer.Count.Should().Be(1);
        buffer.Components[0].Value.Should().Be(99);
    }

    [Fact]
    public void DeleteBuffer_ComponentDoesNotExist_ShouldBeOk()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);

        var action = () => set.DeleteBuffer(new EntityId(3, 1, 0));

        action.Should().NotThrow();
    }

    [Fact]
    public void DeleteBuffer_ComponentExists_ShouldBeOk()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);
        set.AddBuffer(entityId);
        set.DeleteBuffer(entityId);

        set.HasBuffer(entityId).Should().BeFalse();
    }

    [Fact]
    public void DeleteBuffer_SwapAndPop_ShouldKeepRemainingBufferData()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);

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
    public void DeleteBuffer_TwoBuffersDeleteSecond_ShouldKeepFirst()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicDamage { Value = 42 });
        set.AddBuffer(second).AddLast(new DynamicDamage { Value = 69 });

        set.DeleteBuffer(second);

        set.HasBuffer(first).Should().BeTrue();
        set.HasBuffer(second).Should().BeFalse();
        set.GetBuffer(first).Components[0].Value.Should().Be(42);
    }

    [Fact]
    public void DeleteBuffer_ThreeBuffersDeleteMiddle_ShouldKeepFirstAndThird()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 3, 4, _resizeStrategy, _stubEntityManager);
        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);
        var third = new EntityId(7, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicDamage { Value = 42 });
        set.AddBuffer(second).AddLast(new DynamicDamage { Value = 69 });
        set.AddBuffer(third).AddLast(new DynamicDamage { Value = 1024 });

        set.DeleteBuffer(second);

        set.HasBuffer(first).Should().BeTrue();
        set.HasBuffer(second).Should().BeFalse();
        set.HasBuffer(third).Should().BeTrue();
        set.GetBuffer(first).Components[0].Value.Should().Be(42);
        set.GetBuffer(third).Components[0].Value.Should().Be(1024);
    }

    [Fact]
    public void AddAfterDelete_ShouldReturnEmptyBuffer()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(0, 1, 0);

        set.AddBuffer(entityId).AddLast(new DynamicDamage { Value = 100 });
        set.DeleteBuffer(entityId);

        var readded = set.AddBuffer(entityId);

        readded.Count.Should().Be(0);
    }

    [Fact]
    public void AddAfterDeleteFirst_ShouldBeOk()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicDamage { Value = 42 });
        set.AddBuffer(second).AddLast(new DynamicDamage { Value = 69 });

        set.DeleteBuffer(first);

        set.AddBuffer(first).AddLast(new DynamicDamage { Value = 42 });

        set.BufferCount.Should().Be(2);
        set.GetBuffer(first).Count.Should().Be(1);
        set.GetBuffer(first).Components[0].Value.Should().Be(42);
        set.GetBuffer(second).Components[0].Value.Should().Be(69);
    }

    [Fact]
    public void Clear_ShouldDeleteAllBuffers()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);

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
    public void Entities_TwoBuffersExist_ShouldBeTwo()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        set.AddBuffer(new EntityId(3, 1, 0));
        set.AddBuffer(new EntityId(5, 1, 0));

        set.Entities.Length.Should().Be(2);
        set.Entities[0].Index.Should().Be(3);
        set.Entities[1].Index.Should().Be(5);
    }

    [Fact]
    public void Entities_EmptySet_ShouldBeEmpty()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);

        set.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void SlotExpansion_ExceedInitialSlotCount_ShouldResize()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 1, 4, _resizeStrategy, _stubEntityManager);

        for (var i = 0; i < 5; i++)
        {
            set.AddBuffer(new EntityId(i, 1, 0)).AddLast(new DynamicDamage { Value = i });
        }

        set.BufferCount.Should().Be(5);
        for (var i = 0; i < 5; i++)
        {
            set.GetBuffer(new EntityId(i, 1, 0)).Components[0].Value.Should().Be(i);
        }
    }

    // Stale-index tests: after DeleteBuffer the sparse-page entry is set to StaleDenseIndex (-1).
    // All operations must behave correctly when the page exists but holds a stale value.

    [Fact]
    public void Stale_HasBuffer_ShouldBeFalse()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        set.AddBuffer(entityId);
        set.DeleteBuffer(entityId); // page exists, value = -1 (stale)

        set.HasBuffer(entityId).Should().BeFalse();
    }

    [Fact]
    public void Stale_TryGetBuffer_ShouldReturnFalse()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        set.AddBuffer(entityId).AddLast(new DynamicDamage { Value = 42 });
        set.DeleteBuffer(entityId); // stale

        var result = set.TryGetBuffer(entityId, out _);

        result.Should().BeFalse();
    }

    [Fact]
    public void Stale_GetBuffer_ShouldThrow()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        set.AddBuffer(entityId).AddLast(new DynamicDamage { Value = 42 });
        set.DeleteBuffer(entityId); // stale

        Action action = () => set.GetBuffer(entityId);

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void Stale_AddBuffer_ShouldReturnFreshEmptyBuffer()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        set.AddBuffer(entityId).AddLast(new DynamicDamage { Value = 42 });
        set.DeleteBuffer(entityId); // stale

        var buffer = set.AddBuffer(entityId); // re-add through stale slot

        buffer.Count.Should().Be(0);
        set.HasBuffer(entityId).Should().BeTrue();
        set.BufferCount.Should().Be(1);
    }

    [Fact]
    public void Stale_AddBuffer_DataShouldBeIndependentOfPreviousContent()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        set.AddBuffer(entityId).AddLast(new DynamicDamage { Value = 99 });
        set.DeleteBuffer(entityId); // stale

        var buffer = set.AddBuffer(entityId);
        buffer.AddLast(new DynamicDamage { Value = 7 });

        buffer.Count.Should().Be(1);
        buffer.Components[0].Value.Should().Be(7);
    }

    [Fact]
    public void Stale_DeleteBuffer_ShouldBeNoOp()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        set.AddBuffer(entityId);
        set.DeleteBuffer(entityId); // stale

        var action = () => set.DeleteBuffer(entityId); // delete on stale slot

        action.Should().NotThrow();
        set.BufferCount.Should().Be(0);
    }

    [Fact]
    public void Stale_MultipleEntities_OnlyDeletedAreStale()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 3, 4, _resizeStrategy, _stubEntityManager);
        var entityId1 = new EntityId(3, 1, 0);
        var entityId2 = new EntityId(5, 1, 0);

        set.AddBuffer(entityId1).AddLast(new DynamicDamage { Value = 10 });
        set.AddBuffer(entityId2).AddLast(new DynamicDamage { Value = 20 });
        set.DeleteBuffer(entityId1); // entity1 stale, entity2 still valid

        set.HasBuffer(entityId1).Should().BeFalse();
        set.HasBuffer(entityId2).Should().BeTrue();
        set.GetBuffer(entityId2).Components[0].Value.Should().Be(20);
    }

    [Fact]
    public void Stale_ReAddAfterSwapAndPop_ShouldWork()
    {
        // After a swap-and-pop delete, the deleted entity gets StaleDenseIndex.
        // The swapped-in entity gets the freed dense slot.
        // Re-adding the deleted entity must work correctly.
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 3, 4, _resizeStrategy, _stubEntityManager);
        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        set.AddBuffer(first).AddLast(new DynamicDamage { Value = 1 });
        set.AddBuffer(second).AddLast(new DynamicDamage { Value = 2 });

        set.DeleteBuffer(first); // swap-and-pop: second takes first's slot, first goes stale

        set.HasBuffer(second).Should().BeTrue();
        set.GetBuffer(second).Components[0].Value.Should().Be(2);

        set.AddBuffer(first).AddLast(new DynamicDamage { Value = 3 }); // re-add stale entity

        set.HasBuffer(first).Should().BeTrue();
        set.GetBuffer(first).Components[0].Value.Should().Be(3);
        set.BufferCount.Should().Be(2);
    }

    [Fact]
    public void Handle_AfterDelete_AddLast_ShouldThrow()
    {
        var set = new CompactDynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        var handle = set.AddBuffer(entityId);
        set.DeleteBuffer(entityId); // invalidates the underlying DynamicBufferInstance

        var threw = false;
        try { handle.AddLast(new DynamicDamage { Value = 1 }); }
        catch { threw = true; }

        threw.Should().BeTrue();
    }
}
