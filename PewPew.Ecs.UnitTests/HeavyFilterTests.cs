using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class HeavyFilterTests
{
    [Fact]
    public void FetchHeavyFilter_EmptyFilter_EntitiesShouldBeEmpty()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        filter.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void FetchHeavyFilter_ComponentsIsNotInited_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        var filterDefinition = new HeavyFilterDefinition<Component1, Component2>();
        var action = () =>
        {
            var filter = world.FetchHeavyFilter(filterDefinition);
        };

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void FetchHeavyFilter_CreateFilterBeforeEntitiesWereAdded_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddComponent<Component2>(entityId1).Value = 12;
        world.AddComponent<Component2>(entityId1); // duplicate call

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddComponent<Component2>(entityId2).Value = 22;
        world.AddComponent<Component1>(entityId2); // duplicate call

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component2>(0).Value.Should().Be(12);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
        filter.GetComponent<Component2>(1).Value.Should().Be(22);
    }

    [Fact]
    public void FetchHeavyFilter_CreateFilterBeforeAndAfterEntitiesWereAdded_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddComponent<Component2>(entityId1).Value = 12;

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        filter.Entities.Length.Should().Be(1);
        filter.Entities[0].Should().Be(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddComponent<Component2>(entityId2).Value = 22;

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component2>(0).Value.Should().Be(12);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
        filter.GetComponent<Component2>(1).Value.Should().Be(22);
    }

    [Fact]
    public void FetchHeavyFilter_CreateFilterAfterEntitiesWereAdded_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddComponent<Component2>(entityId1).Value = 12;

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddComponent<Component2>(entityId2).Value = 22;

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        world.AddComponent<Component1>(entityId1); // duplicate call
        world.AddComponent<Component2>(entityId2); // duplicate call

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component2>(0).Value.Should().Be(12);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
        filter.GetComponent<Component2>(1).Value.Should().Be(22);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteLastComponent_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddComponent<Component2>(entityId1).Value = 12;

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddComponent<Component2>(entityId2).Value = 22;

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;
        world.AddComponent<Component2>(entityId3).Value = 32;

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        world.DeleteComponent<Component2>(entityId3);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeFalse();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component2>(0).Value.Should().Be(12);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
        filter.GetComponent<Component2>(1).Value.Should().Be(22);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteMiddleComponent_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddComponent<Component2>(entityId1).Value = 12;

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddComponent<Component2>(entityId2).Value = 22;

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;
        world.AddComponent<Component2>(entityId3).Value = 32;

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        world.DeleteComponent<Component2>(entityId2);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId3);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeFalse();
        filter.HasEntity(entityId3).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component2>(0).Value.Should().Be(12);
        filter.GetComponent<Component1>(1).Value.Should().Be(31);
        filter.GetComponent<Component2>(1).Value.Should().Be(32);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteFirstComponent_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddComponent<Component2>(entityId1).Value = 12;

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddComponent<Component2>(entityId2).Value = 22;

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;
        world.AddComponent<Component2>(entityId3).Value = 32;

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        world.DeleteComponent<Component2>(entityId1);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId3); // deletion swaps with last entity
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeFalse();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(31);
        filter.GetComponent<Component2>(0).Value.Should().Be(32);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
        filter.GetComponent<Component2>(1).Value.Should().Be(22);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteComponentsNotFromFilter_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddComponent<Component2>(entityId2).Value = 22;

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        world.DeleteComponent<Component1>(entityId1);
        world.DeleteComponent<Component1>(entityId3);

        filter.Entities.Length.Should().Be(1);
        filter.Entities[0].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeFalse();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeFalse();

        filter.GetComponent<Component1>(0).Value.Should().Be(21);
        filter.GetComponent<Component2>(0).Value.Should().Be(22);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteComponentsNotFromFilter2_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddComponent<Component2>(entityId1).Value = 12;

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;
        world.AddComponent<Component2>(entityId3).Value = 32;

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        world.DeleteComponent<Component1>(entityId2);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId3);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeFalse();
        filter.HasEntity(entityId3).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component2>(0).Value.Should().Be(12);
        filter.GetComponent<Component1>(1).Value.Should().Be(31);
        filter.GetComponent<Component2>(1).Value.Should().Be(32);

        world.DeleteComponent<Component1>(entityId1);

        filter.GetComponent<Component1>(0).Value.Should().Be(31);
        filter.GetComponent<Component2>(0).Value.Should().Be(32);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteAllComponentFromEntity_EntityShouldBeDeleteFromFilter()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddComponent<Component2>(entityId1).Value = 12;

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        world.DeleteComponent<Component1>(entityId1);
        world.DeleteComponent<Component2>(entityId1);

        filter.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void FetchHeavyFilter_GetWrongComponent_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddComponent<Component2>(entityId1).Value = 12;

        var action = () =>
        {
            var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

            filter.Entities.Length.Should().Be(1);
            filter.GetComponent<Component3>(0);
        };

        action.Should().Throw<Exception>();
    }
}