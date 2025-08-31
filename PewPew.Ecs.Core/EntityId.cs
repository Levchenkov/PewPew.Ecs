using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public readonly partial struct EntityId : IEquatable<EntityId>, IPoolableId
{
    public static readonly EntityId Invalid = default;

    public readonly ulong Value;

    internal int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)Value;
    }

    internal ushort Generation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ushort)(Value >> 32);
    }

    internal ushort WorldId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ushort)(Value >> 48);
    }

    internal EntityId(int index, ushort generation, ushort worldId)
    {
        Value = (ulong)((ulong)worldId << 48 | (ulong)generation << 32 | (uint)index);
    }

    public bool Equals(EntityId other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is EntityId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"EntityId {WorldId}|{Generation}|{Index}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator == (EntityId first, EntityId second)
    {
        return first.Value == second.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator != (EntityId first, EntityId second)
    {
        return first.Value != second.Value;
    }

    int IPoolableId.GetIndex() => Index;

    ushort IPoolableId.GetGeneration() => Generation;

    ushort IPoolableId.GetWorldId() =>  WorldId;
}