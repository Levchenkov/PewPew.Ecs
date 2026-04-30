using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PewPew.Ecs.Core.Internals;

internal static class BlittableHelper
{
    private static readonly HashSet<Type> BlittablePrimitives = new()
    {
        typeof(byte), typeof(sbyte),
        typeof(short), typeof(ushort),
        typeof(int), typeof(uint),
        typeof(long), typeof(ulong),
        typeof(float), typeof(double),
        typeof(nint), typeof(nuint),
        typeof(BlittableBool), typeof(BlittableChar),
    };

    private static readonly HashSet<Type> NonBlittablePrimitives = new() { typeof(bool), typeof(char) };

    public static bool IsBlittable(Type type)
    {
        if (!type.IsValueType)
            return false;
        if (BlittablePrimitives.Contains(type))
            return true;
        if (type.IsEnum)
            return true;
        if (NonBlittablePrimitives.Contains(type))
            return false;

        var layout = type.StructLayoutAttribute;
        if (layout is { Value: LayoutKind.Auto })
            return false;

        return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   .All(f => IsBlittable(f.FieldType));
    }
}
