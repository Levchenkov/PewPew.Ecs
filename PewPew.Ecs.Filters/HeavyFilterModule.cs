using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters;

internal sealed class HeavyFilterModule<TMask>
    where TMask : struct, IBitMask<TMask>
{
    private readonly IndexedWorld<TMask> _world;
    private readonly IndexedWorldSettings _settings;

    private readonly Dictionary<int, HeavyFilterDefinition> _heavyFilterDefinitions;
    private List<HeavyFilterInstanceBase<TMask>>[] _heavyFiltersPerComponent;
    private int _nextHeavyFilterId;

    public HeavyFilterModule(IndexedWorld<TMask> world, IndexedWorldSettings settings)
    {
        _world = world;
        _settings = settings;
        _heavyFiltersPerComponent = new List<HeavyFilterInstanceBase<TMask>>[10];
        for (int i = 0; i < _heavyFiltersPerComponent.Length; i++)
        {
            _heavyFiltersPerComponent[i] = new List<HeavyFilterInstanceBase<TMask>>();
        }

        _heavyFilterDefinitions = new Dictionary<int, HeavyFilterDefinition>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal List<HeavyFilterInstanceBase<TMask>> GetHeavyFilters(int componentIndex) => _heavyFiltersPerComponent[componentIndex];

    internal HeavyFilterInstanceBase<TMask> FetchHeavyFilter<T1, T2, T3>(HeavyFilterDefinition filterDefinition, HeavyFilterInstanceBase<TMask>[] filters)
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        if (CheckHeavyFilterExists(filterDefinition, filters, out var existedFilter))
            return existedFilter;

        filterDefinition.HeavyFilterId = _nextHeavyFilterId++;
        filterDefinition.WorldId = _world.Id;
        var filterHash = filterDefinition.GetHashCode();
        _heavyFilterDefinitions[filterHash] = filterDefinition;

        var filter = CreateHeavyFilter<T1, T2, T3>(filterDefinition);
        StoreFilter(filterDefinition, filter);
        ScanEntities(filter, filterDefinition.GlobalComponentIndexes.Last());

        return filter;
    }

    private bool CheckHeavyFilterExists(HeavyFilterDefinition filterDefinition, HeavyFilterInstanceBase<TMask>[] filters, out HeavyFilterInstanceBase<TMask> filter)
    {
        if (filterDefinition.HeavyFilterId == HeavyFilterDefinition.InvalidId)
        {
            var hash = filterDefinition.GetHashCode();
            if (_heavyFilterDefinitions.TryGetValue(hash, out var existedFilterDefinition) && existedFilterDefinition.Equals(filterDefinition))
            {
                if (filterDefinition.GetType() != existedFilterDefinition.GetType())
                    throw new NotSupportedException($"Filter definition with the same components but with different order has already registered: {existedFilterDefinition.GetType()}");

                filterDefinition.HeavyFilterId = existedFilterDefinition.HeavyFilterId;
                filterDefinition.WorldId = existedFilterDefinition.WorldId;
            }

            if (filterDefinition.HeavyFilterId == HeavyFilterDefinition.InvalidId)
            {
                filter = null!;

                return false;
            }
        }

        filter = filters[filterDefinition.HeavyFilterId];

        return filter != null!;
    }

    private HeavyFilterInstanceBase<TMask> CreateHeavyFilter<T1, T2, T3>(HeavyFilterDefinition filterDefinition)
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        if(filterDefinition.HeavyFilterId == HeavyFilterDefinition.InvalidId)
            throw new NotSupportedException();

        var mask = default(TMask);

        foreach (var globalIndex in filterDefinition.GlobalComponentIndexes)
        {
            var localIndex = _world.GetLocalIndex(globalIndex);
            if (localIndex == -1)
            {
                throw new NotSupportedException($"Component with global index {globalIndex} is not initialized");
            }
            mask.SetBit(localIndex);
        }

        return filterDefinition.GlobalComponentIndexes.Count switch
        {
            2 => new HeavyFilterInstance<TMask, T1, T2>(
                filterDefinition.HeavyFilterId,
                _settings.MaxEntitiesCount,
                _settings.MaxEntitiesPerFilter,
                mask,
                _world,
                filterDefinition.GlobalComponentIndexes),
            3 => new HeavyFilterInstance<TMask, T1, T2, T3>(
                filterDefinition.HeavyFilterId,
                _settings.MaxEntitiesCount,
                _settings.MaxEntitiesPerFilter,
                mask,
                _world,
                filterDefinition.GlobalComponentIndexes),
            _ => throw new NotSupportedException($"Count: {filterDefinition.GlobalComponentIndexes.Count}")
        };
    }

    private void StoreFilter(HeavyFilterDefinition filterDefinition, HeavyFilterInstanceBase<TMask> filter)
    {
        foreach (var componentIndex in filterDefinition.GlobalComponentIndexes)
        {
            var filters = _heavyFiltersPerComponent[componentIndex];

            filters.Add(filter);
        }
    }

    private void ScanEntities(HeavyFilterInstanceBase<TMask> filter, int lastAddedGlobalComponentIndex)
    {
        var filterMask = filter.Mask;

        var lastAddedComponents = _world.GetComponentCollectionByGlobalIndex(lastAddedGlobalComponentIndex);
        var entities = lastAddedComponents.Entities;
        var entityMasks = _world._entityMasks;
        for (int i = 0; i < entities.Length; i++)
        {
            var entityId = entities[i];
            if (entityMasks[entityId.Index].Has(filterMask))
            {
                filter.AddEntity(entityId);
            }
        }
    }

    public void InitComponent<T>() where T : struct, IComponent
    {
        InitGlobalIndexMapping<T>();
    }

    public void InitTag<T>() where T : struct, ITagComponent
    {
        InitGlobalIndexMapping<T>();
    }

    private void InitGlobalIndexMapping<T>() where T : struct
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        if (_heavyFiltersPerComponent.Length <= globalIndex)
        {
            ArrayHelper.ResizeTwiceAndFill(
                ref _heavyFiltersPerComponent,
                () => new List<HeavyFilterInstanceBase<TMask>>(),
                Math.Max(_heavyFiltersPerComponent.Length << 1, ComponentMetadata.TotalCount));
        }
    }
}