using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid;

public static class HybridWorldQueryExtensions
{
    public static void ExecuteQueryWithoutId<TQ, T1>(this HybridWorld world, TQ query)
        where TQ : IQueryWithoutId<T1>
        where T1 : struct, IComponent
    {
        var sparseSet1 = world.GetSparseSet<T1>();
        var components = sparseSet1.Components;

        ref var component = ref components[0];
        var length = components.Length;

        for (int index = 0; index < length; index++)
        {
            query.Update(ref component);

            component = ref Unsafe.Add(ref component, 1);
        }
    }

    public static void ExecuteQueryWithoutId<TQ, T1, T2>(this HybridWorld world, FilterDefinition definition, TQ query)
        where TQ : IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.GetFilter(definition);
        filter.ExecuteQueryWithoutId<BitMask64, TQ, T1, T2>(query);
    }

    public static void ExecuteQueryWithoutId<TQ, T1, T2, T3>(this HybridWorld world, FilterDefinition definition, TQ query)
        where TQ : IQueryWithoutId<T1, T2, T3>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var filter = world.GetFilter(definition);
        filter.ExecuteQueryWithoutId<BitMask64, TQ, T1, T2, T3>(query);
    }

    public static void ExecuteBatchQuery<TQ, T1>(this HybridWorld world, TQ query)
        where TQ : IBatchQuery<T1>
        where T1 : struct, IComponent
    {
        var globalIndex = ComponentMetadata<T1>.GlobalIndex;
        if (globalIndex == ComponentMetadata.InvalidIndex)
        {
            throw new NotSupportedException($"Component {typeof(T1).Name} is not initialized");
        }

        var localIndex = world.GetLocalIndex(globalIndex);
        var mask = default(BitMask64);
        if (localIndex == ComponentMetadata.InvalidIndex)
        {
            throw new NotSupportedException($"Component {typeof(T1).Name} is not initialized");
        }
        mask.SetBit(localIndex);

        foreach (var (_, staticArchetype) in world._typeToArchetypeMap)
        {
            if (staticArchetype.Mask.Has(mask))
            {
                var span1 = staticArchetype.GetComponents<T1>();

                query.BatchUpdate(span1);
            }
        }

        var sparseSet1 = world.GetSparseSet<T1>();
        var components = sparseSet1.Components;
        query.BatchUpdate(components);
    }

    public static void ExecuteBatchQuery<TQ, T1, T2>(this HybridWorld world, FilterDefinition definition, TQ query)
        where TQ : IBatchQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.GetFilter(definition);
        filter.ExecuteBatchQuery<BitMask64, TQ, T1, T2>(query);
    }

    public static void ExecuteBatchQuery<TQ, T1, T2, T3>(this HybridWorld world, FilterDefinition definition, TQ query)
        where TQ : IBatchQuery<T1, T2, T3>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var filter = world.GetFilter(definition);
        filter.ExecuteBatchQuery<BitMask64, TQ, T1, T2, T3>(query);
    }
}