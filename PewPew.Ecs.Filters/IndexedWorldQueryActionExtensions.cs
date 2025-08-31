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

public static class IndexedWorldQueryActionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutId<T1, T2>(this IndexedWorld world, FilterDefinitionBase filterDefinition, QueryActionWithoutId<T1, T2> query)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.GetFilter(filterDefinition);
        filter.ExecuteQueryWithoutId(query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<T1, T2>(this IndexedWorld world, FilterDefinitionBase filterDefinition, QueryAction<T1, T2> query)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.GetFilter(filterDefinition);
        filter.ExecuteQuery(query);
    }

    public static void ExecuteQueryWithoutId<T1, T2>(this IndexedWorld world, QueryActionWithoutId<T1, T2> query)
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

            query(ref componentRef1.Component, ref components[i]);
        }
    }

    public static void ExecuteQuery<T1>(this IndexedWorld world, QueryAction<T1> query)
        where T1 : struct, IComponent
    {
        var sparseSet1 = world.GetSparseSet<T1>();
        var entities = sparseSet1.Entities;
        var components = sparseSet1.Components;

        for (int i = 0; i < sparseSet1.Count; i++)
        {
            var entityId = entities[i];
            ref var component = ref components[i];

            query(entityId, ref component);
        }
    }

    public static void ExecuteQuery<T1, T2>(this IndexedWorld world, QueryAction<T1, T2> query)
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

            query(entityId, ref componentRef1.Component, ref components[i]);
        }
    }

    public static void ExecuteQuery<T1, T2, T3>(this IndexedWorld world, QueryAction<T1, T2, T3> query)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var sparseSet3 = world.GetSparseSet<T3>();
        var entities = sparseSet3.Entities;
        var components = sparseSet3.Components;

        var sparseSet2 = world.GetSparseSet<T2>();
        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet3.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet2.TryGetComponent(entityId, out var componentRef2))
            {
                continue;
            }

            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            query(entityId, ref componentRef1.Component, ref componentRef2.Component, ref components[i]);
        }
    }

    public static void ExecuteQueryWithTag<T1, T2>(this IndexedWorld world, FilterDefinitionBase filterDefinition, QueryActionWithTag<T1, T2> queryAction)
        where T1 : struct, IComponent
        where T2 : struct, ITagComponent
    {
        var filter = world.GetFilter(filterDefinition);
        filter.ExecuteQueryWithTag<T1, T2>(queryAction);
    }
}