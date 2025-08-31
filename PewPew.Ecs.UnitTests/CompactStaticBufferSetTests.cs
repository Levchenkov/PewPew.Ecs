using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Exceptions;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.UnitTests;

public class CompactStaticBufferSetTests
{
    private readonly DoubleSizeStrategy _resizeStrategy = new(10);
    private readonly StubEntityManager _stubEntityManager = new();

    [Fact]
    public void HasBuffer_EmptySet_ShouldBeFalse()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        var result = staticBufferSet.HasBuffer(new EntityId(1, 1, 0));

        result.Should().BeFalse();
    }

    [Fact]
    public void HasBuffer_ComponentDoesntExist_ShouldBeFalse()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var result = staticBufferSet.HasBuffer(new EntityId(2, 1, 0));

        result.Should().BeFalse();
    }

    [Fact]
    public void HasBuffer_ComponentExists_ShouldBeTrue()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));

        var result = staticBufferSet.HasBuffer(new EntityId(1, 1, 0));

        result.Should().BeTrue();
    }

    [Fact]
    public void AddBuffer_AddMoreThanMaxComponentPerType_ShouldThrow()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        staticBufferSet.AddBuffer(new EntityId(0, 1, 0));
        staticBufferSet.AddBuffer(new EntityId(1, 1, 0));
        Action action = () => staticBufferSet.AddBuffer(new EntityId(2, 1, 0));
        action.Should().NotThrow();

        staticBufferSet.BufferCount.Should().Be(3);
    }

    [Fact]
    public void GetBuffer_OneComponentExists_ShouldGet()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        staticBufferSet.AddBuffer(new EntityId(3, 1, 0));

        var buffer = staticBufferSet.GetBuffer(new EntityId(3, 1, 0));
        buffer.Capacity.Should().Be(10);
    }

    [Fact]
    public void GetBuffer_TwoComponentsExist_ShouldGet()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        staticBufferSet.AddBuffer(new EntityId(3, 1, 0));
        staticBufferSet.AddBuffer(new EntityId(5, 1, 0));

        var buffer = staticBufferSet.GetBuffer(new EntityId(3, 1, 0));
        buffer.Capacity.Should().Be(10);

        var buffer2 = staticBufferSet.GetBuffer(new EntityId(5, 1, 0));
        buffer2.Capacity.Should().Be(10);
    }

    [Fact]
    public void GetBuffer_ComponentDoesntExist_ShouldThrow()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        staticBufferSet.AddBuffer(new EntityId(0, 1, 0));

        Action action = () => staticBufferSet.GetBuffer(new EntityId(1, 1, 0));

        action.Should().ThrowExactly<ComponentNotFoundException>();
    }

    [Fact]
    public void Entities_TwoComponentsExist_ShouldBeTwo()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        staticBufferSet.AddBuffer(new EntityId(3, 1, 0));
        staticBufferSet.AddBuffer(new EntityId(5, 1, 0));

        var entities = staticBufferSet.Entities;
        entities.Length.Should().Be(2);

        entities[0].Index.Should().Be(3);
        entities[1].Index.Should().Be(5);
    }

    [Fact]
    public void Entities_EmptySet_ShouldBeEmpty()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        staticBufferSet.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void AddBuffer_AddTwiceTheSameEntity_ShouldReturnExisting()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        var buffer = staticBufferSet.AddBuffer(new EntityId(3, 1, 0));
        buffer.AddLast(new Damage { Value = 42 });

        var buffer2 = staticBufferSet.AddBuffer(new EntityId(3, 1, 0));
        buffer2.Count.Should().Be(1);
        buffer2.Components[0].Value.Should().Be(42);
    }

    [Fact]
    public void DeleteBuffer_ComponentDoesNotExist_ShouldBeOk()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        staticBufferSet.DeleteBuffer(new EntityId(3, 1, 0));
    }

    [Fact]
    public void DeleteBuffer_ComponentExists_ShouldBeOk()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        var entityId = new EntityId(3, 1, 0);
        staticBufferSet.AddBuffer(entityId);
        staticBufferSet.DeleteBuffer(entityId);

        staticBufferSet.HasBuffer(entityId).Should().BeFalse();
    }

    [Fact]
    public void DeleteBuffer_TwoComponentExistsDeleteFirst_ShouldBeOk()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        staticBufferSet.AddBuffer(first).AddLast(new Damage {Value = 42 });
        staticBufferSet.AddBuffer(second).AddLast(new Damage {Value = 69 });

        staticBufferSet.DeleteBuffer(first);

        staticBufferSet.HasBuffer(first).Should().BeFalse();
        staticBufferSet.HasBuffer(second).Should().BeTrue();
        staticBufferSet.GetBuffer(second).RemoveLast().Value.Should().Be(69);
    }

    [Fact]
    public void DeleteComponent_TwoComponentExistsDeleteSecond_ShouldBeOk()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        staticBufferSet.AddBuffer(first).AddLast(new Damage {Value = 42 });
        staticBufferSet.AddBuffer(second).AddLast(new Damage {Value = 69 });

        staticBufferSet.DeleteBuffer(second);

        staticBufferSet.HasBuffer(first).Should().BeTrue();
        staticBufferSet.HasBuffer(second).Should().BeFalse();
        staticBufferSet.GetBuffer(first).RemoveLast().Value.Should().Be(42);
    }

    [Fact]
    public void DeleteBuffer_ThreeComponentExistsDeleteMiddle_ShouldBeOk()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);
        var third = new EntityId(7, 1, 0);

        staticBufferSet.AddBuffer(first).AddLast(new Damage {Value = 42 });
        staticBufferSet.AddBuffer(second).AddLast(new Damage {Value = 69 });
        staticBufferSet.AddBuffer(third).AddLast(new Damage {Value = 1024 });

        staticBufferSet.DeleteBuffer(second);

        staticBufferSet.HasBuffer(first).Should().BeTrue();
        staticBufferSet.HasBuffer(second).Should().BeFalse();
        staticBufferSet.HasBuffer(third).Should().BeTrue();
        staticBufferSet.GetBuffer(first).RemoveLast().Value.Should().Be(42);
        staticBufferSet.GetBuffer(third).RemoveLast().Value.Should().Be(1024);
    }

    [Fact]
    public void AddComponent_AddAfterDeletingFirst_ShouldBeOk()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        staticBufferSet.AddBuffer(first).AddLast(new Damage {Value = 42 });
        staticBufferSet.AddBuffer(second).AddLast(new Damage {Value = 69 });

        staticBufferSet.DeleteBuffer(first);

        staticBufferSet.HasBuffer(first).Should().BeFalse();
        staticBufferSet.HasBuffer(second).Should().BeTrue();

        staticBufferSet.AddBuffer(first).AddLast(new Damage {Value = 42 });
        staticBufferSet.BufferCount.Should().Be(2);

        var firstBuffer = staticBufferSet.GetBuffer(first);
        firstBuffer.Count.Should().Be(1);
        firstBuffer.Components[0].Value.Should().Be(42);

        var secondBuffer = staticBufferSet.GetBuffer(second);
        secondBuffer.Count.Should().Be(1);
        secondBuffer.Components[0].Value.Should().Be(69);
    }

    [Fact]
    public void AddComponent_AddAfterDeletingLast_ShouldBeOk()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        staticBufferSet.AddBuffer(first).AddLast(new Damage {Value = 42 });
        staticBufferSet.AddBuffer(second).AddLast(new Damage {Value = 69 });

        staticBufferSet.DeleteBuffer(second);

        staticBufferSet.HasBuffer(first).Should().BeTrue();
        staticBufferSet.HasBuffer(second).Should().BeFalse();

        staticBufferSet.AddBuffer(second).AddLast(new Damage {Value = 69 });
        staticBufferSet.BufferCount.Should().Be(2);

        var firstBuffer = staticBufferSet.GetBuffer(first);
        firstBuffer.Count.Should().Be(1);
        firstBuffer.Components[0].Value.Should().Be(42);

        var secondBuffer = staticBufferSet.GetBuffer(second);
        secondBuffer.Count.Should().Be(1);
        secondBuffer.Components[0].Value.Should().Be(69);
    }

    [Fact]
    public void Clear()
    {
        var staticBufferSet = new CompactStaticBufferSet<Damage>(10, 2, 10, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        staticBufferSet.AddBuffer(first).AddLast(new Damage {Value = 42 });
        staticBufferSet.AddBuffer(second).AddLast(new Damage {Value = 69 });

        staticBufferSet.Clear();

        staticBufferSet.BufferCount.Should().Be(0);

        staticBufferSet.HasBuffer(first).Should().BeFalse();
        staticBufferSet.HasBuffer(second).Should().BeFalse();

        var bufferRef = staticBufferSet.AddBuffer(first);
        bufferRef.Count.Should().Be(0);
    }
}