using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.Hybrid;

public readonly ref partial struct StaticArchetype<TMask, T1, T2>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4, TT5> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4, TT5, TT6> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        where TT6 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4, TT5> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4, TT5, TT6> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        where TT6 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4, TT5> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4, TT5, TT6> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        where TT6 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4, T5>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4, T5>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4, T5>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4, T5>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4, TT5, TT6> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        where TT6 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4, T5, T6>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4, T5, T6>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4, T5, T6>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4>(entityId, target.Instance);
}

public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3, T4, T5, T6>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2, TT3, TT4, TT5> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        => _instance.MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(entityId, target.Instance);
}

