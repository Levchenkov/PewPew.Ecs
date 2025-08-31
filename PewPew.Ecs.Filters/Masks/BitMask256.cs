using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if !NETSTANDARD2_1
using System.Numerics;
using System.Runtime.Intrinsics;
#endif

namespace PewPew.Ecs.Filters.Masks;

[StructLayout(LayoutKind.Explicit)]
public struct BitMask256 : IBitMask<BitMask256>
{
    [FieldOffset(0)]
    internal ulong _l0;

    [FieldOffset(8)]
    internal ulong _l1;

    [FieldOffset(16)]
    internal ulong _l2;

    [FieldOffset(24)]
    internal ulong _l3;

#if !NETSTANDARD2_1
    [FieldOffset(0)]
    private Vector256<ulong> _mask;
#endif

    public BitMask256(ulong[] values)
    {
        if (values.Length != 4)
        {
            throw new ArgumentException("Array must contain exactly 4 elements.");
        }
#if NETSTANDARD2_1
        _l0 = values[0];
        _l1 = values[1];
        _l2 = values[2];
        _l3 = values[3];
#else
        _mask = Vector256.Create(values[0], values[1], values[2], values[3]);
#endif

    }

    public BitMask256(ulong value)
    {
#if NETSTANDARD2_1
        _l0 = value;
        _l1 = value;
        _l2 = value;
        _l3 = value;
#else
        _mask = Vector256.Create(value);
#endif

    }

    public static BitMask256 Zero => default;

    public int Capacity => 256;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Has(in BitMask256 value)
    {
#if NETSTANDARD2_1
        return (_l0 & value._l0) == value._l0
               && (_l1 & value._l1) == value._l1
               && (_l2 & value._l2) == value._l2
               && (_l3 & value._l3) == value._l3;
#else
        return (_mask & value._mask) == value._mask;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool HasIntersection(in BitMask256 value)
    {
#if NETSTANDARD2_1
        return (_l0 & value._l0) != 0
               && (_l1 & value._l1) != 0
               && (_l2 & value._l2) != 0
               && (_l3 & value._l3) != 0;
#else
        return (_mask & value._mask) != Vector256<ulong>.Zero;
#endif
    }

    public BitMask256 SetBit(int index)
    {
        switch (index) {
            case < 64:      _l0 |= 1UL <<  index;          return this;
            case < 128:     _l1 |= 1UL << (index - 64);    return this;
            case < 192:     _l2 |= 1UL << (index - 128);   return this;
            default:        _l3 |= 1UL << (index - 192);   return this;
        }
    }

    public BitMask256 SetBits(in BitMask256 value)
    {
#if NETSTANDARD2_1
        _l0 |= value._l0;
        _l1 |= value._l1;
        _l2 |= value._l2;
        _l3 |= value._l3;
#else
        _mask |= value._mask;
#endif

        return this;
    }

    public BitMask256 ClearBit(int index)
    {
        switch (index) {
            case < 64:      _l0 &= ~(1UL <<  index);          return this;
            case < 128:     _l1 &= ~(1UL << (index - 64));    return this;
            case < 192:     _l2 &= ~(1UL << (index - 128));   return this;
            default:        _l3 &= ~(1UL << (index - 192));   return this;
        }
    }

    public BitMask256 ClearBits(in BitMask256 value)
    {
#if NETSTANDARD2_1
        _l0 &= ~value._l0;
        _l1 &= ~value._l1;
        _l2 &= ~value._l2;
        _l3 &= ~value._l3;
#else
        _mask &= ~value._mask;
#endif
        return this;
    }

    public readonly bool GetBit(int index)
    {
        switch (index) {
            case < 64:      return (_l0 & (1UL <<  index))        != 0;
            case < 128:     return (_l1 & (1UL << (index -  64))) != 0;
            case < 192:     return (_l2 & (1UL << (index - 128))) != 0;
            default:        return (_l3 & (1UL << (index - 192))) != 0;
        }
    }

    public override string ToString()
    {
        return $"{Convert.ToString((long)_l3, 2).PadLeft(64, '0')}" +
               $"{Convert.ToString((long)_l2, 2).PadLeft(64, '0')}" +
               $"{Convert.ToString((long)_l1, 2).PadLeft(64, '0')}" +
               $"{Convert.ToString((long)_l0, 2).PadLeft(64, '0')}";
    }

    public bool Equals(BitMask256 other)
    {
#if NETSTANDARD2_1
        return _l0.Equals(other._l0)
               && _l1.Equals(other._l1)
               && _l2.Equals(other._l2)
               && _l3.Equals(other._l3);
#else
        return _mask.Equals(other._mask);
#endif
    }

    public override bool Equals(object? obj)
    {
        return obj is BitMask256 other && Equals(other);
    }

    public override int GetHashCode()
    {
#if NETSTANDARD2_1
        var hashCode = new HashCode();
        hashCode.Add(_l0);
        hashCode.Add(_l1);
        hashCode.Add(_l2);
        hashCode.Add(_l3);
        return hashCode.ToHashCode();
#else
        return _mask.GetHashCode();
#endif
    }
}