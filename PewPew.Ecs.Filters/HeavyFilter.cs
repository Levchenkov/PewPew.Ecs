using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters;

public readonly ref struct HeavyFilter<TMask, T1, T2>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct
    where T2 : struct
{
    internal readonly HeavyFilterInstance<TMask, T1, T2> Instance;

    internal HeavyFilter(HeavyFilterInstance<TMask, T1, T2> heavyFilter)
    {
        Instance = heavyFilter;
    }

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Instance.Entities;
    }

    internal int Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => Instance.Id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntity(EntityId entityId) => Instance.HasEntity(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset() => Instance.Reset();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref T GetComponent<T>(int index) where T : struct, IComponent =>  ref Instance.GetComponent<T>(index);
}

public readonly ref struct HeavyFilter<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct
    where T2 : struct
    where T3 : struct
{
    internal readonly HeavyFilterInstance<TMask, T1, T2, T3> Instance;

    internal HeavyFilter(HeavyFilterInstance<TMask, T1, T2, T3> heavyFilter)
    {
        Instance = heavyFilter;
    }

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Instance.Entities;
    }

    internal int Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => Instance.Id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntity(EntityId entityId) => Instance.HasEntity(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset() => Instance.Reset();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref T GetComponent<T>(int index) where T : struct, IComponent =>  ref Instance.GetComponent<T>(index);
}