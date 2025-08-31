using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid;

namespace PewPew.Ecs.UnitTests;

public class HybridFilterTests
{
    // filter is [A, B], archetype1 is [A, B]
    [Fact]
    public void Test()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype1 = world.GetStaticArchetype<Component1, Component2>();

        var filterDefinition = new FilterDefinition().With<Component1>().With<Component2>();

        for (int i = 0; i < 10; i++)
        {
            // full
            var entityId1 = world.CreateEntityId();
            var tuple1 = archetype1.Add(entityId1);
            tuple1.Component1.Value = 42;
            tuple1.Component2.Value = entityId1.Index;
        }

        for (int i = 0; i < 10; i++)
        {
            // sparse set
            var entityId2 = world.CreateEntityId();
            world.AddComponent<Component1>(entityId2).Value = 42;
            if (entityId2.Index % 2 == 0)
            {
                world.AddComponent<Component2>(entityId2).Value = entityId2.Index;
            }
        }

        HybridFilter<BitMask64> filter = world.GetFilter(filterDefinition);

        filter.ExecuteQueryWithoutId<BitMask64, Query, Component1, Component2>(default);

        archetype1.Count.Should().Be(10);

        for (int i = 0; i < archetype1.Count; i++)
        {
            var entityId1 = archetype1.Entities[i];
            archetype1.Components1[i].Value.Should().Be(entityId1.Index);
        }

        var components1 = world.GetSparseSet<Component1>(); // not public api
        components1.Count.Should().Be(10);
        for (int i = 0; i < components1.Count; i++)
        {
            var entityId3 = components1.Entities[i];
            if (entityId3.Index % 2 == 0)
            {
                components1.Components[i].Value.Should().Be(entityId3.Index);
            }
            else
            {
                components1.Components[i].Value.Should().Be(42);
            }
        }
    }

    private readonly struct Query : IQueryWithoutId<Component1, Component2>
    {
        public void Update(ref Component1 component1, ref Component2 component2)
        {
            component1.Value = component2.Value;
        }
    }
}