using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if !NETSTANDARD2_1
using System.Numerics;
using System.Runtime.Intrinsics;
#endif

namespace PewPew.Ecs.Filters.Masks;

[StructLayout(LayoutKind.Explicit)]
public struct BitMask128 : IBitMask<BitMask128>
{
    [FieldOffset(0)]
    private ulong _lower;

    [FieldOffset(8)]
    private ulong _upper;

#if !NETSTANDARD2_1
    [FieldOffset(0)]
    private Vector128<ulong> _mask;
#endif

    public BitMask128(ulong lower)
    {
        _lower = lower;
    }

    public BitMask128(ulong lower, ulong upper)
    {
        _lower = lower;
        _upper = upper;
    }

    public static BitMask128 Zero => default;

    public int Capacity => 128;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Has(in BitMask128 value)
    {
#if NETSTANDARD2_1
        return (_lower & value._lower) == value._lower
               && (_upper & value._upper) == value._upper;
#else
        return (_mask & value._mask) == value._mask;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool HasIntersection(in BitMask128 value)
    {
#if NETSTANDARD2_1
        return (_lower & value._lower) != 0
               && (_upper & value._upper) != 0;
#else
        return (_mask & value._mask) != Vector128<ulong>.Zero;
#endif
    }

    public BitMask128 SetBit(int index)
    {
        switch (index)
        {
            case < 64:
                _lower |= 1UL << index;
                return this;
            default:
                _upper |= 1UL << (index - 64);
                return this;
        }
    }

    public BitMask128 SetBits(in BitMask128 value)
    {
#if NETSTANDARD2_1
        _lower |= value._lower;
        _upper |= value._upper;
#else
        _mask |= value._mask;
#endif

        return this;
    }

    public BitMask128 ClearBit(int index)
    {
        switch (index)
        {
            case < 64:
                _lower &= ~(1UL << index);
                return this;
            default:
                _upper &= ~(1uL << (index - 64));
                return this;
        }
    }

    public BitMask128 ClearBits(in BitMask128 value)
    {
#if NETSTANDARD2_1
        _lower &= ~value._lower;
        _upper &= ~value._upper;
#else
        _mask &= ~value._mask;
#endif
        return this;
    }

    public readonly bool GetBit(int index)
    {
        switch (index)
        {
            case < 64:
                return (_lower & (1UL << index)) != 0;
            default:
                return (_upper & (1UL << (index - 64))) != 0;
        }
    }

    public bool Equals(BitMask128 other)
    {
#if NETSTANDARD2_1
        return _lower.Equals(other._lower)
               && _upper.Equals(other._upper);
#else
        return _mask.Equals(other._mask);
#endif
    }

    public override bool Equals(object? obj)
    {
        return obj is BitMask128 other && Equals(other);
    }

    public override int GetHashCode()
    {
#if NETSTANDARD2_1
        var hashCode = new HashCode();
        hashCode.Add(_lower);
        hashCode.Add(_upper);
        return hashCode.ToHashCode();
#else
        return _mask.GetHashCode();
#endif
    }
}