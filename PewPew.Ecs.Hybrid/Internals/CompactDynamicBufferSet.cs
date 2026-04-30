using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Hybrid.Internals;

internal class CompactDynamicBufferSet<T> : IComponentCollection
    where T : struct, IDynamicBufferComponent
{
    private const int InvalidIndex = CompactSparseSet.InvalidIndex; // 0
    private const int StaleDenseIndex = -1;
    private const int PageShift = CompactSparseSet.PageShift;
    private const int PageSize = CompactSparseSet.PageSize;
    private const int PageMask = CompactSparseSet.PageMask;

    private readonly int _initialCapacity;
    private readonly IResizeStrategy _resizeStrategy;
    private readonly int[]?[] _sparseIndexPages;
    private DynamicBufferInstance<T>[] _instances;
    private EntityId[] _entities;
    private readonly IEntityManager _entityManager;

    private int _bufferCount;

    public Span<EntityId> Entities => new(_entities, 1, _bufferCount);

    public int BufferCount => _bufferCount;

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _entities.Length - 1;
    }

    public CompactDynamicBufferSet(int maxEntitiesCount, int maxComponentsCount, int initialCapacity, IResizeStrategy resizeStrategy, IEntityManager entityManager)
    {
        _initialCapacity = Math.Max(1, initialCapacity);
        _resizeStrategy = resizeStrategy;
        _entityManager = entityManager;

        var pageCount = (maxEntitiesCount + PageSize - 1) >> PageShift;
        _sparseIndexPages = new int[pageCount][];
        _instances = new DynamicBufferInstance<T>[maxComponentsCount + 1]; // index 0 reserved as invalid sentinel
        _entities = new EntityId[maxComponentsCount + 1];

        Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _bufferCount = 0;
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

    public DynamicBuffer<T> GetBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        if (denseIndex <= 0)
            ThrowHelper.ThrowComponentNotFoundException<T>(entityId);

        return new DynamicBuffer<T>(_instances[denseIndex]);
    }

    public bool TryGetBuffer(EntityId entityId, out DynamicBuffer<T> dynamicBuffer)
    {
        DebugValidateEntityId(entityId);

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        if (denseIndex <= 0)
        {
            dynamicBuffer = default;
            return false;
        }

        dynamicBuffer = new DynamicBuffer<T>(_instances[denseIndex]);
        return true;
    }

    public DynamicBuffer<T> AddBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var denseIndex = ref CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, entityId);

        if (denseIndex > 0)
            return new DynamicBuffer<T>(_instances[denseIndex]);

        if (_bufferCount == Capacity)
            Resize();

        denseIndex = ++_bufferCount;
        _entities[denseIndex] = entityId;
        _instances[denseIndex] = new DynamicBufferInstance<T>(_initialCapacity, _resizeStrategy);

        return new DynamicBuffer<T>(_instances[denseIndex]);
    }

    public void DeleteBuffer(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var entityIndex = entityId.Index;
        var pageIndex = entityIndex >> PageShift;

        var page = _sparseIndexPages[pageIndex];
        if (page == null)
            return;

        var offset = entityIndex & PageMask;

        ref var denseIndex = ref page[offset];

        if (denseIndex <= 0)
            return;

#if DEBUG
        if (denseIndex > _bufferCount)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(denseIndex, "Should be less than or equal to BufferCount");
#endif

        _instances[denseIndex].Invalidate();

        if (denseIndex == _bufferCount)
        {
            _instances[denseIndex] = null!;
            denseIndex = StaleDenseIndex;
            _bufferCount--;
            return;
        }

        // O(1) swap: move the last slot into the freed slot
        _instances[denseIndex] = _instances[_bufferCount];
        _instances[_bufferCount] = null!;

        var replacedEntityId = _entities[denseIndex] = _entities[_bufferCount];
        CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, replacedEntityId) = denseIndex; // page already exists

        denseIndex = StaleDenseIndex;
        _bufferCount--;
    }

    private void Resize()
    {
        var newSize = _resizeStrategy.GetNewSize(_bufferCount);
        EnsureCapacity(newSize);
    }

    private void EnsureCapacity(int capacity)
    {
        if (capacity <= Capacity)
            return;

        Array.Resize(ref _entities, capacity + 1);
        Array.Resize(ref _instances, capacity + 1);
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
