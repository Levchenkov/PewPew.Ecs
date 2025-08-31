using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class TagHeavyFilterTests
{
    [Fact]
    public void FetchHeavyFilter_EmptyFilter_EntitiesShouldBeEmpty()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        filter.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void FetchHeavyFilter_ComponentsIsNotInited_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        var filterDefinition = new HeavyFilterDefinition<Component1, Tag1>();
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
        world.InitTag<Tag1>();

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);
        world.AddTag<Tag1>(entityId1); // duplicate call

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddTag<Tag1>(entityId2);
        world.AddComponent<Component1>(entityId2); // duplicate call

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
    }

    [Fact]
    public void FetchHeavyFilter_CreateFilterBeforeAndAfterEntitiesWereAdded_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        filter.Entities.Length.Should().Be(1);
        filter.Entities[0].Should().Be(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddTag<Tag1>(entityId2);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
    }

    [Fact]
    public void FetchHeavyFilter_CreateFilterAfterEntitiesWereAdded_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddTag<Tag1>(entityId2);

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        world.AddComponent<Component1>(entityId1); // duplicate call
        world.AddTag<Tag1>(entityId2); // duplicate call

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteLastComponent_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddTag<Tag1>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;
        world.AddTag<Tag1>(entityId3);

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        world.DeleteTag<Tag1>(entityId3);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeFalse();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteMiddleComponent_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddTag<Tag1>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;
        world.AddTag<Tag1>(entityId3);

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        world.DeleteTag<Tag1>(entityId2);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId3);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeFalse();
        filter.HasEntity(entityId3).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component1>(1).Value.Should().Be(31);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteFirstComponent_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddTag<Tag1>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;
        world.AddTag<Tag1>(entityId3);

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        world.DeleteTag<Tag1>(entityId1);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId3); // deletion swaps with last entity
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeFalse();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(31);
        filter.GetComponent<Component1>(1).Value.Should().Be(21);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteComponentsNotFromFilter_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddTag<Tag1>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        world.DeleteComponent<Component1>(entityId1);
        world.DeleteComponent<Component1>(entityId3);

        filter.Entities.Length.Should().Be(1);
        filter.Entities[0].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeFalse();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeFalse();

        filter.GetComponent<Component1>(0).Value.Should().Be(21);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteTagsNotFromFilter_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;
        world.AddTag<Tag1>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId3);

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        world.DeleteTag<Tag1>(entityId1);
        world.DeleteTag<Tag1>(entityId3);

        filter.Entities.Length.Should().Be(1);
        filter.Entities[0].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeFalse();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeFalse();

        filter.GetComponent<Component1>(0).Value.Should().Be(21);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteComponentsNotFromFilter2_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId2).Value = 21;

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;
        world.AddTag<Tag1>(entityId3);

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        world.DeleteComponent<Component1>(entityId2);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId3);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeFalse();
        filter.HasEntity(entityId3).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component1>(1).Value.Should().Be(31);

        world.DeleteComponent<Component1>(entityId1);

        filter.GetComponent<Component1>(0).Value.Should().Be(31);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteTagsNotFromFilter2_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId3).Value = 31;
        world.AddTag<Tag1>(entityId3);

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        world.DeleteTag<Tag1>(entityId2);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId3);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeFalse();
        filter.HasEntity(entityId3).Should().BeTrue();

        filter.GetComponent<Component1>(0).Value.Should().Be(11);
        filter.GetComponent<Component1>(1).Value.Should().Be(31);

        world.DeleteTag<Tag1>(entityId1);

        filter.GetComponent<Component1>(0).Value.Should().Be(31);
    }

    [Fact]
    public void FetchHeavyFilter_DeleteAllComponentFromEntity_EntityShouldBeDeleteFromFilter()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);

        var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

        world.DeleteComponent<Component1>(entityId1);
        world.DeleteTag<Tag1>(entityId1);

        filter.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void FetchHeavyFilter_GetWrongComponent_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var entityId1 = world.CreateEntityId();
        world.AddComponent<Component1>(entityId1).Value = 11;
        world.AddTag<Tag1>(entityId1);

        var action = () =>
        {
            var filter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Tag1>());

            filter.Entities.Length.Should().Be(1);
            filter.GetComponent<Component3>(0);
        };

        action.Should().Throw<Exception>();
    }
}