using System.Runtime.InteropServices;

namespace PewPew.Ecs.Core;

/// <summary>
/// A blittable alternative to <see cref="bool"/> for use in ECS component structs.
/// <para>
/// Plain <c>bool</c> is non-blittable and cannot be used directly as a component field.
/// Use <c>BlittableBool</c> instead — it stores the value as a <see cref="byte"/> and
/// implicitly converts to and from <c>bool</c>.
/// </para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct BlittableBool : IEquatable<BlittableBool>
{
    private readonly byte _value;

    private BlittableBool(byte value) => _value = value;

    public static implicit operator BlittableBool(bool value) => new BlittableBool(value ? (byte)1 : (byte)0);

    public static implicit operator bool(BlittableBool value) => value._value != 0;

    public bool Equals(BlittableBool other) => _value == other._value;

    public override bool Equals(object? obj) => obj is BlittableBool other && Equals(other);

    public override int GetHashCode() => ((bool)this).GetHashCode();

    public override string ToString() => ((bool)this).ToString();
}
