using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid;

public static class HybridFilterQueryExtensions
{
    // todo: test and benchmark
    public static void ExecuteQueryWithoutId<TMask, TQ, T1, T2>(this HybridFilter<TMask> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        // archetypes that contain full entity
        for (int i = 0; i < filter.FullArchetypes.Length; i++)
        {
            var archetype = filter.FullArchetypes[i];
            var span1 = archetype.GetComponents<T1>();
            var span2 = archetype.GetComponents<T2>();

            ref var component1 = ref span1[0];
            ref var component2 = ref span2[0];
            var length = span1.Length;

            for (int index = 0; index < length; index++)
            {
                query.Update(ref component1, ref component2);

                component1 = ref Unsafe.Add(ref component1, 1);
                component2 = ref Unsafe.Add(ref component2, 1);
            }
        }

        var sparseSet1 = filter.World.GetSparseSet<T1>();
        var sparseSet2 = filter.World.GetSparseSet<T2>();

        var entities = filter.SparseEntities;

        // generic case
        for (int index = 0; index < entities.Length; index++)
        {
            var entityId = entities[index];
            ref var c1 = ref sparseSet1.GetComponent(entityId);
            ref var c2 = ref sparseSet2.GetComponent(entityId);
            query.Update(ref c1, ref c2);
        }
    }

    public static void ExecuteBatchQuery<TMask, TQ, T1, T2>(this HybridFilter<TMask> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : IBatchQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        // archetypes that contain full entity
        for (int i = 0; i < filter.FullArchetypes.Length; i++)
        {
            var archetype = filter.FullArchetypes[i];
            var span1 = archetype.GetComponents<T1>();
            var span2 = archetype.GetComponents<T2>();

            query.BatchUpdate(span1, span2);
        }

        var sparseSet1 = filter.World.GetSparseSet<T1>();
        var sparseSet2 = filter.World.GetSparseSet<T2>();

        var entities = filter.SparseEntities;

        // generic case
        for (int index = 0; index < entities.Length; index++)
        {
            var entityId = entities[index];
            ref var c1 = ref sparseSet1.GetComponent(entityId);
            ref var c2 = ref sparseSet2.GetComponent(entityId);
            query.SparseUpdate(ref c1, ref c2);
        }
    }
}