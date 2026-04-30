using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public readonly ref struct DynamicBufferCollection<T>
    where T : struct, IDynamicBufferComponent
{
    private readonly DynamicBufferSet<T> _dynamicBufferSet;

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _dynamicBufferSet.Entities;
    }

    public int BufferCount => _dynamicBufferSet.BufferCount;

    internal DynamicBufferCollection(DynamicBufferSet<T> dynamicBufferSet)
    {
        _dynamicBufferSet = dynamicBufferSet;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _dynamicBufferSet.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasBuffer(EntityId entityId) => _dynamicBufferSet.HasBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DynamicBuffer<T> GetBuffer(EntityId entityId) => _dynamicBufferSet.GetBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetBuffer(EntityId entityId, out DynamicBuffer<T> dynamicBuffer)
        => _dynamicBufferSet.TryGetBuffer(entityId, out dynamicBuffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DynamicBuffer<T> AddBuffer(EntityId entityId) => _dynamicBufferSet.AddBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteBuffer(EntityId entityId) => _dynamicBufferSet.DeleteBuffer(entityId);
}
