using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Hybrid.Internals;

internal class CompactStaticBufferSet<T> : IComponentCollection
    where T : struct, IStaticBufferComponent
{
    private const int InvalidIndex = CompactSparseSet.InvalidIndex;
    private const int PageShift = CompactSparseSet.PageShift;
    private const int PageSize = CompactSparseSet.PageSize;
    private const int PageMask = CompactSparseSet.PageMask;

    private readonly int _maxElementsCount;
    private readonly int[]?[] _sparseIndexPages;
    private T[] _components;
    private EntityId[] _entities;
    private int[] _elementsCounts;
    private readonly IResizeStrategy _resizeStrategy;
    private readonly IEntityManager _entityManager;

    private int _bufferCount;

    public Span<EntityId> Entities => new(_entities, 1, _bufferCount);

    public int BufferCount => _bufferCount;

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _entities.Length - 1;
    }

    public CompactStaticBufferSet(int maxEntitiesCount, int maxComponentsCount, int maxElementsCount, IResizeStrategy resizeStrategy, IEntityManager entityManager)
    {
        _maxElementsCount = maxElementsCount;
        _resizeStrategy = resizeStrategy;
        _entityManager = entityManager;

        var pageCount = (maxEntitiesCount + PageSize - 1) >> PageShift;
        _sparseIndexPages = new int[pageCount][];
        _entities = new EntityId[maxComponentsCount + 1]; // 0 index element reserved for non valid dense index
        _elementsCounts = new int[maxComponentsCount + 1];
        _components = new T[(maxComponentsCount + 1) * _maxElementsCount];

        Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _bufferCount = 0;
        new Span<int>(_elementsCounts).Clear();

        CompactSparseSet.Reset(_sparseIndexPages);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Reset();

    public bool HasBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        return denseIndex != InvalidIndex;
    }

    public StaticBuffer<T> GetBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        DebugValidateIndex(denseIndex, entityId);

        return GetBufferByIndex(denseIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, out StaticBuffer<T> staticBuffer)
    {
        DebugValidateEntityId(entityId);

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        if (denseIndex == InvalidIndex)
        {
            staticBuffer = default;

            return false;
        }

        staticBuffer = GetBufferByIndex(denseIndex);

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

        ref var denseIndex = ref CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, entityId);

        if (denseIndex != InvalidIndex)
            return GetBufferByIndex(denseIndex);

        if (_bufferCount == Capacity)
            Resize();

        denseIndex = ++_bufferCount;
        _entities[denseIndex] = entityId;
        _elementsCounts[denseIndex] = 0;

        return GetBufferByIndex(denseIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteBuffer(EntityId entityId)
    {
        SwapAndPopComponent(entityId);
    }

    public EntityId SwapAndPopComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var entityIndex = entityId.Index;
        var pageIndex = entityIndex >> PageShift;

        var page = _sparseIndexPages[pageIndex];
        if (page == null)
            return EntityId.Invalid;

        var offset = entityIndex & PageMask;

        ref var denseIndex = ref page[offset];

        if (denseIndex == InvalidIndex)
            return EntityId.Invalid;

#if DEBUG
        if (denseIndex < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(denseIndex, "Should be non negative.");

        if (denseIndex > _bufferCount)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(denseIndex, "Should be less than Count");
#endif

        if (denseIndex == _bufferCount)
        {
            denseIndex = InvalidIndex;

            _bufferCount--;

            return EntityId.Invalid;
        }

        var deletingBuffer = new Span<T>(_components, denseIndex * _maxElementsCount, _maxElementsCount);
        var lastBuffer = new Span<T>(_components, _bufferCount * _maxElementsCount, _elementsCounts[_bufferCount]);
        lastBuffer.CopyTo(deletingBuffer);

        _elementsCounts[denseIndex] = _elementsCounts[_bufferCount];

        var replacedEntityId = _entities[denseIndex] = _entities[_bufferCount];

        CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, replacedEntityId) = denseIndex; // existing entity, so no page creation

        denseIndex = InvalidIndex;

        _bufferCount--;

        return replacedEntityId;
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

    [Conditional("DEBUG")]
    private void DebugValidateIndex(int index, EntityId entityId)
    {
        if (index == InvalidIndex)
            ThrowHelper.ThrowComponentNotFoundException<T>(entityId);
    }
}