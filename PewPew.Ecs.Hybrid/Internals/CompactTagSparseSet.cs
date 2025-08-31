using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Hybrid.Internals;

internal sealed class CompactTagSparseSet<T> : IComponentCollection
    where T : struct, ITagComponent
{
    private const int InvalidIndex = CompactSparseSet.InvalidIndex;
    private const int PageShift = CompactSparseSet.PageShift;
    private const int PageSize = CompactSparseSet.PageSize;
    private const int PageMask = CompactSparseSet.PageMask;

    private readonly int[]?[] _sparseIndexPages;
    private EntityId[] _entities;
    private readonly IResizeStrategy _resizeStrategy;
    private readonly IEntityManager _entityManager;

    private int _count;

    public Span<EntityId> Entities => new(_entities, 1, _count);

    public int Count => _count;

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _entities.Length - 1;
    }

    public CompactTagSparseSet(int maxEntitiesCount, int maxComponentsPerType, IResizeStrategy resizeStrategy, IEntityManager entityManager)
    {
        _resizeStrategy = resizeStrategy;
        _entityManager = entityManager;
#if DEBUG
        if (maxComponentsPerType < 0)
            throw new ArgumentOutOfRangeException(nameof(maxComponentsPerType), maxComponentsPerType, "Should be non negative.");
#endif

        var pageCount = (maxEntitiesCount + PageSize - 1) >> PageShift;
        _sparseIndexPages = new int[pageCount][];
        _entities = new EntityId[maxComponentsPerType + 1]; // 0 index element reserved for non valid dense index
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        return denseIndex != InvalidIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var denseIndex = ref CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, entityId);

        if (denseIndex != InvalidIndex)
            return;

        if (_count == Capacity)
            Resize();

        denseIndex = ++_count;
        _entities[denseIndex] = entityId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteComponent(EntityId entityId) => SwapAndPopComponent(entityId);

    /// <summary>
    /// Deletes component for the entity by replacing with the last entity. Returns the last entity.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        if (denseIndex > _count)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(denseIndex, "Should be less than Count");
#endif

        if (denseIndex == _count)
        {
            denseIndex = InvalidIndex;

            _count--;

            return EntityId.Invalid;
        }

        var replacedEntityId = _entities[denseIndex] = _entities[_count];

        CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, replacedEntityId) = denseIndex; // existing entity, so no page creation

        denseIndex = InvalidIndex;

        _count--;

        return replacedEntityId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _count = 0;
        CompactSparseSet.Reset(_sparseIndexPages);
    }

    private void Resize()
    {
        var newSize = _resizeStrategy.GetNewSize(_count);
        EnsureCapacity(newSize);
    }

    private void EnsureCapacity(int capacity)
    {
        if (capacity <= Capacity)
            return;

        Array.Resize(ref _entities, capacity + 1);
    }

    [Conditional("DEBUG")]
    private void DebugValidateEntityId(EntityId entityId)
    {
        if(!_entityManager.IsAlive(entityId))
            ThrowHelper.ThrowNotSupportedException($"Component {typeof(T).Name}. Entity {entityId} is not alive.");
    }
}