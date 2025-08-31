using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters;

public readonly ref struct IndexedComponentCollection<T, TMask>
    where T : struct, IComponent
    where TMask : struct, IBitMask<TMask>
{
    private readonly SparseSet<T> _sparseSet; // SparseSet should be the first. Order of fields is important for performance!
    private readonly IndexedWorld<TMask> _world;
    private readonly List<FilterInstance<TMask>> _filters;
    private readonly int _componentLocalIndex;
    private readonly int _componentGlobalIndex;

#if HEAVY_FILTERS_ENABLED
    private readonly List<HeavyFilterInstanceBase<TMask>> _heavyFilters;
#endif

    internal int[] InternalIndexes => _sparseSet.InternalIndexes;
    internal T[] InternalData => _sparseSet.InternalData;

    public Span<T> Components
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _sparseSet.Components;
    }

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _sparseSet.Entities;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _sparseSet.Count;
    }

    internal IndexedComponentCollection(
        SparseSet<T> sparseSet,
        IndexedWorld<TMask> world,
        int componentGlobalIndex,
        int componentLocalIndex,
        List<FilterInstance<TMask>> filters)
    {
        _sparseSet = sparseSet;
        _world = world;
        _componentGlobalIndex = componentGlobalIndex;
        _componentLocalIndex = componentLocalIndex;
        _filters = filters;
    }

#if HEAVY_FILTERS_ENABLED
    internal IndexedComponentCollection(
        SparseSet<T> sparseSet,
        IndexedWorld<TMask> world,
        int componentGlobalIndex,
        int componentLocalIndex,
        List<FilterInstance<TMask>> filters,
        List<HeavyFilterInstanceBase<TMask>> heavyFilters)
    {
        _sparseSet = sparseSet;
        _world = world;
        _componentGlobalIndex = componentGlobalIndex;
        _componentLocalIndex = componentLocalIndex;
        _filters = filters;
        _heavyFilters = heavyFilters;
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset() => _sparseSet.Reset();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Restore(EntityId[] entities, T[] components, int[] indexes, int count)
        => _sparseSet.Restore(entities, components, indexes, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Backup(out EntityId[] entities, out T[] components, out int[] indexes, out int count)
        => _sparseSet.Backup(out entities, out components, out indexes, out count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent(EntityId entityId) => _sparseSet.HasComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent(EntityId entityId) => ref _sparseSet.GetComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, out ComponentRef<T> componentRef) =>
        _sparseSet.TryGetComponent(entityId, out componentRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, ref T component) =>
        _sparseSet.TryGetComponent(entityId, ref component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddComponent(EntityId entityId)
    {
        ref var component = ref _sparseSet.AddComponent(entityId);

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

        return ref component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteComponent(EntityId entityId)
    {
        var replacedEntityId = _sparseSet.SwapAndPopComponent(entityId);

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