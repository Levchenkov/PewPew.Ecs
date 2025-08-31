using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.UnitTests;

public class LightVsHeavyTests
{
    [Fact]
    public void LightVsHeavy()
    {
        var count = 100_000;
        var world = WorldFactory.Shared.CreateIndexedWorld(count);
        world.InitComponent<Component1>(count);
        world.InitComponent<Component2>(count);

        var lightFilter = world.GetFilter(new FilterDefinition().With<Component1>().With<Component2>());
        var heavyFilter = world.FetchHeavyFilter(new HeavyFilterDefinition<Component1, Component2>());

        for (int i = 0; i < count; i++)
        {
            var entityId = world.CreateEntityId();
            world.AddComponent<Component1>(entityId).Value = i;
            world.AddComponent<Component2>(entityId).Value = i;
        }

        lightFilter.Entities.Length.Should().Be(heavyFilter.Entities.Length);
        for (int i = 0; i < count; i++)
        {
            var entityId = lightFilter.Entities[i];
            entityId.Value.Should().Be(heavyFilter.Entities[i].Value);
        }

        var testSystem = new System();
        testSystem.IndexedWorld = world;
        testSystem.Update();

        var components1 = world.GetComponents<Component1>();
        var components2 = world.GetComponents<Component2>();

        var rawComponents1 = components1.Components;
        for (int i = 0; i < components1.Count; i++)
        {
            rawComponents1[i].Value.Should().Be(i * 2);
        }

        var rawComponents2 = components2.Components;
        for (int i = 0; i < components2.Count; i++)
        {
            rawComponents2[i].Value.Should().Be(i * 3);
        }
    }

    public class System
    {
        public IndexedWorld IndexedWorld;

        private readonly HeavyFilterDefinition<Component1, Component2> _heavyFilterDefinition = new();

        private Query _query = new();

        public void Update()
        {
            var filter = IndexedWorld.FetchHeavyFilter(_heavyFilterDefinition);
            filter.ExecuteQueryWithoutId(_query);
        }

        private struct Query : IQueryWithoutId<Component1, Component2>
        {
            public void Update(ref Component1 component1, ref Component2 component2)
            {
                component1.Value *= 2;
                component2.Value *= 3;
            }
        }
    }
}