using System.Runtime.InteropServices;
using System.Text;

namespace PewPew.Ecs.Core;


/// <summary>
/// A blittable, fixed-capacity UTF-8 string for use in ECS component structs.
/// <para>
/// Stores up to <see cref="Capacity"/> UTF-8 bytes inline without heap allocation.
/// Use <see cref="AsReadOnlySpan"/> for zero-copy reads and comparisons.
/// <see cref="ToString"/> is available but allocates — prefer it only for debugging.
/// </para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct BlittableString32 : IEquatable<BlittableString32>
{
    public const int Capacity = 32;

    private fixed byte _bytes[Capacity];
    private byte _length;

    /// <summary>Number of UTF-8 bytes currently stored.</summary>
    public int Length => _length;

    /// <summary>True when no bytes are stored.</summary>
    public bool IsEmpty => _length == 0;

    internal Span<byte> Bytes => MemoryMarshal.CreateSpan(ref _bytes[0], Capacity);

    /// <summary>
    /// Returns a read-only view of the stored UTF-8 bytes. Zero-copy, no allocation.
    /// </summary>
    public ReadOnlySpan<byte> AsReadOnlySpan()
    {
        return MemoryMarshal.CreateReadOnlySpan(ref _bytes[0], _length);
    }

    /// <summary>
    /// Copies UTF-8 bytes into this instance, truncating to <see cref="Capacity"/> if the input is longer.
    /// Returns the number of bytes actually stored.
    /// </summary>
    public int Set(ReadOnlySpan<byte> utf8)
    {
        var count = Math.Min(utf8.Length, Capacity);

        utf8.Slice(0, count).CopyTo(Bytes);
        _length = (byte)count;

        return count;
    }

    /// <summary>
    /// Tries to copy UTF-8 bytes into this instance.
    /// Returns <c>false</c> without modifying the instance when the input exceeds <see cref="Capacity"/>.
    /// </summary>
    public bool TrySet(ReadOnlySpan<byte> utf8)
    {
        if (utf8.Length > Capacity)
            return false;

        Set(utf8);

        return true;
    }

    /// <summary>
    /// Creates an <see cref="BlittableString32"/> by encoding <paramref name="value"/> as UTF-8.
    /// Returns <c>false</c> when the encoded byte count exceeds <see cref="Capacity"/>.
    /// </summary>
    public static bool TryCreate(string value, out BlittableString32 result)
    {
        result = default;

        var byteCount = Encoding.UTF8.GetByteCount(value);
        if (byteCount > Capacity)
            return false;

        Encoding.UTF8.GetBytes(value, result.Bytes);
        result._length = (byte)byteCount;

        return true;
    }

    /// <summary>
    /// Decodes the stored UTF-8 bytes to a managed string. Allocates.
    /// Prefer <see cref="AsReadOnlySpan"/> for comparisons and hot-path iteration.
    /// </summary>
    public override string ToString()
    {
        return Encoding.UTF8.GetString(AsReadOnlySpan());
    }

    public bool Equals(BlittableString32 other) => AsReadOnlySpan().SequenceEqual(other.AsReadOnlySpan());

    public override bool Equals(object? obj) => obj is BlittableString32 other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var b in AsReadOnlySpan())
            hash.Add(b);
        return hash.ToHashCode();
    }

    public static bool operator ==(BlittableString32 left, BlittableString32 right) => left.Equals(right);
    public static bool operator !=(BlittableString32 left, BlittableString32 right) => !left.Equals(right);
}
