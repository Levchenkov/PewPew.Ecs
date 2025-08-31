using System.Numerics;
using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Filters.Masks;

public struct BitMask64 : IBitMask<BitMask64>
{
    internal ulong _mask;

    public BitMask64()
    {
    }

    public BitMask64(ulong value)
    {
        _mask = value;
    }

    public static BitMask64 Zero => default;

    public readonly int Capacity => 64;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Has(in BitMask64 value)
    {
        var valueMask = value._mask;
        return (_mask & valueMask) == valueMask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitMask64 SetBit(int index)
    {
        _mask |= 1UL << index;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitMask64 SetBits(in BitMask64 value)
    {
        _mask |= value._mask;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitMask64 ClearBit(int index)
    {
        _mask &= ~(1UL << index);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitMask64 ClearBits(in BitMask64 value)
    {
        _mask &= ~value._mask;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool GetBit(int index)
    {
        return (_mask & (1UL << index)) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool HasIntersection(in BitMask64 value)
    {
        return (_mask & value._mask) != 0;
    }

    public bool Equals(BitMask64 other)
    {
        return _mask == other._mask;
    }

    public override bool Equals(object? obj)
    {
        return obj is BitMask64 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _mask.GetHashCode();
    }
}