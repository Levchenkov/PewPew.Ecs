using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core.Internals;

internal class StaticBufferSet<T> : IComponentCollection
    where T : struct, IStaticBufferComponent
{
    private const int InvalidIndex = -1;

    private readonly int _maxElementsCount;
    private readonly int[] _indexes;
    private T[] _components;
    private EntityId[] _entities;
    private int[] _elementsCounts;
    private readonly IResizeStrategy _resizeStrategy;
    private readonly IEntityManager _entityManager;

    private int _bufferCount;

    public Span<EntityId> Entities => new(_entities, 0, _bufferCount);

    public int BufferCount => _bufferCount;

    public StaticBufferSet(int maxEntitiesCount, int maxComponentsCount, int maxElementsCount, IResizeStrategy resizeStrategy, IEntityManager entityManager)
    {
        _maxElementsCount = maxElementsCount;
        _resizeStrategy = resizeStrategy;
        _entityManager = entityManager;
        _indexes = new int[maxEntitiesCount];
        _entities = new EntityId[maxComponentsCount];
        _elementsCounts = new int[maxComponentsCount];
        _components = new T[maxComponentsCount * _maxElementsCount];

        Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _bufferCount = 0;
        new Span<int>(_indexes).Fill(InvalidIndex);
        new Span<int>(_elementsCounts).Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Reset();

    public bool HasBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var index = _indexes[entityId.Index];
        if (index == InvalidIndex)
            return false;

        return true;
    }

    public StaticBuffer<T> GetBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var index = _indexes[entityId.Index];
        if(index == InvalidIndex)
            ThrowHelper.ThrowComponentNotFoundException<T>(entityId);

        return GetBufferByIndex(index);
    }

    public bool TryGetBuffer(EntityId entityId, out StaticBuffer<T> staticBuffer)
    {
        DebugValidateEntityId(entityId);

        var index = _indexes[entityId.Index];

        if (index == InvalidIndex)
        {
            staticBuffer = default;

            return false;
        }

        staticBuffer = GetBufferByIndex(index);

        return true;
    }

    private StaticBuffer<T> GetBufferByIndex(int index)
    {
        var start = index * _maxElementsCount;
        ref var count = ref _elementsCounts[index];

        return new StaticBuffer<T>(_components, start, ref count, _maxElementsCount);
    }

    public StaticBuffer<T> AddBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var index = ref _indexes[entityId.Index];
        if (index != InvalidIndex)
            return GetBufferByIndex(index);

        if (_bufferCount == _entities.Length)
            Resize();

        index = _bufferCount++;
        _entities[index] = entityId;
        _elementsCounts[index] = 0;

        return GetBufferByIndex(index);
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

        _bufferCount--;

        if (index == _bufferCount)
        {
            index = InvalidIndex;

            return;
        }

        var deletingBuffer = new Span<T>(_components, index * _maxElementsCount, _maxElementsCount);
        var lastBuffer = new Span<T>(_components, _bufferCount * _maxElementsCount, _elementsCounts[_bufferCount]);
        lastBuffer.CopyTo(deletingBuffer);

        _elementsCounts[index] = _elementsCounts[_bufferCount];
        var replacedEntityId = _entities[index] = _entities[_bufferCount];
        _indexes[replacedEntityId.Index] = index;
        index = InvalidIndex;
    }

    bool IComponentCollection.HasComponent(EntityId entityId) => HasBuffer(entityId);

    void IComponentCollection.DeleteComponent(EntityId entityId) => DeleteBuffer(entityId);

    private void Resize()
    {
        var newSize = _resizeStrategy.GetNewSize(_bufferCount);
        EnsureCapacity(newSize);
    }

    private void EnsureCapacity(int capacity)
    {
        if (capacity <= _entities.Length)
            return;

        Array.Resize(ref _components, capacity * _maxElementsCount);
        Array.Resize(ref _entities, capacity);
        Array.Resize(ref _elementsCounts, capacity);
    }

    [Conditional("DEBUG")]
    private void DebugValidateEntityId(EntityId entityId)
    {
        if(!_entityManager.IsAlive(entityId))
            ThrowHelper.ThrowNotSupportedException($"Component {typeof(T).Name}. Entity {entityId} is not alive.");
    }
}