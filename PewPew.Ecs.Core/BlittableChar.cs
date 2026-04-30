using System.Runtime.InteropServices;

namespace PewPew.Ecs.Core;

/// <summary>
/// A blittable alternative to <see cref="char"/> for use in ECS component structs.
/// <para>
/// Plain <c>char</c> is non-blittable and cannot be used directly as a component field.
/// Use <c>BlittableChar</c> instead — it stores the UTF-16 code unit as a <see cref="ushort"/> and
/// implicitly converts to and from <c>char</c>.
/// </para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct BlittableChar : IEquatable<BlittableChar>
{
    private readonly ushort _value;

    private BlittableChar(ushort value) => _value = value;

    public static implicit operator BlittableChar(char value) => new BlittableChar(value);

    public static implicit operator char(BlittableChar value) => (char)value._value;

    public bool Equals(BlittableChar other) => _value == other._value;

    public override bool Equals(object? obj) => obj is BlittableChar other && Equals(other);

    public override int GetHashCode() => ((char)this).GetHashCode();

    public override string ToString() => ((char)this).ToString();
}
