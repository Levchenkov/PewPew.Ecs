using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters;

public readonly ref struct Filter<TMask>
    where TMask : struct, IBitMask<TMask>
{
    private readonly FilterInstance<TMask> _filter;

    internal Filter(FilterInstance<TMask> filter)
    {
        _filter = filter;
    }

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _filter.Entities;
    }

    internal int Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => _filter.Id;
    }

    internal TMask Mask
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => _filter.Mask;
    }

    internal IndexedWorld<TMask> World
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _filter.World;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntity(EntityId entityId) => _filter.HasEntity(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset() => _filter.Reset();

    [Conditional("DEBUG")]
    internal void DebugValidateFilterComponent<T>()
        where T : struct
    {
        if (!World.HasFilterComponent<T>(this))
            throw new NotSupportedException($"Filter {Id} does not have a component of type {typeof(T).Name}");
    }
}