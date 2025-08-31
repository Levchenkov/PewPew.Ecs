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
}
