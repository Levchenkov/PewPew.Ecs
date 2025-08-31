using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Hybrid.Internals;

internal readonly struct ArchetypeDenseIndexPair : IEquatable<ArchetypeDenseIndexPair>
{
    public static ArchetypeDenseIndexPair Invalid = new (-1, -1);

    public readonly ulong Value;

    internal int DenseIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)Value;
    }

    // todo: should we swap ArchetypeId and DenseIndex?
    internal int ArchetypeId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)(Value >> 32);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ArchetypeDenseIndexPair(int archetypeId, int denseIndex)
    {
        Value = (ulong)archetypeId << 32 | (uint)denseIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ArchetypeDenseIndexPair other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is ArchetypeDenseIndexPair other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"ArchetypeDenseId {ArchetypeId}|{DenseIndex}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator == (ArchetypeDenseIndexPair first, ArchetypeDenseIndexPair second)
    {
        return first.Value == second.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator != (ArchetypeDenseIndexPair first, ArchetypeDenseIndexPair second)
    {
        return first.Value != second.Value;
    }
}