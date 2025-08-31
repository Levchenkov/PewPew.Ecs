using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public readonly ref struct StaticBufferCollection<T>
    where T : struct, IStaticBufferComponent
{
    private readonly StaticBufferSet<T> _staticBufferSet; // StaticBufferSet should be the first. Order of fields is important for performance!

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _staticBufferSet.Entities;
    }

    public int BufferCount => _staticBufferSet.BufferCount;

    internal StaticBufferCollection(StaticBufferSet<T> staticBufferSet)
    {
        _staticBufferSet = staticBufferSet;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _staticBufferSet.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasBuffer(EntityId entityId) => _staticBufferSet.HasBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticBuffer<T> GetBuffer(EntityId entityId) => _staticBufferSet.GetBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetBuffer(EntityId entityId, out StaticBuffer<T> staticBuffer)
        => _staticBufferSet.TryGetBuffer(entityId, out staticBuffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticBuffer<T> AddBuffer(EntityId entityId) => _staticBufferSet.AddBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteBuffer(EntityId entityId) => _staticBufferSet.DeleteBuffer(entityId);
}