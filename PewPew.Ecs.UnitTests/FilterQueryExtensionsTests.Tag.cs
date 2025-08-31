using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.UnitTests;

public class TagFilterQueryExtensionsTests
{
    [Fact]
    public void ExecuteQuery_Filter_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitTag<Tag1>();
        world.InitComponent<Component2>();

        for (int i = 0; i < world.EntityCapacity; i++)
        {
            var entityId = world.CreateEntityId();

            world.AddTag<Tag1>(entityId);
            world.AddComponent<Component2>(entityId).Value = entityId.Index;
        }

        var filterDefinition = new FilterDefinition().WithTag<Tag1>().With<Component2>();
        var filter = world.GetFilter(filterDefinition);

        filter.ExecuteQueryWithTag<QueryWithTag, Component2, Tag1>(default);

        foreach (var entityId in world.Entities)
        {
            world.HasTag<Tag1>(entityId).Should().Be(true);
            world.GetComponent<Component2>(entityId).Value.Should().Be(entityId.Index * 2);
        }
    }

    [Fact]
    public void ExecuteQueryAction_Filter_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitTag<Tag1>();
        world.InitComponent<Component2>();

        for (int i = 0; i < world.EntityCapacity; i++)
        {
            var entityId = world.CreateEntityId();

            world.AddTag<Tag1>(entityId);
            world.AddComponent<Component2>(entityId).Value = entityId.Index;
        }

        var filterDefinition = new FilterDefinition().WithTag<Tag1>().With<Component2>();

        world.ExecuteQueryWithTag<Component2, Tag1>(filterDefinition, (EntityId _, ref Component2 component2) => component2.Value *= 2);

        foreach (var entityId in world.Entities)
        {
            world.HasTag<Tag1>(entityId).Should().Be(true);
            world.GetComponent<Component2>(entityId).Value.Should().Be(entityId.Index * 2);
        }
    }

    private struct QueryWithTag : IQueryWithTag<Component2, Tag1>
    {
        public void Update(EntityId entityId, ref Component2 component2)
        {
            component2.Value *= 2;
        }
    }
}