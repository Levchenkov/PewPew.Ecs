using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core.Internals;

internal sealed class TagSparseSet<T> : IContinuousSparseSet
    where T : struct, ITagComponent
{
    private const int InvalidIndex = -1;

    private readonly int[] _indexes;
    private EntityId[] _entities;
    private readonly IResizeStrategy _resizeStrategy;
    private readonly IEntityManager _entityManager;

    private int _count;

    public int[] InternalIndexes => _indexes;

    public Span<EntityId> Entities => new (_entities, 0, _count);

    public int Count => _count;

    public TagSparseSet(int maxEntitiesCount, int maxComponentsPerType, IResizeStrategy resizeStrategy, IEntityManager entityManager)
    {
        _resizeStrategy = resizeStrategy;
        _entityManager = entityManager;
        _indexes = new int[maxEntitiesCount];
#if DEBUG
        if (maxComponentsPerType < 0)
            throw new ArgumentOutOfRangeException(nameof(maxComponentsPerType), maxComponentsPerType, "Should be non negative.");
#endif
        _entities = new EntityId[maxComponentsPerType];
        _count = 0;

        Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _count = 0;
        new Span<int>(_indexes).Fill(InvalidIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Reset();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        return _indexes[entityId.Index] != InvalidIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var index = ref _indexes[entityId.Index];

        if (index != InvalidIndex)
            return;

        if (_count == _entities.Length)
            Resize();

        index = _count++;
        _entities[index] = entityId;
    }

    public void DeleteComponent(EntityId entityId) => SwapAndPopComponent(entityId);

    /// <summary>
    /// Deletes component for the entity by replacing with the last entity. Returns the last entity.
    /// </summary>
    public EntityId SwapAndPopComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var index = ref _indexes[entityId.Index];

        if (index == InvalidIndex)
            return EntityId.Invalid;

#if DEBUG
        if (index < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(index, "Should be non negative.");

        if (index >= _count)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(index, "Should be less than Count");
#endif

        _count--;

        if (index == _count)
        {
            index = InvalidIndex;

            return EntityId.Invalid;
        }

        var replacedEntityId = _entities[index] = _entities[_count];
        _indexes[replacedEntityId.Index] = index;

        index = InvalidIndex;

        return replacedEntityId;
    }

    private void Resize()
    {
        var newSize = _resizeStrategy.GetNewSize(_count);
        EnsureCapacity(newSize);
    }

    private void EnsureCapacity(int capacity)
    {
        if (capacity <= _entities.Length)
            return;

        Array.Resize(ref _entities, capacity);
    }

    [Conditional("DEBUG")]
    private void DebugValidateEntityId(EntityId entityId)
    {
        if(!_entityManager.IsAlive(entityId))
            ThrowHelper.ThrowNotSupportedException($"Component {typeof(T).Name}. Entity {entityId} is not alive.");
    }
}