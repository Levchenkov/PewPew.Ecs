using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.UnitTests;

public class HeavyFilterQueryExtensionsTests
{
    [Fact]
    public void ExecuteQuery_HeavyFilterWithWrongComponentOrder_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        for (int i = 0; i < world.EntityCapacity; i++)
        {
            var entityId = world.CreateEntityId();

            world.AddComponent<Component1>(entityId).Value = entityId.Index;
            world.AddComponent<Component2>(entityId).Value = entityId.Index;
        }

        var filterDefinition = new HeavyFilterDefinition<Component1,Component2>();
        var filter = world.FetchHeavyFilter(filterDefinition);

        filter.ExecuteQueryWithoutId<Query12, Component1, Component2>(default);
        // filter.ExecuteQuery<Query21, Component2, Component1>(default); // compile error

        foreach (var entityId in world.Entities)
        {
            world.GetComponent<Component1>(entityId).Value.Should().Be(entityId.Index * 2);
            world.GetComponent<Component2>(entityId).Value.Should().Be(entityId.Index * 3);
        }
    }

    private struct Query12 : IQueryWithoutId<Component1, Component2>
    {
        public void Update(ref Component1 component1, ref Component2 component2)
        {
            component1.Value *= 2;
            component2.Value *= 3;
        }
    }

    private struct Query21 : IQueryWithoutId<Component2, Component1>
    {
        public void Update(ref Component2 component2, ref Component1 component1)
        {
            component1.Value *= 2;
            component2.Value *= 3;
        }
    }

    private struct Query : IQuery<Component1, Component2>
    {
        public void Update(EntityId entityId, ref Component1 component1, ref Component2 component2)
        {
            component1.Value *= 2;
            component2.Value *= 3;
        }
    }
}