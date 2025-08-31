using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class TagFilterTests
{
    [Fact]
    public void GetFilter_EmptyFilter_EntitiesShouldBeEmpty()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitTag<Tag1>();
        world.InitTag<Tag2>();

        var filter = world.GetFilter(new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>());

        filter.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void GetFilter_TagsIsNotInited_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        var filterDefinition = new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>();
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
        world.InitTag<Tag1>();
        world.InitTag<Tag2>();

        var filter = world.GetFilter(new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>());

        var entityId1 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId1);
        world.AddTag<Tag2>(entityId1);
        world.AddTag<Tag2>(entityId1); // duplicate call

        var entityId2 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId2);
        world.AddTag<Tag2>(entityId2);
        world.AddTag<Tag1>(entityId2); // duplicate call

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
        world.InitTag<Tag1>();
        world.InitTag<Tag2>();

        var entityId1 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId1);
        world.AddTag<Tag2>(entityId1);

        var filter = world.GetFilter(new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>());

        filter.Entities.Length.Should().Be(1);
        filter.Entities[0].Should().Be(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId2);
        world.AddTag<Tag2>(entityId2);

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
        world.InitTag<Tag1>();
        world.InitTag<Tag2>();

        var entityId1 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId1);
        world.AddTag<Tag2>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId2);
        world.AddTag<Tag2>(entityId2);

        var filter = world.GetFilter(new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>());

        world.AddTag<Tag1>(entityId1); // duplicate call
        world.AddTag<Tag2>(entityId2); // duplicate call

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();
    }

    [Fact]
    public void GetFilter_DeleteLastEntity_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitTag<Tag1>();
        world.InitTag<Tag2>();

        var entityId1 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId1);
        world.AddTag<Tag2>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId2);
        world.AddTag<Tag2>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId3);
        world.AddTag<Tag2>(entityId3);

        var filter = world.GetFilter(new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>());

        world.DeleteTag<Tag2>(entityId3);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeFalse();
    }

    [Fact]
    public void GetFilter_DeleteMiddleEntity_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitTag<Tag1>();
        world.InitTag<Tag2>();

        var entityId1 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId1);
        world.AddTag<Tag2>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId2);
        world.AddTag<Tag2>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId3);
        world.AddTag<Tag2>(entityId3);

        var filter = world.GetFilter(new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>());

        world.DeleteTag<Tag2>(entityId2);

        filter.Entities.Length.Should().Be(2);
        filter.Entities[0].Should().Be(entityId1);
        filter.Entities[1].Should().Be(entityId3);

        filter.HasEntity(entityId1).Should().BeTrue();
        filter.HasEntity(entityId2).Should().BeFalse();
        filter.HasEntity(entityId3).Should().BeTrue();
    }

    [Fact]
    public void GetFilter_DeleteFirstEntity_EntitiesExist()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitTag<Tag1>();
        world.InitTag<Tag2>();

        var entityId1 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId1);
        world.AddTag<Tag2>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId2);
        world.AddTag<Tag2>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId3);
        world.AddTag<Tag2>(entityId3);

        var filter = world.GetFilter(new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>());

        world.DeleteTag<Tag2>(entityId1);

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
        world.InitTag<Tag1>();
        world.InitTag<Tag2>();

        var entityId1 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId1);

        var entityId2 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId2);
        world.AddTag<Tag2>(entityId2);

        var entityId3 = world.CreateEntityId();
        world.AddTag<Tag1>(entityId3);

        var filter = world.GetFilter(new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>());

        world.DeleteTag<Tag1>(entityId1);
        world.DeleteTag<Tag1>(entityId3);

        filter.Entities.Length.Should().Be(1);
        filter.Entities[0].Should().Be(entityId2);

        filter.HasEntity(entityId1).Should().BeFalse();
        filter.HasEntity(entityId2).Should().BeTrue();
        filter.HasEntity(entityId3).Should().BeFalse();
    }
}