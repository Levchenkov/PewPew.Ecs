using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Exceptions;

namespace PewPew.Ecs.Core.Internals;

internal static class ThrowHelper
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowMaxCapacityException<T>() where T : struct
    {
        throw new MaxCapacityException(typeof(T));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowEntityNotFoundException(EntityId entityId)
    {
        throw new NotSupportedException($"Entity {entityId} not found.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowComponentNotFoundException(int globalIndex)
    {
        throw new Exception($"Component with global index {globalIndex} not found.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowComponentNotFoundException<T>()
    {
        throw new ComponentNotFoundException(typeof(T));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowComponentNotFoundException(Type type, EntityId entityId)
    {
        throw new ComponentNotFoundException(type, entityId);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowComponentNotFoundException<T>(EntityId entityId)
    {
        throw new ComponentNotFoundException(typeof(T), entityId);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowComponentNotFoundException<T>(int id)
    {
        throw new ComponentNotFoundException(typeof(T), id);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentOutOfRangeException(int index, string message)
    {
        throw new ArgumentOutOfRangeException(nameof(index), index, message);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentOutOfRangeException<T>(int index, string message)
    {
        throw new ArgumentOutOfRangeException(nameof(index), index, $"[{typeof(T).Name}] {message}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidEntityIdException(EntityId entityId, int minEntityIndex, int maxEntityIndex)
    {
        ThrowInvalidEntityIdException($"Entity {entityId} is not in range [{minEntityIndex}, {maxEntityIndex}]");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidEntityIdException<T>(T id, int minEntityIndex, int maxEntityIndex)
    {
        ThrowInvalidEntityIdException($"Id {id} is not in range [{minEntityIndex}, {maxEntityIndex}]");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidEntityIdException(string message)
    {
        throw new ArgumentException(message);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidRangeIdException(int rangeId)
    {
        throw new ArgumentException(rangeId.ToString());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowCollectionIsEmptyException<T>()
    {
        throw new ArgumentException(typeof(T).Name);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNonBlittableComponentException<T>()
    {
        throw new NotSupportedException(
            $"Component type '{typeof(T).FullName}' is not blittable. " +
            "All component types must be blittable structs (no bool, char, managed references, or LayoutKind.Auto).");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNotSupportedException(string message)
    {
        throw new NotSupportedException(message);
    }
}