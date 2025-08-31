using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class EntityIdPoolTests
{

    [Fact]
    public void Ctor_MaxEntitiesCountIsNegative_ShouldThrow()
    {
        Action action = () =>
        {
            var entityIdPool = new EntityIdPool(0, -1);
        };

        action.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Ctor_MaxEntitiesCountIs0_ShouldNotThrow()
    {
        Action action = () =>
        {
            var entityIdPool = new EntityIdPool(0, 0);
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void Capacity_MaxEntitiesCountIsPositive_ShouldBeEqual()
    {
        var entityIdPool = new EntityIdPool(0, 42);

        entityIdPool.Capacity.Should().Be(42);
    }

    [Fact]
    public void Get_GetMoreThenCapacity_ShouldThrow()
    {
        var entityIdPool = new EntityIdPool(0, 1);

        Action action1 = () =>
        {
            entityIdPool.Get();
        };

        action1.Should().NotThrow();

        Action action2 = () =>
        {
            entityIdPool.Get();
        };

        action2.Should().ThrowExactly<NotSupportedException>();
    }

    [Fact]
    public void Get_EnoughCapacity_IndexesShouldBeASequence()
    {
        var entityIdPool = new EntityIdPool(0, 3);

        var entityId1 = entityIdPool.Get();
        var entityId2 = entityIdPool.Get();
        var entityId3 = entityIdPool.Get();

        entityId1.Index.Should().Be(0);
        entityId2.Index.Should().Be(1);
        entityId3.Index.Should().Be(2);
    }

    [Fact]
    public void Get_ReturnAndGet_ShouldBeTheSameIndex()
    {
        var entityIdPool = new EntityIdPool(0, 1);

        var entityId = entityIdPool.Get();
        entityId.Index.Should().Be(0);

        entityIdPool.Return(entityId);

        entityId = entityIdPool.Get();
        entityId.Index.Should().Be(0);
    }

    [Fact]
    public void Get_ReturnFirstAndGet_ShouldBeTheSameIndex()
    {
        var entityIdPool = new EntityIdPool(0, 2);

        var entityId1 = entityIdPool.Get();
        var entityId2 = entityIdPool.Get();
        entityId1.Index.Should().Be(0);
        entityId2.Index.Should().Be(1);

        entityIdPool.Return(entityId1);

        entityId1 = entityIdPool.Get();
        entityId1.Index.Should().Be(0);
        entityId2.Index.Should().Be(1);
    }

    [Fact]
    public void Get_ReturnSecondAndGet_ShouldBeTheSameIndex()
    {
        var entityIdPool = new EntityIdPool(0, 2);

        var entityId1 = entityIdPool.Get();
        var entityId2 = entityIdPool.Get();
        entityId1.Index.Should().Be(0);
        entityId2.Index.Should().Be(1);

        entityIdPool.Return(entityId2);

        entityId2 = entityIdPool.Get();
        entityId1.Index.Should().Be(0);
        entityId2.Index.Should().Be(1);
    }

    [Fact]
    public void Get_ReturnTwiceAndGetTwice_ShouldBeTheSameIndex()
    {
        var entityIdPool = new EntityIdPool(0, 2);

        var entityId1 = entityIdPool.Get();
        var entityId2 = entityIdPool.Get();
        entityId1.Index.Should().Be(0);
        entityId2.Index.Should().Be(1);

        entityIdPool.Return(entityId1);
        entityIdPool.Return(entityId2);

        entityId2 = entityIdPool.Get();
        entityId1 = entityIdPool.Get();

        entityId1.Index.Should().Be(0);
        entityId2.Index.Should().Be(1);
    }

    [Fact]
    public void Get_ReturnTwiceAndGetTwiceRevers_ShouldBeTheSameIndex()
    {
        var entityIdPool = new EntityIdPool(0, 2);

        var entityId1 = entityIdPool.Get();
        var entityId2 = entityIdPool.Get();
        entityId1.Index.Should().Be(0);
        entityId2.Index.Should().Be(1);

        entityIdPool.Return(entityId2);
        entityIdPool.Return(entityId1);

        entityId1 = entityIdPool.Get();
        entityId2 = entityIdPool.Get();

        entityId1.Index.Should().Be(0);
        entityId2.Index.Should().Be(1);
    }

    [Fact]
    public void Get_ReturnToWrongPool_ShouldThrow()
    {
        var entityIdPool1 = new EntityIdPool(0, 1);
        var entityIdPool2 = new EntityIdPool(1, 1);

        var entityId1 = entityIdPool1.Get();

        var action = () =>
        {
            entityIdPool2.Return(entityId1);
        };

        action.Should().ThrowExactly<NotSupportedException>();
    }

    [Fact]
    public void Get_ReturnDeadEntityId_ShouldBeOk()
    {
        var entityIdPool = new EntityIdPool(0, 1);

        var entityId = entityIdPool.Get();

        entityIdPool.Return(entityId);

        var action = () => { entityIdPool.Return(entityId); };

        action.Should().NotThrow();
    }

    [Fact]
    public void IsAlive_AliveEntityId_ShouldBeTrue()
    {
        var entityIdPool = new EntityIdPool(0, 1);

        var entityId = entityIdPool.Get();

        entityIdPool.IsAlive(entityId).Should().BeTrue();
    }

    [Fact]
    public void IsAlive_DeadEntityId_ShouldBeFalse()
    {
        var entityIdPool = new EntityIdPool(0, 1);

        var entityId = entityIdPool.Get();
        entityIdPool.Return(entityId);

        entityIdPool.IsAlive(entityId).Should().BeFalse();
    }

    [Fact]
    public void IsAlive_WrongWorld_ShouldBeFalse()
    {
        var entityIdPool1 = new EntityIdPool(0, 1);
        var entityIdPool2 = new EntityIdPool(1, 1);

        var entityId = entityIdPool1.Get();

        var action = () => entityIdPool2.IsAlive(entityId);

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Get_GenerationOverflow_ShouldBeInitial()
    {
        var entityIdPool = new EntityIdPool(0, 1);

        for (ushort i = 0; i < ushort.MaxValue; i++)
        {
            var entityId = entityIdPool.Get();

            entityId.Generation.Should().Be((ushort)(i + 1));

            entityIdPool.Return(entityId);
        }

        var nextEntityId = entityIdPool.Get();
        nextEntityId.Generation.Should().Be(IdPool<EntityId>.InitialGen);
    }
}