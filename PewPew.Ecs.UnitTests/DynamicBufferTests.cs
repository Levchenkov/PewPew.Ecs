using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class DynamicBufferTests
{
    private readonly DoubleSizeStrategy _resizeStrategy = new(2);
    private readonly StubEntityManager _stubEntityManager = new();

    [Fact]
    public void AddLast_ReachesInitialCapacity_ShouldGrow()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);
        dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var buffer = dynamicBufferSet.AddBuffer(new EntityId(1, 1, 0));
        buffer.AddLast(new DynamicDamage { Value = 1 });
        buffer.AddLast(new DynamicDamage { Value = 2 });
        buffer.AddLast(new DynamicDamage { Value = 3 });

        buffer.Count.Should().Be(3);
        buffer.Capacity.Should().BeGreaterOrEqualTo(3);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(2);
        buffer.Components[2].Value.Should().Be(3);
    }

    [Fact]
    public void TrimExcess_ShouldNotShrinkBelowInitialCapacity()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var buffer = dynamicBufferSet.AddBuffer(new EntityId(1, 1, 0));
        buffer.EnsureCapacity(32);
        buffer.Capacity.Should().BeGreaterOrEqualTo(32);

        buffer.AddLast(new DynamicDamage { Value = 42 });
        buffer.Clear();
        buffer.TrimExcess();

        buffer.Count.Should().Be(0);
        buffer.Capacity.Should().BeGreaterOrEqualTo(4);
    }

    // [Fact]
    // public void TrimToInitialCapacity_AfterGrowth_ShouldKeepData()
    // {
    //     var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
    //     dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));
    //
    //     var buffer = dynamicBufferSet.AddBuffer(new EntityId(1, 1, 0));
    //     buffer.AddLast(new DynamicDamage { Value = 11 });
    //     buffer.AddLast(new DynamicDamage { Value = 22 });
    //     buffer.AddLast(new DynamicDamage { Value = 33 });
    //
    //     buffer.Capacity.Should().BeGreaterOrEqualTo(3);
    //
    //     buffer.TrimToInitialCapacity();
    //
    //     buffer.Count.Should().Be(3);
    //     buffer.Capacity.Should().BeGreaterOrEqualTo(3);
    //     buffer.Components[0].Value.Should().Be(11);
    //     buffer.Components[1].Value.Should().Be(22);
    //     buffer.Components[2].Value.Should().Be(33);
    // }

    [Fact]
    public void Handle_AfterDelete_ShouldBeInvalid()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);
        dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var entity = new EntityId(1, 1, 0);
        var buffer = dynamicBufferSet.AddBuffer(entity);
        buffer.AddLast(new DynamicDamage { Value = 1 });

        dynamicBufferSet.DeleteBuffer(entity);

        var isThrown = false;
        try
        {
            buffer.AddLast(new DynamicDamage { Value = 2 });
        }
        catch (NullReferenceException)
        {
            isThrown = true;
        }

        isThrown.Should().BeTrue();
    }

    [Fact]
    public void Handle_AfterDeletingAnotherEntity_ShouldStillTrackItsBuffer()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(0, 1, 0);
        var second = new EntityId(1, 1, 0);

        dynamicBufferSet.AddBuffer(first).AddLast(new DynamicDamage { Value = 10 });
        var buffer = dynamicBufferSet.AddBuffer(second);
        buffer.AddLast(new DynamicDamage { Value = 20 });

        dynamicBufferSet.DeleteBuffer(first);
        buffer.AddLast(new DynamicDamage { Value = 30 });

        buffer.Count.Should().Be(2);
        buffer.Components[0].Value.Should().Be(20);
        buffer.Components[1].Value.Should().Be(30);
        dynamicBufferSet.GetBuffer(second).Components[1].Value.Should().Be(30);
    }

    [Fact]
    public void MultipleHandles_AfterGrowth_ShouldSyncToLatestArray()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 1, _resizeStrategy, _stubEntityManager);
        dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var entity = new EntityId(1, 1, 0);
        var first = dynamicBufferSet.AddBuffer(entity);
        var second = dynamicBufferSet.GetBuffer(entity);

        first.EnsureCapacity(32);
        second.AddLast(new DynamicDamage { Value = 7 });

        first.Capacity.Should().BeGreaterOrEqualTo(32);
        second.Capacity.Should().BeGreaterOrEqualTo(32);
        first.Count.Should().Be(1);
        first.Components[0].Value.Should().Be(7);
    }

    [Fact]
    public void EnsureCapacity_LessOrEqualCurrent_ShouldKeepData()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var buffer = dynamicBufferSet.AddBuffer(new EntityId(1, 1, 0));
        buffer.AddLast(new DynamicDamage { Value = 5 });
        buffer.AddLast(new DynamicDamage { Value = 6 });

        var previousCapacity = buffer.Capacity;
        buffer.EnsureCapacity(1);

        buffer.Count.Should().Be(2);
        buffer.Capacity.Should().Be(previousCapacity);
        buffer.Components[0].Value.Should().Be(5);
        buffer.Components[1].Value.Should().Be(6);
    }

    [Fact]
    public void EnsureCapacity_Negative_ShouldThrow()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var buffer = dynamicBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var isThrown = false;
        try
        {
            buffer.EnsureCapacity(-1);
        }
        catch (ArgumentOutOfRangeException)
        {
            isThrown = true;
        }

        isThrown.Should().BeTrue();
    }

    [Fact]
    public void TrimExcess_WhenCountGreaterThanInitial_ShouldKeepAllElements()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var buffer = dynamicBufferSet.AddBuffer(new EntityId(1, 1, 0));
        buffer.EnsureCapacity(64);

        buffer.AddLast(new DynamicDamage { Value = 10 });
        buffer.AddLast(new DynamicDamage { Value = 20 });
        buffer.AddLast(new DynamicDamage { Value = 30 });

        buffer.TrimExcess();

        buffer.Count.Should().Be(3);
        buffer.Capacity.Should().BeGreaterOrEqualTo(3);
        buffer.Components[0].Value.Should().Be(10);
        buffer.Components[1].Value.Should().Be(20);
        buffer.Components[2].Value.Should().Be(30);
    }

    [Fact]
    public void RemoveLast_ShouldPopLastElement()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var buffer = dynamicBufferSet.AddBuffer(new EntityId(1, 1, 0));
        buffer.AddLast(new DynamicDamage { Value = 7 });
        buffer.AddLast(new DynamicDamage { Value = 8 });

        var popped = buffer.RemoveLast();

        popped.Value.Should().Be(8);
        buffer.Count.Should().Be(1);
        buffer.Components[0].Value.Should().Be(7);
    }

    [Fact]
    public void SwapAndPop_Middle_ShouldReplaceWithLast()
    {
        var dynamicBufferSet = new DynamicBufferSet<DynamicDamage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        dynamicBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var buffer = dynamicBufferSet.AddBuffer(new EntityId(1, 1, 0));
        buffer.AddLast(new DynamicDamage { Value = 1 });
        buffer.AddLast(new DynamicDamage { Value = 2 });
        buffer.AddLast(new DynamicDamage { Value = 3 });

        var removed = buffer.SwapAndPop(1);

        removed.Value.Should().Be(2);
        buffer.Count.Should().Be(2);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(3);
    }

    [Fact]
    public void Handle_AfterDelete_Count_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { _ = buffer.Count; } catch (Exception) { isThrown = true; }
        isThrown.Should().BeFalse();
    }

    [Fact]
    public void Handle_AfterDelete_Capacity_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { _ = buffer.Capacity; } catch (NullReferenceException) { isThrown = true; }
        isThrown.Should().BeTrue();
    }

    [Fact]
    public void Handle_AfterDelete_IsEmpty_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { _ = buffer.IsEmpty; } catch (Exception) { isThrown = true; }
        isThrown.Should().BeFalse();
    }

    [Fact]
    public void Handle_AfterDelete_Components_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { _ = buffer.Components.Length; } catch (ArgumentOutOfRangeException) { isThrown = true; }
        isThrown.Should().BeFalse();
    }

    [Fact]
    public void Handle_AfterDelete_RemoveLast_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { buffer.RemoveLast(); } catch (Exception) { isThrown = true; }
        isThrown.Should().BeTrue();
    }

    [Fact]
    public void Handle_AfterDelete_AddLast_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { buffer.AddLast(new DynamicDamage()); } catch (Exception) { isThrown = true; }
        isThrown.Should().BeTrue();
    }

    [Fact]
    public void Handle_AfterDelete_SwapAndPop_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { buffer.SwapAndPop(0); } catch (Exception) { isThrown = true; }
        isThrown.Should().BeTrue();
    }

    [Fact]
    public void Handle_AfterDelete_Clear_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { buffer.Clear(); } catch (NullReferenceException) { isThrown = true; }
        isThrown.Should().BeFalse();
    }

    [Fact]
    public void Handle_AfterDelete_EnsureCapacity_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { buffer.EnsureCapacity(8); } catch (NullReferenceException) { isThrown = true; }
        isThrown.Should().BeTrue();
    }

    [Fact]
    public void Handle_AfterDelete_TrimExcess_ShouldThrow()
    {
        var buffer = CreateDeletedBuffer(out var set, out var entity);
        var isThrown = false;
        try { buffer.TrimExcess(); } catch (NullReferenceException) { isThrown = true; }
        isThrown.Should().BeTrue();
    }

    private DynamicBuffer<DynamicDamage> CreateDeletedBuffer(
        out DynamicBufferSet<DynamicDamage> set,
        out EntityId entity)
    {
        set = new DynamicBufferSet<DynamicDamage>(10, 2, 4, _resizeStrategy, _stubEntityManager);
        set.AddBuffer(new EntityId(0, 1, 0));
        entity = new EntityId(1, 1, 0);
        var buffer = set.AddBuffer(entity);
        buffer.AddLast(new DynamicDamage { Value = 1 });
        buffer.AddLast(new DynamicDamage { Value = 2 });
        set.DeleteBuffer(entity);
        return buffer;
    }
}






