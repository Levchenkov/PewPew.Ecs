using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Hybrid.Internals;

internal class CompactStaticBufferSet<T> : IComponentCollection
    where T : struct, IStaticBufferComponent
{
    private const int InvalidIndex = CompactSparseSet.InvalidIndex;
    private const int StaleDenseIndex = -1;
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
        _elementsCounts = new int[maxEntitiesCount];
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

        return denseIndex > 0;
    }

    public StaticBuffer<T> GetBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var denseIndex = ref CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        if (denseIndex <= 0)
            ThrowHelper.ThrowComponentNotFoundException<T>(entityId);

        return GetBufferByIndex(ref denseIndex, entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, out StaticBuffer<T> staticBuffer)
    {
        DebugValidateEntityId(entityId);

        ref var denseIndex = ref CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        if (denseIndex <= 0)
        {
            staticBuffer = default;

            return false;
        }

        staticBuffer = GetBufferByIndex(ref denseIndex, entityId);

        return true;
    }

    private StaticBuffer<T> GetBufferByIndex(ref int denseIndex, EntityId entityId)
    {
        ref var count = ref _elementsCounts[entityId.Index];

        return new StaticBuffer<T>(_components, ref denseIndex, ref count, _maxElementsCount);
    }

    public StaticBuffer<T> AddBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var denseIndex = ref CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, entityId);

        if (denseIndex > 0)
            return GetBufferByIndex(ref denseIndex, entityId);

        if (_bufferCount == Capacity)
            Resize();

        denseIndex = ++_bufferCount;
        _entities[denseIndex] = entityId;
        _elementsCounts[entityId.Index] = 0;

        return GetBufferByIndex(ref denseIndex, entityId);
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

        if (denseIndex <= 0)
            return EntityId.Invalid;

#if DEBUG
        if (denseIndex > _bufferCount)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(denseIndex, "Should be less than Count");
#endif

        if (denseIndex == _bufferCount)
        {
            denseIndex = StaleDenseIndex;

            _bufferCount--;

            return EntityId.Invalid;
        }

        var deletingBuffer = new Span<T>(_components, denseIndex * _maxElementsCount, _maxElementsCount);
        var lastBuffer = new Span<T>(_components, _bufferCount * _maxElementsCount, _elementsCounts[_entities[_bufferCount].Index]);
        lastBuffer.CopyTo(deletingBuffer);

        var replacedEntityId = _entities[denseIndex] = _entities[_bufferCount];

        CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, replacedEntityId) = denseIndex; // existing entity, so no page creation

        denseIndex = StaleDenseIndex;

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
    }

    [Conditional("DEBUG")]
    private void DebugValidateEntityId(EntityId entityId)
    {
        if(!_entityManager.IsAlive(entityId))
            ThrowHelper.ThrowNotSupportedException($"Component {typeof(T).Name}. Entity {entityId} is not alive.");
    }
}