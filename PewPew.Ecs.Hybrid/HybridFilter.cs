using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.Hybrid;

public readonly ref struct HybridFilter<TMask>
    where TMask : struct, IBitMask<TMask>
{
    private readonly HybridFilterInstance<TMask> _instance;

    internal HybridFilter(HybridFilterInstance<TMask> hybridFilter)
    {
        _instance = hybridFilter;
    }

    internal Span<EntityId> SparseEntities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _instance.SparseEntities;
    }

    internal int Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => _instance.Id;
    }

    internal TMask Mask
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => _instance.Mask;
    }

    internal HybridWorld<TMask> World
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _instance.World;
    }

    internal IStaticArchetypeInstance<TMask>[] FullArchetypes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _instance.FullArchetypes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntity(EntityId entityId) => _instance.HasEntity(entityId);

    [Conditional("DEBUG")]
    internal void DebugValidateFilterComponent<T>()
        where T : struct
    {
        if (!World.HasFilterComponent<T>(this))
            throw new NotSupportedException($"Filter {Id} does not have a component of type {typeof(T).Name}");
    }
}