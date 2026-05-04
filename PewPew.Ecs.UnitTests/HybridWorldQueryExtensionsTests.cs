using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Hybrid;

namespace PewPew.Ecs.UnitTests;

public class HybridWorldQueryExtensionsTests
{
    // -------------------------------------------------------------------------
    // ExecuteQueryWithoutId<T1> — sparse set, no filter
    // -------------------------------------------------------------------------

    [Fact]
    public void ExecuteQueryWithoutId_T1_SparseSet_AllEntitiesUpdated()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component1>();

        var e1 = world.CreateEntityId();
        world.AddComponent<Component1>(e1).Value = 10;

        var e2 = world.CreateEntityId();
        world.AddComponent<Component1>(e2).Value = 20;

        var e3 = world.CreateEntityId();
        world.AddComponent<Component1>(e3).Value = 30;

        world.ExecuteQueryWithoutId<DoubleComponent1Query, Component1>(default);

        world.GetComponent<Component1>(e1).Value.Should().Be(20);
        world.GetComponent<Component1>(e2).Value.Should().Be(40);
        world.GetComponent<Component1>(e3).Value.Should().Be(60);
    }

    // -------------------------------------------------------------------------
    // ExecuteQueryWithoutId<T1, T2> — with FilterDefinition
    // -------------------------------------------------------------------------

    [Fact]
    public void ExecuteQueryWithoutId_T1T2_WithFilter_OnlyMatchingEntitiesUpdated()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        // entity matching filter: Component1 + Component2
        var matched = world.CreateEntityId();
        world.AddComponent<Component1>(matched).Value = 1;
        world.AddComponent<Component2>(matched).Value = 5;

        // entity NOT matching filter: only Component1
        var unmatched = world.CreateEntityId();
        world.AddComponent<Component1>(unmatched).Value = 1;

        var filterDef = new FilterDefinition().With<Component1>().With<Component2>();

        // query: component1.Value += component2.Value
        world.ExecuteQueryWithoutId<AddComponent2ToComponent1Query, Component1, Component2>(filterDef, default);

        world.GetComponent<Component1>(matched).Value.Should().Be(6);   // 1 + 5
        world.GetComponent<Component1>(unmatched).Value.Should().Be(1); // unchanged
    }

    // -------------------------------------------------------------------------
    // ExecuteQueryWithoutId<T1, T2, T3> — with FilterDefinition
    // -------------------------------------------------------------------------

    [Fact]
    public void ExecuteQueryWithoutId_T1T2T3_WithFilter_OnlyMatchingEntitiesUpdated()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();
        world.InitComponent<Component3>();

        // entity matching filter: Component1 + Component2 + Component3
        var matched = world.CreateEntityId();
        world.AddComponent<Component1>(matched).Value = 1;
        world.AddComponent<Component2>(matched).Value = 10;
        world.AddComponent<Component3>(matched).Value = 100;

        // entity NOT matching filter: Component1 + Component2 only
        var unmatched = world.CreateEntityId();
        world.AddComponent<Component1>(unmatched).Value = 1;
        world.AddComponent<Component2>(unmatched).Value = 10;

        var filterDef = new FilterDefinition().With<Component1>().With<Component2>().With<Component3>();

        // query: component1.Value = component2.Value + component3.Value
        world.ExecuteQueryWithoutId<SumComponents2And3IntoComponent1Query, Component1, Component2, Component3>(filterDef, default);

        world.GetComponent<Component1>(matched).Value.Should().Be(110);  // 10 + 100
        world.GetComponent<Component1>(unmatched).Value.Should().Be(1);  // unchanged
    }

    // -------------------------------------------------------------------------
    // ExecuteBatchQuery<T1> — archetypes + sparse set, no filter
    // -------------------------------------------------------------------------

    [Fact]
    public void ExecuteBatchQuery_T1_ArchetypeAndSparseEntitiesBothUpdated()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        // entity in static archetype
        var archetypeEntity = world.CreateEntityId();
        archetype.Add(archetypeEntity, new Component1 { Value = 10 }, new Component2 { Value = 0 });

        // entity in sparse set
        var sparseEntity = world.CreateEntityId();
        world.AddComponent<Component1>(sparseEntity).Value = 20;

        world.ExecuteBatchQuery<DoubleComponent1BatchQuery, Component1>(default);

        // both should be doubled
        archetype.GetComponent1(archetypeEntity).Value.Should().Be(20);
        world.GetComponent<Component1>(sparseEntity).Value.Should().Be(40);
    }

    [Fact]
    public void ExecuteBatchQuery_T1_UninitializedComponent_ThrowsNotSupportedException()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        // Component5 is never initialized in this world

        Action action = () => world.ExecuteBatchQuery<DoubleComponent5BatchQuery, Component5>(default);

        action.Should().Throw<NotSupportedException>();
    }

    // -------------------------------------------------------------------------
    // ExecuteBatchQuery<T1, T2, T3> — with FilterDefinition
    // -------------------------------------------------------------------------

    [Fact]
    public void ExecuteBatchQuery_T1T2T3_WithFilter_MatchingEntitiesUpdated()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2, Component3>();

        var archetype = world.GetStaticArchetype<Component1, Component2, Component3>();

        // entity in archetype (matches filter)
        var archetypeEntity = world.CreateEntityId();
        archetype.Add(archetypeEntity,
            new Component1 { Value = 0 },
            new Component2 { Value = 3 },
            new Component3 { Value = 7 });

        // entity in sparse set (matches filter)
        var sparseEntity = world.CreateEntityId();
        world.AddComponent<Component1>(sparseEntity).Value = 0;
        world.AddComponent<Component2>(sparseEntity).Value = 4;
        world.AddComponent<Component3>(sparseEntity).Value = 6;

        // entity in sparse set, only Component1+Component2 (does NOT match filter)
        var unmatchedEntity = world.CreateEntityId();
        world.AddComponent<Component1>(unmatchedEntity).Value = 99;
        world.AddComponent<Component2>(unmatchedEntity).Value = 1;

        var filterDef = new FilterDefinition()
            .With<Component1>().With<Component2>().With<Component3>();

        // query: component1.Value = component2.Value + component3.Value
        world.ExecuteBatchQuery<SumComponents2And3BatchQuery, Component1, Component2, Component3>(filterDef, default);

        archetype.GetComponent1(archetypeEntity).Value.Should().Be(10);  // 3 + 7
        world.GetComponent<Component1>(sparseEntity).Value.Should().Be(10);   // 4 + 6
        world.GetComponent<Component1>(unmatchedEntity).Value.Should().Be(99); // unchanged
    }
}

