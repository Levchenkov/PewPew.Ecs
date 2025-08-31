using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters.Internals;

internal sealed class FilterModule<TMask, TFilter>
    where TMask : struct, IBitMask<TMask>
    where TFilter : class
{
    private readonly Dictionary<int, FilterDefinitionBase> _filterDefinitions;

    private int[] _globalToLocalComponentIndexMap;
    private int _componentsCount;

    private List<TFilter>[] _filtersPerComponent;
    private int _nextFilterId;

    public FilterModule(int maxAllowedUniqueComponentsCount)
    {
        _filtersPerComponent = new List<TFilter>[10];
        for (int i = 0; i < _filtersPerComponent.Length; i++)
        {
            _filtersPerComponent[i] = new List<TFilter>();
        }

        _filterDefinitions = new Dictionary<int, FilterDefinitionBase>();

        _globalToLocalComponentIndexMap = new int[maxAllowedUniqueComponentsCount];
        Array.Fill(_globalToLocalComponentIndexMap, -1);
    }

    internal void InitComponent<T>()
        where T : struct, IComponent
    {
        InitGlobalIndexMapping<T>();
    }

    internal void InitTag<T>()
        where T : struct, ITagComponent
    {
        InitGlobalIndexMapping<T>();
    }

    private void InitGlobalIndexMapping<T>() where T : struct
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        if (globalIndex >= _globalToLocalComponentIndexMap.Length)
        {
            var oldLength = _globalToLocalComponentIndexMap.Length;
            Array.Resize(ref _globalToLocalComponentIndexMap, Math.Max(_globalToLocalComponentIndexMap.Length << 1, ComponentMetadata.TotalCount));
            Array.Fill(_globalToLocalComponentIndexMap, -1, oldLength, _globalToLocalComponentIndexMap.Length - oldLength);
        }

        _globalToLocalComponentIndexMap[globalIndex] = _componentsCount++;

        if (_filtersPerComponent.Length <= globalIndex)
        {
            ArrayHelper.ResizeTwiceAndFill(
                ref _filtersPerComponent,
                () => new List<TFilter>(),
                Math.Max(_filtersPerComponent.Length << 1, ComponentMetadata.TotalCount));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetLocalIndex(int componentGlobalIndex) => _globalToLocalComponentIndexMap[componentGlobalIndex];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal List<TFilter> GetFilters(int globalComponentIndex) => _filtersPerComponent[globalComponentIndex];

    internal TFilter FetchOrCreateFilter(
        object world,
        ushort worldId,
        FilterDefinitionBase filterDefinition,
        TFilter[] filters,
        int maxAllowedUniqueComponentsCount,
        Func<int, TMask, object, TFilter> filterFactory,
        Action<TFilter, int> scanEntities)
    {
        if(filterDefinition.GlobalComponentIndexes.Count > maxAllowedUniqueComponentsCount)
            ThrowHelper.ThrowNotSupportedException($"Max filter component count is {maxAllowedUniqueComponentsCount}.");

        filterDefinition.Compile();

        if (CheckFilterExists(filterDefinition, filters, out var existedFilter))
            return existedFilter;

        filterDefinition.FilterId = _nextFilterId++;
        filterDefinition.WorldId = worldId;
        var filterHash = filterDefinition.GetHashCode();
        _filterDefinitions[filterHash] = filterDefinition;

        var filter = CreateFilter(filterDefinition, world, filterFactory);
        StoreFilter(filterDefinition, filter);
        scanEntities(filter, filterDefinition.LastAddedGlobalComponentIndex);

        return filter;
    }

    private bool CheckFilterExists(FilterDefinitionBase filterDefinition, TFilter[] filters, out TFilter filter)
    {
        if (filterDefinition.FilterId == FilterDefinitionBase.InvalidId)
        {
            var hash = filterDefinition.GetHashCode();
            if (_filterDefinitions.TryGetValue(hash, out var existedFilterDefinition) && existedFilterDefinition.Equals(filterDefinition))
            {
                filterDefinition.FilterId = existedFilterDefinition.FilterId;
                filterDefinition.WorldId = existedFilterDefinition.WorldId;
            }
            else
            {
                filter = null!;

                return false;
            }
        }

        filter = filters[filterDefinition.FilterId];

        return filter != null!;
    }

    private TFilter CreateFilter(FilterDefinitionBase filterDefinition, object world, Func<int, TMask, object, TFilter> filterFactory)
    {
        if (!filterDefinition.IsCompiled)
            throw new NotSupportedException();

        if(filterDefinition.FilterId == FilterDefinitionBase.InvalidId)
            throw new NotSupportedException();

        var mask = default(TMask);

        foreach (var globalIndex in filterDefinition.GlobalComponentIndexes)
        {
            var localIndex = _globalToLocalComponentIndexMap[globalIndex];
            if (localIndex == -1)
            {
                throw new NotSupportedException($"Component with global index {globalIndex} is not initialized");
            }
            mask.SetBit(localIndex);
        }

        var filter = filterFactory(filterDefinition.FilterId, mask, world);

        return filter;
    }

    private void StoreFilter(FilterDefinitionBase filterDefinition, TFilter filter)
    {
        foreach (var globalIndex in filterDefinition.GlobalComponentIndexes)
        {
            var filters = _filtersPerComponent[globalIndex];

            filters.Add(filter);
        }
    }
}