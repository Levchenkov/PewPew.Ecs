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

public static class HeavyIndexedWorldQueryExtensions
{
    public static void ExecuteHeavyFilterableQuery<TQ, T1, T2>(this IndexedWorld world, TQ query)
        where TQ : struct, IHeavyFilterableQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var heavyFilter = world.FetchHeavyFilter(query.HeavyFilter);
        heavyFilter.ExecuteQueryWithoutId(query);
    }

    public static void ExecuteHeavyFilterableQuery<TMask, TQ, T1, T2>(this IndexedWorld<TMask> world, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : struct, IHeavyFilterableQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var heavyFilter = world.FetchHeavyFilter(query.HeavyFilter);
        heavyFilter.ExecuteQueryWithoutId(query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<EntityId> GetEntities<T1, T2>(this IndexedWorld world, HeavyFilterDefinition<T1, T2> filterDefinition)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.FetchHeavyFilter(filterDefinition);

        return filter.Entities;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutId<TQ, T1, T2>(this IndexedWorld world, HeavyFilterDefinition<T1, T2> filterDefinition, TQ query)
        where TQ : struct, IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var heavyFilter = world.FetchHeavyFilter(filterDefinition);
        heavyFilter.ExecuteQueryWithoutId<BitMask, TQ, T1, T2>(query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<TQ, T1, T2>(this IndexedWorld world, HeavyFilterDefinition<T1, T2> filterDefinition, TQ query)
        where TQ : struct, IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var heavyFilter = world.FetchHeavyFilter(filterDefinition);
        heavyFilter.ExecuteQuery<BitMask, TQ, T1, T2>(query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutId<T1, T2>(this IndexedWorld world, HeavyFilterDefinition<T1, T2> filterDefinition, QueryActionWithoutId<T1, T2> query)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.FetchHeavyFilter(filterDefinition);
        filter.ExecuteQueryWithoutId(query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<T1, T2>(this IndexedWorld world, HeavyFilterDefinition<T1, T2> filterDefinition, QueryAction<T1, T2> query)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.FetchHeavyFilter(filterDefinition);
        filter.ExecuteQuery(query);
    }

    public static void ExecuteQueryWithTag<T1, T2>(this IndexedWorld world, HeavyFilterDefinition<T1, T2> filterDefinition, QueryActionWithTag<T1, T2> queryAction)
        where T1 : struct, IComponent
        where T2 : struct, ITagComponent
    {
        var filter = world.FetchHeavyFilter(filterDefinition);
        filter.ExecuteQueryWithTag<T1, T2>(queryAction);
    }
}