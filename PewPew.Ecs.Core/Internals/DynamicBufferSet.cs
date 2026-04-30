using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core.Internals;

internal class DynamicBufferSet<T> : IComponentCollection
    where T : struct, IDynamicBufferComponent
{
    private const int InvalidIndex = -1;

    private readonly int _initialCapacity;
    private readonly IResizeStrategy _resizeStrategy;
    private readonly int[] _indexes;
    private DynamicBufferInstance<T>[] _instances;
    private EntityId[] _entities;
    private readonly IEntityManager _entityManager;

    private int _bufferCount;

    public Span<EntityId> Entities => new(_entities, 0, _bufferCount);

    public int BufferCount => _bufferCount;

    public DynamicBufferSet(int maxEntitiesCount, int maxComponentsCount, int initialCapacity, IResizeStrategy resizeStrategy, IEntityManager entityManager)
    {
        _initialCapacity = Math.Max(1, initialCapacity);
        _resizeStrategy = resizeStrategy;
        _entityManager = entityManager;
        _indexes = new int[maxEntitiesCount];
        _entities = new EntityId[maxComponentsCount];
        _instances = new DynamicBufferInstance<T>[maxComponentsCount];

        Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _bufferCount = 0;
        new Span<int>(_indexes).Fill(InvalidIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Reset();

    public bool HasBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        return _indexes[entityId.Index] != InvalidIndex;
    }

    public DynamicBuffer<T> GetBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var index = _indexes[entityId.Index];
        if (index == InvalidIndex)
            ThrowHelper.ThrowComponentNotFoundException<T>(entityId);

        return new DynamicBuffer<T>(_instances[index]);
    }

    public bool TryGetBuffer(EntityId entityId, out DynamicBuffer<T> dynamicBuffer)
    {
        DebugValidateEntityId(entityId);

        var index = _indexes[entityId.Index];
        if (index == InvalidIndex)
        {
            dynamicBuffer = default;
            return false;
        }

        dynamicBuffer = new DynamicBuffer<T>(_instances[index]);
        return true;
    }

    public DynamicBuffer<T> AddBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var index = ref _indexes[entityId.Index];
        if (index == InvalidIndex)
        {
            if (_bufferCount == _entities.Length)
                ResizeSlots();

            index = _bufferCount++;
            _entities[index] = entityId;
            _instances[index] = new DynamicBufferInstance<T>(_initialCapacity, _resizeStrategy);
        }

        return new DynamicBuffer<T>(_instances[index]);
    }

    public void DeleteBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var index = ref _indexes[entityId.Index];
        if (index == InvalidIndex)
            return;

#if DEBUG
        if (index < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(index, "Should be non negative.");

        if (index >= _bufferCount)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(index, "Should be less than Count");
#endif

        _instances[index].Invalidate();
        _bufferCount--;

        if (index == _bufferCount)
        {
            _instances[index] = null!;
            index = InvalidIndex;
            return;
        }

        // O(1) swap: move the last slot's instance reference
        _instances[index] = _instances[_bufferCount];
        _instances[_bufferCount] = null!;

        var replacedEntityId = _entities[index] = _entities[_bufferCount];
        _indexes[replacedEntityId.Index] = index;
        index = InvalidIndex;
    }

    private void ResizeSlots()
    {
        var newSize = _resizeStrategy.GetNewSize(_bufferCount);
        EnsureSlotsCapacity(newSize);
    }

    private void EnsureSlotsCapacity(int capacity)
    {
        if (capacity <= _entities.Length)
            return;

        Array.Resize(ref _entities, capacity);
        Array.Resize(ref _instances, capacity);
    }

    bool IComponentCollection.HasComponent(EntityId entityId) => HasBuffer(entityId);

    void IComponentCollection.DeleteComponent(EntityId entityId) => DeleteBuffer(entityId);

    [Conditional("DEBUG")]
    private void DebugValidateEntityId(EntityId entityId)
    {
        if (!_entityManager.IsAlive(entityId))
            ThrowHelper.ThrowNotSupportedException($"Component {typeof(T).Name}. Entity {entityId} is not alive.");
    }
}
