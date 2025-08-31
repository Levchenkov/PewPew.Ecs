using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters;

public partial class IndexedWorld<TMask>
    where TMask : struct, IBitMask<TMask>
{
    private HeavyFilterModule<TMask> _heavyFilterModule;
    private HeavyFilterInstanceBase<TMask>[] _heavyFilters;

    private void HeavyFilterCtor()
    {
        _heavyFilterModule = new HeavyFilterModule<TMask>(this, _settings);
        _heavyFilters = new HeavyFilterInstanceBase<TMask>[10];
    }

    public void WarmUpFilter<T1, T2>(HeavyFilterDefinition<T1, T2> filterDefinition)
        where T1 : struct
        where T2 : struct
    {
        FetchHeavyFilter(filterDefinition);
    }

    public void WarmUpFilter<T1, T2, T3>(HeavyFilterDefinition<T1, T2, T3> filterDefinition)
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        FetchHeavyFilter(filterDefinition);
    }

    public HeavyFilter<TMask, T1, T2> FetchHeavyFilter<T1, T2>(HeavyFilterDefinition<T1, T2> filterDefinition)
        where T1 : struct
        where T2 : struct
    {
        var filter = (HeavyFilterInstance<TMask, T1, T2>)FetchHeavyFilterInternal<T1, T2, None>(filterDefinition);

        return new HeavyFilter<TMask, T1, T2>(filter);
    }

    public HeavyFilter<TMask, T1, T2, T3> FetchHeavyFilter<T1, T2, T3>(HeavyFilterDefinition<T1, T2, T3> filterDefinition)
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        var filter = (HeavyFilterInstance<TMask, T1, T2, T3>)FetchHeavyFilterInternal<T1, T2, T3>(filterDefinition);

        return new HeavyFilter<TMask, T1, T2, T3>(filter);
    }

    private HeavyFilterInstanceBase<TMask> FetchHeavyFilterInternal<T1, T2, T3>(HeavyFilterDefinition filterDefinition)
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        var filterId = filterDefinition.HeavyFilterId;
        if (filterId == HeavyFilterDefinition.InvalidId)
        {
            var filter = FindOrCreateHeavyFilter<T1, T2, T3>(this, filterDefinition);

            return filter;
        }

#if DEBUG
        if(filterDefinition.WorldId != _world.Id)
            throw new NotSupportedException($"{nameof(HeavyFilterDefinition)} WorldId '{filterDefinition.WorldId}' is not equal to World Id '{_world.Id}'");
#endif

        return _heavyFilters[filterId];
    }

    private static HeavyFilterInstanceBase<TMask> FindOrCreateHeavyFilter<T1, T2, T3>(IndexedWorld<TMask> world, HeavyFilterDefinition filterDefinition)
        where T1 : struct where T2 : struct where T3 : struct
    {
        ref var heavyFilters = ref world._heavyFilters;
        var filter = world._heavyFilterModule.FetchHeavyFilter<T1, T2, T3>(filterDefinition, heavyFilters);

        if (filterDefinition.HeavyFilterId >= heavyFilters.Length)
        {
            Array.Resize(ref heavyFilters, heavyFilters.Length * 2);
        }

        heavyFilters[filterDefinition.HeavyFilterId] = filter;

        return filter;
    }

    internal void OnComponentAdded(EntityId entityId, int localIndex, List<FilterInstance<TMask>> filters, List<HeavyFilterInstanceBase<TMask>> heavyFilters)
    {
        ref var entityMask = ref _entityMasks[entityId.Index];
        entityMask.SetBit(localIndex);

        for (var index = 0; index < filters.Count; index++)
        {
            var filter = filters[index];
            if (entityMask.Has(filter.Mask))
            {
                filter.AddEntity(entityId);
            }
        }

        for (var index = 0; index < heavyFilters.Count; index++)
        {
            var heavyFilter = heavyFilters[index];
            if (entityMask.Has(heavyFilter.Mask))
            {
                heavyFilter.AddEntity(entityId);
            }
        }
    }

    internal void OnComponentDeleted(
        EntityId entityId,
        EntityId replacedEntityId,
        int globalIndex,
        int localIndex,
        List<FilterInstance<TMask>> filters,
        List<HeavyFilterInstanceBase<TMask>> heavyFilters)
    {
        ref var entityMask = ref _entityMasks[entityId.Index];
        var oldEntityMask = entityMask;
        entityMask.ClearBit(localIndex);

        for (var index = 0; index < filters.Count; index++)
        {
            var filter = filters[index];
            if (oldEntityMask.Has(filter.Mask))
            {
                filter.DeleteEntity(entityId);
            }
        }

        if (replacedEntityId == EntityId.Invalid)
        {
            for (int index = 0; index < heavyFilters.Count; index++)
            {
                var heavyFilter = heavyFilters[index];

                if (oldEntityMask.Has(heavyFilter.Mask))
                {
                    heavyFilter.DeleteEntity(entityId);
                }
            }
        }
        else
        {
            var replacedEntityMask = _entityMasks[replacedEntityId.Index];

            for (int index = 0; index < heavyFilters.Count; index++)
            {
                var heavyFilter = heavyFilters[index];

                if (oldEntityMask.Has(heavyFilter.Mask))
                {
                    heavyFilter.DeleteEntity(entityId);
                }

                if (replacedEntityMask.Has(heavyFilter.Mask))
                {
                    heavyFilter.UpdateIndex(replacedEntityId, globalIndex);
                }
            }
        }
    }
}