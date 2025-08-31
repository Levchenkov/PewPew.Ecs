using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class FilterTests
{
    [Fact]
    public void GetFilter_EmptyFilter_EntitiesShouldBeEmpty()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filter = world.GetFilter(new FilterDefinition().With<Component1>().With<Component2>());

        filter.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void GetFilter_ComponentsIsNotInited_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        var filterDefinition = new FilterDefinition().With<Component1>().With<Component2>();
        var action = () =>
        {
            var filter = world.GetFilter(filterDefinition);
        };

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void GetFilter_CreateFilterBeforeEntitiesWereAdded_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filter = world.GetFilter(new FilterDefinition().With<Component1>().With<Component2>());

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1);
        world.AddComponent<Component2>(entityId1);
        world.AddComponent<Component2>(entityId1); // duplicate call

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2);
        world.AddComponent<Component2>(entityId2);
        world.AddComponent<Component1>(entityId2); // duplicate call

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();
    }

    [Fact]
    public void GetFilter_CreateFilterBeforeAndAfterEntitiesWereAdded_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1);
        world.AddComponent<Component2>(entityId1);

        var filter = world.GetFilter(new FilterDefinition().With<Component1>().With<Component2>());

        filter.Entities.Length.Should().Be(1);
        filter.Entities[0].Should().Be(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2);
        world.AddComponent<Component2>(entityId2);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();
    }

    [Fact]
    public void GetFilter_CreateFilterAfterEntitiesWereAdded_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1);
        world.AddComponent<Component2>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2);
        world.AddComponent<Component2>(entityId2);

        var filter = world.GetFilter(new FilterDefinition().With<Component1>().With<Component2>());

        world.AddComponent<Component1>(entityId1); // duplicate call
        world.AddComponent<Component2>(entityId2); // duplicate call

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();
    }

    [Fact]
    public void GetFilter_DeleteLastComponent_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1);
        world.AddComponent<Component2>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2);
        world.AddComponent<Component2>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3);
        world.AddComponent<Component2>(entityId3);

        var filter = world.GetFilter(new FilterDefinition().With<Component1>().With<Component2>());

        world.DeleteComponent<Component2>(entityId3);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeFalse();
    }

    [Fact]
    public void GetFilter_DeleteMiddleComponent_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1);
        world.AddComponent<Component2>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2);
        world.AddComponent<Component2>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3);
        world.AddComponent<Component2>(entityId3);

        var filter = world.GetFilter(new FilterDefinition().With<Component1>().With<Component2>());

        world.DeleteComponent<Component2>(entityId2);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId3);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeFalse();
        filter.HasEntity(entityId3).Should().BeTrue();
    }

    [Fact]
    public void GetFilter_DeleteFirstComponent_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1);
        world.AddComponent<Component2>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2);
        world.AddComponent<Component2>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3);
        world.AddComponent<Component2>(entityId3);

        var filter = world.GetFilter(new FilterDefinition().With<Component1>().With<Component2>());

        world.DeleteComponent<Component2>(entityId1);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId3); // deletion swaps with last entity
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeFalse();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeTrue();
    }

    [Fact]
    public void GetFilter_DeleteComponentNotFromFilter_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2);
        world.AddComponent<Component2>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3);

        var filter = world.GetFilter(new FilterDefinition().With<Component1>().With<Component2>());

        world.DeleteComponent<Component1>(entityId1);
        world.DeleteComponent<Component1>(entityId3);

        filter.Entities.Length.Should().Be(1);
        filter.Entities[0].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeFalse();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeFalse();
    }
}