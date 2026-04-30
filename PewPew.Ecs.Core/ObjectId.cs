using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core;

/// <summary>
/// A versioned handle to an object stored in an <see cref="ObjectStorage{T}"/>.
/// <para>
/// Packs <c>StorageId</c> (bits 48–63), <c>Generation</c> (bits 32–47), and <c>Index</c> (bits 0–31)
/// into a single <see cref="ulong"/>. <c>default</c> equals <see cref="Invalid"/>.
/// </para>
/// </summary>
public readonly struct ObjectId : IEquatable<ObjectId>, IObjectId
{
    public static readonly ObjectId Invalid = default;

    public readonly ulong Value;

    public int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)Value;
    }

    public ushort Generation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ushort)(Value >> 32);
    }

    public ushort StorageId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ushort)(Value >> 48);
    }

    internal ObjectId(int index, ushort generation, ushort storageId)
    {
        Value = (ulong)storageId << 48 | (ulong)generation << 32 | (uint)index;
    }

    public bool Equals(ObjectId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is ObjectId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => $"ObjectId {StorageId}|{Generation}|{Index}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ObjectId a, ObjectId b) => a.Value == b.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ObjectId a, ObjectId b) => a.Value != b.Value;
}
