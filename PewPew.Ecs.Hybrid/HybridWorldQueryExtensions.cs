using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid;

public static class HybridWorldQueryExtensions
{
    public static void ExecuteQueryWithoutId<TQ, T1, T2>(this HybridWorld world, FilterDefinition definition, TQ query)
        where TQ : IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.GetFilter(definition);
        filter.ExecuteQueryWithoutId<BitMask64, TQ, T1, T2>(query);
    }

    public static void ExecuteBatchQuery<TQ, T1, T2>(this HybridWorld world, FilterDefinition definition, TQ query)
        where TQ : IBatchQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var filter = world.GetFilter(definition);
        filter.ExecuteBatchQuery<BitMask64, TQ, T1, T2>(query);
    }
}