// ---------------------------------------------------------------------------
// Query helpers
// ---------------------------------------------------------------------------

file readonly struct DoubleComponent1Query : IQueryWithoutId<Component1>
{
    public void Update(ref Component1 c1) => c1.Value *= 2;
}

file readonly struct AddComponent2ToComponent1Query : IQueryWithoutId<Component1, Component2>
{
    public void Update(ref Component1 c1, ref Component2 c2) => c1.Value += c2.Value;
}

file readonly struct SumComponents2And3IntoComponent1Query : IQueryWithoutId<Component1, Component2, Component3>
{
    public void Update(ref Component1 c1, ref Component2 c2, ref Component3 c3)
        => c1.Value = c2.Value + c3.Value;
}

file readonly struct DoubleComponent1BatchQuery : IBatchQuery<Component1>
{
    public void BatchUpdate(Span<Component1> components)
    {
        for (int i = 0; i < components.Length; i++)
            components[i].Value *= 2;
    }

    public void SparseUpdate(ref Component1 c1) => c1.Value *= 2;
}

file readonly struct DoubleComponent5BatchQuery : IBatchQuery<Component5>
{
    public void BatchUpdate(Span<Component5> components)
    {
        for (int i = 0; i < components.Length; i++)
            components[i].Value *= 2;
    }

    public void SparseUpdate(ref Component5 c) => c.Value *= 2;
}

file readonly struct SumComponents2And3BatchQuery : IBatchQuery<Component1, Component2, Component3>
{
    public void BatchUpdate(Span<Component1> c1s, Span<Component2> c2s, Span<Component3> c3s)
    {
        for (int i = 0; i < c1s.Length; i++)
            c1s[i].Value = c2s[i].Value + c3s[i].Value;
    }

    public void SparseUpdate(ref Component1 c1, ref Component2 c2, ref Component3 c3)
        => c1.Value = c2.Value + c3.Value;
}
