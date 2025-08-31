using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

using BitMask =
#if BIT_MASK_64_DEFAULT
    PewPew.Ecs.Filters.Masks.BitMask64;
#elif BIT_MASK_128_DEFAULT
    PewPew.Ecs.Filters.Masks.BitMask128;
#elif BIT_MASK_256_DEFAULT
    PewPew.Ecs.Filters.Masks.BitMask256;
#endif


namespace PewPew.Ecs.Filters;

public static class IndexedWorldQueryExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteFilterableQuery<TQ, T1, T2>(this IndexedWorld world, TQ query)
        where TQ : struct, IFilterableQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        ExecuteFilterableQuery<BitMask, TQ, T1, T2>(world, query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteFilterableQuery<TMask, TQ, T1, T2>(this IndexedWorld<TMask> world, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : struct, IFilterableQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.GetFilter(query.Filter);
        filter.ExecuteQueryWithoutId<TMask, TQ, T1, T2>(query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutId<TQ, T1, T2>(this IndexedWorld world, FilterDefinitionBase filterDefinition, TQ query)
        where TQ : struct, IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.GetFilter(filterDefinition);
        filter.ExecuteQueryWithoutId<BitMask, TQ, T1, T2>(query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<TQ, T1, T2>(this IndexedWorld world, FilterDefinitionBase filterDefinition, TQ query)
        where TQ : struct, IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.GetFilter(filterDefinition);
        filter.ExecuteQuery<BitMask, TQ, T1, T2>(query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<EntityId> GetEntities(this IndexedWorld world, FilterDefinitionBase filterDefinition)
    {
        var filter = world.GetFilter(filterDefinition);

        return filter.Entities;
    }

    public static void ExecuteQueryWithoutId<TQ, T1, T2>(this IndexedWorld world, TQ query)
        where TQ : IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var sparseSet2 = world.GetSparseSet<T2>();
        var entities = sparseSet2.Entities;
        var components = sparseSet2.Components;

        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet2.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            query.Update(ref componentRef1.Component, ref components[i]);
        }
    }

    public static void ExecuteQuery<TQ, T1, T2>(this IndexedWorld world, TQ query)
        where TQ : IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var sparseSet2 = world.GetSparseSet<T2>();
        var entities = sparseSet2.Entities;
        var components = sparseSet2.Components;

        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet2.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            query.Update(entityId, ref componentRef1.Component, ref components[i]);
        }
    }

    public static void ExecuteQueryWithTag<TQ, T1, T2>(this IndexedWorld world, FilterDefinitionBase filterDefinition, TQ query)
        where TQ : IQueryWithTag<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, ITagComponent
    {
        var filter = world.GetFilter(filterDefinition);
        filter.ExecuteQueryWithTag<TQ, T1, T2>(query);
    }
}