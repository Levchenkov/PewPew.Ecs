using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.UnitTests;

public class TagHeavyFilterQueryExtensionsTests
{
    [Fact]
    public void ExecuteQuery_Filter_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        for (int i = 0; i < world.EntityCapacity; i++)
        {
            var entityId = world.CreateEntityId();

            world.AddComponent<Component1>(entityId).Value = entityId.Index;
            world.AddTag<Tag1>(entityId);
        }

        var filterDefinition = new HeavyFilterDefinition<Component1, Tag1>();
        var filter = world.FetchHeavyFilter(filterDefinition);

        filter.ExecuteQueryWithTag<QueryWithTag, Component1, Tag1>(default);

        foreach (var entityId in world.Entities)
        {
            world.GetComponent<Component1>(entityId).Value.Should().Be(entityId.Index * 2);
            world.HasTag<Tag1>(entityId).Should().Be(true);
        }
    }

    [Fact]
    public void ExecuteQueryAction_Filter_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        for (int i = 0; i < world.EntityCapacity; i++)
        {
            var entityId = world.CreateEntityId();

            world.AddComponent<Component1>(entityId).Value = entityId.Index;
            world.AddTag<Tag1>(entityId);
        }

        var filterDefinition = new HeavyFilterDefinition<Component1, Tag1>();

        world.ExecuteQueryWithTag(filterDefinition, (EntityId _, ref Component1 component1) => component1.Value *= 2);

        foreach (var entityId in world.Entities)
        {
            world.GetComponent<Component1>(entityId).Value.Should().Be(entityId.Index * 2);
            world.HasTag<Tag1>(entityId).Should().Be(true);
        }
    }

    private struct QueryWithTag : IQueryWithTag<Component1, Tag1>
    {
        public void Update(EntityId entityId, ref Component1 component1)
        {
            component1.Value *= 2;
        }
    }
}