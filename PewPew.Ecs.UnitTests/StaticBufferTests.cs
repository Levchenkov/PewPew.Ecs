using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class StaticBufferTests
{
    private readonly DoubleSizeStrategy _resizeStrategy = new(10);
    private readonly StubEntityManager _stubEntityManager = new();

    [Fact]
    public void EmptyBuffer_ShouldBeEmpty()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var buffer = staticBufferSet.AddBuffer(new EntityId(2, 1, 0));

        buffer.Count.Should().Be(0);
        buffer.Capacity.Should().Be(10);
        buffer.IsEmpty.Should().BeTrue();
        buffer.IsFull.Should().BeFalse();
    }

    [Fact]
    public void AddLast_ShouldBeNonEmpty()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var buffer = staticBufferSet.AddBuffer(new EntityId(2, 1, 0));

        buffer.AddLast(new Damage{Value = 1});
        buffer.AddLast(new Damage{Value = 2});

        buffer.Count.Should().Be(2);
        buffer.Capacity.Should().Be(10);
        buffer.IsEmpty.Should().BeFalse();
        buffer.IsFull.Should().BeFalse();

        buffer.Components.Length.Should().Be(2);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(2);
    }

    [Fact]
    public void RemoveLast_ShouldRemove()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var buffer = staticBufferSet.AddBuffer(new EntityId(2, 1, 0));

        buffer.AddLast(new Damage{Value = 1});
        buffer.AddLast(new Damage{Value = 2});

        buffer.RemoveLast().Value.Should().Be(2);

        buffer.Components.Length.Should().Be(1);
        buffer.Components[0].Value.Should().Be(1);
    }

    [Fact]
    public void Clear_ShouldBeEmpty()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var buffer = staticBufferSet.AddBuffer(new EntityId(2, 1, 0));

        buffer.AddLast(new Damage{Value = 1});
        buffer.AddLast(new Damage{Value = 2});

        buffer.Clear();

        buffer.Count.Should().Be(0);
        buffer.Capacity.Should().Be(10);
        buffer.IsEmpty.Should().BeTrue();
        buffer.IsFull.Should().BeFalse();
    }

    [Fact]
    public void SwapAndPop_First_LastShouldBecomeFirst()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var buffer = staticBufferSet.AddBuffer(new EntityId(2, 1, 0));

        buffer.AddLast(new Damage{Value = 1});
        buffer.AddLast(new Damage{Value = 2});
        buffer.AddLast(new Damage{Value = 3});

        buffer.SwapAndPop(0).Value.Should().Be(1);

        buffer.Components.Length.Should().Be(2);
        buffer.Components[0].Value.Should().Be(3);
        buffer.Components[1].Value.Should().Be(2);
    }

    [Fact]
    public void SwapAndPop_Middle_LastShouldBecomeMiddle()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var buffer = staticBufferSet.AddBuffer(new EntityId(2, 1, 0));

        buffer.AddLast(new Damage{Value = 1});
        buffer.AddLast(new Damage{Value = 2});
        buffer.AddLast(new Damage{Value = 3});

        buffer.SwapAndPop(1).Value.Should().Be(2);

        buffer.Components.Length.Should().Be(2);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(3);
    }

    [Fact]
    public void SwapAndPop_Last_LastShouldBePopped()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var buffer = staticBufferSet.AddBuffer(new EntityId(2, 1, 0));

        buffer.AddLast(new Damage{Value = 1});
        buffer.AddLast(new Damage{Value = 2});
        buffer.AddLast(new Damage{Value = 3});

        buffer.SwapAndPop(2).Value.Should().Be(3);

        buffer.Components.Length.Should().Be(2);
        buffer.Components[0].Value.Should().Be(1);
        buffer.Components[1].Value.Should().Be(2);
    }

    [Fact]
    public void Handle_AfterDeletingAnotherEntity_ShouldStillTrackItsBuffer()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 2, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(0, 1, 0);
        var second = new EntityId(1, 1, 0);

        staticBufferSet.AddBuffer(first).AddLast(new Damage { Value = 10 });
        var buffer = staticBufferSet.AddBuffer(second);
        buffer.AddLast(new Damage { Value = 20 });

        staticBufferSet.DeleteBuffer(first);
        buffer.AddLast(new Damage { Value = 30 });

        buffer.Count.Should().Be(2);
        buffer.Components[0].Value.Should().Be(20);
        buffer.Components[1].Value.Should().Be(30);
        staticBufferSet.GetBuffer(second).Components[1].Value.Should().Be(30);
    }

    [Fact]
    public void Handle_AfterDeletingAnotherEntity_ShouldStillTrackItsBuffer2()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 2, _resizeStrategy, _stubEntityManager);

        var entityId = new EntityId(0, 1, 0);

        var staticBuffer = staticBufferSet.AddBuffer(entityId);
        staticBuffer.AddLast(new Damage { Value = 10 });

        var components = staticBuffer.Components;

        components.Length.Should().Be(1);

        staticBuffer.AddLast(new Damage { Value = 20 });

        components.Length.Should().Be(1);

        staticBufferSet.DeleteBuffer(entityId);

        components.Length.Should().Be(1);
    }

    [DebugOnlyFact]
    public void Handle_AfterDelete_ShouldBeInvalid()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var entity = new EntityId(1, 1, 0);
        var buffer = staticBufferSet.AddBuffer(entity);
        buffer.AddLast(new Damage { Value = 1 });

        staticBufferSet.DeleteBuffer(entity);

        var isThrown = false;
        try
        {
            buffer.AddLast(new Damage { Value = 2 });
        }
        catch (NotSupportedException)
        {
            isThrown = true;
        }

        isThrown.Should().BeTrue();
    }

    [DebugOnlyFact]
    public void Handle_AfterDelete_RemoveLast_ShouldThrow()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var entity = new EntityId(1, 1, 0);
        var buffer = staticBufferSet.AddBuffer(entity);
        buffer.AddLast(new Damage { Value = 1 });

        staticBufferSet.DeleteBuffer(entity);

        var isThrown = false;
        try
        {
            buffer.RemoveLast();
        }
        catch (NotSupportedException)
        {
            isThrown = true;
        }

        isThrown.Should().BeTrue();
    }

    [DebugOnlyFact]
    public void Handle_AfterDelete_SwapAndPop_ShouldThrow()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var entity = new EntityId(1, 1, 0);
        var buffer = staticBufferSet.AddBuffer(entity);
        buffer.AddLast(new Damage { Value = 1 });
        buffer.AddLast(new Damage { Value = 2 });

        staticBufferSet.DeleteBuffer(entity);

        var isThrown = false;
        try
        {
            buffer.SwapAndPop(0);
        }
        catch (NotSupportedException)
        {
            isThrown = true;
        }

        isThrown.Should().BeTrue();
    }

    [DebugOnlyFact]
    public void Handle_AfterDelete_Components_ShouldThrow()
    {
        var staticBufferSet = new StaticBufferSet<Damage>(10, 2, 2, _resizeStrategy, _stubEntityManager);
        staticBufferSet.AddBuffer(new EntityId(0, 1, 0));

        var entity = new EntityId(1, 1, 0);
        var buffer = staticBufferSet.AddBuffer(entity);
        buffer.AddLast(new Damage { Value = 1 });

        staticBufferSet.DeleteBuffer(entity);

        var isThrown = false;
        try
        {
            _ = buffer.Components;
        }
        catch (NotSupportedException)
        {
            isThrown = true;
        }

        isThrown.Should().BeTrue();
    }
}
