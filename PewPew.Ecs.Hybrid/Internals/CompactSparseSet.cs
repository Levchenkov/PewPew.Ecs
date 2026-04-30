using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Hybrid.Internals;

internal sealed class CompactSparseSet<T> : IComponentCollection
    where T : struct, IComponent
{
    private const int InvalidIndex = CompactSparseSet.InvalidIndex;
    private const int PageShift = CompactSparseSet.PageShift;
    private const int PageSize = CompactSparseSet.PageSize;
    private const int PageMask = CompactSparseSet.PageMask;

    private readonly int[]?[] _sparseIndexPages;
    private T[] _components;
    private EntityId[] _entities;
    private readonly IResizeStrategy _resizeStrategy;
    private readonly IEntityManager _entityManager;

    private int _count;

    public Span<T> Components => new(_components, 1, _count);

    public Span<EntityId> Entities => new(_entities, 1, _count);

    public int Count => _count;

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _components.Length - 1;
    }

    public CompactSparseSet(int maxEntitiesCount, int maxComponentsPerType, IResizeStrategy resizeStrategy, IEntityManager entityManager)
    {
        _resizeStrategy = resizeStrategy;
        _entityManager = entityManager;
#if DEBUG
        if (maxComponentsPerType < 0)
            throw new ArgumentOutOfRangeException(nameof(maxComponentsPerType), maxComponentsPerType, "Should be non negative.");
#endif

        var pageCount = (maxEntitiesCount + PageSize - 1) >> PageShift;
        _sparseIndexPages = new int[pageCount][];
        _components = new T[maxComponentsPerType + 1]; // 0 index element reserved for non valid dense index
        _entities = new EntityId[maxComponentsPerType + 1];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        Reset();
        new Span<T>(_components).Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        return denseIndex != InvalidIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        DebugValidateIndex(denseIndex, entityId);

        return ref GetComponentByDenseIndex(denseIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, out ComponentRef<T> componentRef)
    {
        DebugValidateEntityId(entityId);

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        if (denseIndex == InvalidIndex)
        {
            componentRef = default;

            return false;
        }

        ref var component = ref GetComponentByDenseIndex(denseIndex);
        componentRef = new ComponentRef<T>(ref component);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, ref T component)
    {
        DebugValidateEntityId(entityId);

        component = default;

        var denseIndex = CompactSparseSet.GetDenseIndexDoNotCreatePage(_sparseIndexPages, entityId);

        if (denseIndex == InvalidIndex)
            return false;

        component = ref GetComponentByDenseIndex(denseIndex);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var denseIndex = ref CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, entityId);

        if (denseIndex != InvalidIndex)
            return ref GetComponentByDenseIndex(denseIndex);

        if (_count == Capacity)
            Resize();

        denseIndex = ++_count;
        _entities[denseIndex] = entityId;

        return ref _components[denseIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteComponent(EntityId entityId) => SwapAndPopComponent(entityId);

    /// <summary>
    /// Deletes component for the entity by replacing with the last entity. Returns the last entity.
    /// </summary>
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
            _components[_count] = default;
            denseIndex = InvalidIndex;

            _count--;

            return EntityId.Invalid;
        }

        _components[denseIndex] = _components[_count];
        _components[_count] = default;

        var replacedEntityId = _entities[denseIndex] = _entities[_count];

        CompactSparseSet.GetDenseIndexRefCreatePage(_sparseIndexPages, replacedEntityId) = denseIndex; // existing entity, so no page creation

        denseIndex = InvalidIndex;

        _count--;

        return replacedEntityId;
    }

    public void AddComponents(DenseSet<T> denseSet)
    {
        var entities = denseSet.Entities;
        var components = denseSet.Components;
        for (int i = 0; i < denseSet.Count; i++)
        {
            AddComponent(entities[i]) = components[i];
        }
    }

    public void DeleteComponents(Span<EntityId> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            SwapAndPopComponent(entities[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _count = 0;
        CompactSparseSet.Reset(_sparseIndexPages);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref T GetComponentByDenseIndex(int index)
    {
#if DEBUG
        if (index < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(index, "Should be non negative.");

        if (index > _count)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(index, "Should be less than Count");
#endif

        return ref _components[index];
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

        Array.Resize(ref _components, capacity + 1);
        Array.Resize(ref _entities, capacity + 1);
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

internal class CompactSparseSet
{
    public const int InvalidIndex = 0;
    public const int PageShift = 12; // 1<<12 == 4096 , 85_000 bytes object goes to LOH
    public const int PageSize = 1 << PageShift;
    public const int PageMask = PageSize - 1;

    private static int InvalidIndexRef = InvalidIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref int GetDenseIndexDoNotCreatePage(int[]?[] sparseIndexPages, EntityId entityId)
    {
        var entityIndex = entityId.Index;
        var pageIndex = entityIndex >> PageShift;

        var page = sparseIndexPages[pageIndex];
        if (page == null)
            return ref InvalidIndexRef;

        var offset = entityIndex & PageMask;

        ref var denseIndex = ref page[offset];

        return ref denseIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref int GetDenseIndexRefCreatePage(int[]?[] sparseIndexPages, EntityId entityId)
    {
        var entityIndex = entityId.Index;
        var pageIndex = entityIndex >> PageShift;

        ref var page = ref sparseIndexPages[pageIndex];
        if (page == null)
            page = CreatePage();

        var offset = entityIndex & PageMask;

        ref var denseIndex = ref page[offset];

        return ref denseIndex;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int[] CreatePage()
    {
        var page = new int[PageSize];

        return page;
    }

    public static void Reset(int[]?[] sparseIndexPages)
    {
        foreach (var sparseIndexPage in sparseIndexPages)
        {
            if(sparseIndexPage == null)
                continue;

            ResetPage(sparseIndexPage);
        }
    }

    private static void ResetPage(int[] sparseIndexPage)
    {
        new Span<int>(sparseIndexPage).Fill(InvalidIndex);
    }
}