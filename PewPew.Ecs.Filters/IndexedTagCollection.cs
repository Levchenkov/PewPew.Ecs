using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters;

public readonly ref struct IndexedTagCollection<T, TMask>
    where T : struct, ITagComponent
    where TMask : struct, IBitMask<TMask>
{
    private readonly TagSparseSet<T> _tagSparseSet; // SparseSet should be the first. Order of fields is important for performance!
    private readonly IndexedWorld<TMask> _world;
    private readonly List<FilterInstance<TMask>> _filters;
    private readonly int _componentLocalIndex;
    private readonly int _componentGlobalIndex;

#if HEAVY_FILTERS_ENABLED
    private readonly List<HeavyFilterInstanceBase<TMask>> _heavyFilters;
#endif

    internal int[] InternalIndexes => _tagSparseSet.InternalIndexes;

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tagSparseSet.Entities;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tagSparseSet.Count;
    }

    internal IndexedTagCollection(
        TagSparseSet<T> sparseSet,
        IndexedWorld<TMask> world,
        int componentGlobalIndex,
        int componentLocalIndex,
        List<FilterInstance<TMask>> filters)
    {
        _tagSparseSet = sparseSet;
        _world = world;
        _componentGlobalIndex = componentGlobalIndex;
        _componentLocalIndex = componentLocalIndex;
        _filters = filters;
    }

#if HEAVY_FILTERS_ENABLED
    internal IndexedTagCollection(
        TagSparseSet<T> sparseSet,
        IndexedWorld<TMask> world,
        int componentGlobalIndex,
        int componentLocalIndex,
        List<FilterInstance<TMask>> filters,
        List<HeavyFilterInstanceBase<TMask>> heavyFilters)
    {
        _tagSparseSet = sparseSet;
        _world = world;
        _componentGlobalIndex = componentGlobalIndex;
        _componentLocalIndex = componentLocalIndex;
        _filters = filters;
        _heavyFilters = heavyFilters;
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset() => _tagSparseSet.Reset();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent(EntityId entityId) => _tagSparseSet.HasComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent(EntityId entityId)
    {
        _tagSparseSet.AddComponent(entityId);

#if HEAVY_FILTERS_ENABLED
        if (_heavyFilters.Count == 0)
        {
            _world.OnComponentAdded(entityId, _componentLocalIndex, _filters);
        }
        else
        {
            _world.OnComponentAdded(entityId, _componentLocalIndex, _filters, _heavyFilters);
        }
#else
        _world.OnComponentAdded(entityId, _componentLocalIndex, _filters);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteComponent(EntityId entityId)
    {
        var replacedEntityId = _tagSparseSet.SwapAndPopComponent(entityId);

#if HEAVY_FILTERS_ENABLED
        if (_heavyFilters.Count == 0)
        {
            _world.OnComponentDeleted(entityId, _componentLocalIndex, _filters);
        }
        else
        {
            _world.OnComponentDeleted(entityId, replacedEntityId, _componentGlobalIndex, _componentLocalIndex, _filters, _heavyFilters);
        }
#else
        _world.OnComponentDeleted(entityId, _componentLocalIndex, _filters);
#endif
    }
}