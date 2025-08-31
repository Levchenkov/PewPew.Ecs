using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core.Internals;

internal sealed class SparseSet<T> : IContinuousSparseSet
    where T : struct, IComponent
{
    private const int InvalidIndex = -1;

    private readonly int[] _indexes;
    private T[] _components;
    private EntityId[] _entities;
    private readonly IResizeStrategy _resizeStrategy;
    private readonly IEntityManager _entityManager;

    private int _count;

    public T[] InternalData => _components;

    public int[] InternalIndexes => _indexes;

    public Span<T> Components => new(_components, 0, _count);

    public Span<EntityId> Entities => new(_entities, 0, _count);

    public int Count => _count;

    public int Capacity => _components.Length;

    public SparseSet(int maxEntitiesCount, int maxComponentsPerType, IResizeStrategy resizeStrategy, IEntityManager entityManager)
    {
        _resizeStrategy = resizeStrategy;
        _entityManager = entityManager;
#if DEBUG
        if (maxComponentsPerType < 0)
            throw new ArgumentOutOfRangeException(nameof(maxComponentsPerType), maxComponentsPerType, "Should be non negative.");
#endif
        _indexes = new int[maxEntitiesCount];
        _components = new T[maxComponentsPerType];
        _entities = new EntityId[maxComponentsPerType];

        Reset();
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

        return _indexes[entityId.Index] != InvalidIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var index = _indexes[entityId.Index];

        DebugValidateIndex(index, entityId);

        return ref GetByIndex(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, out ComponentRef<T> componentRef)
    {
        DebugValidateEntityId(entityId);

        var index = _indexes[entityId.Index];

        if (index == InvalidIndex)
        {
            componentRef = default;

            return false;
        }

        ref var component = ref GetByIndex(index);
        componentRef = new ComponentRef<T>(ref component);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, ref T component)
    {
        DebugValidateEntityId(entityId);

        component = default;

        var index = _indexes[entityId.Index];

        if (index == InvalidIndex)
            return false;

        component = ref GetByIndex(index);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var index = ref _indexes[entityId.Index];

        if (index != InvalidIndex)
            return ref GetByIndex(index);

        if (_count == _components.Length)
            Resize();

        index = _count++;
        _entities[index] = entityId;

        return ref _components[index];
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
            _components[_count] = default;
            index = InvalidIndex;

            return EntityId.Invalid;
        }

        _components[index] = _components[_count];
        _components[_count] = default;

        var replacedEntityId = _entities[index] = _entities[_count];
        _indexes[replacedEntityId.Index] = index;

        index = InvalidIndex;

        return replacedEntityId;
    }

    public void AddComponents(DenseSet<T> denseSet)
    {
        if (_count + denseSet.Count >= _components.Length)
        {
            var newSize = _count;
            do
            {
                newSize = _resizeStrategy.GetNewSize(newSize);
            } while (newSize < _count + denseSet.Count);

            EnsureCapacity(newSize);
        }

        var components = denseSet.Components;
        var entities = denseSet.Entities;

        var blockSize = 0;
        for (int i = 0; i < denseSet.Count; i++)
        {
            var entityId = entities[i];
            var internalIndex = GetInternalIndex(entityId);
            if (internalIndex != InvalidIndex)
            {
                GetByIndex(internalIndex) = components[i];
                if (blockSize > 0)
                {
                    CopyBlock(entities.Slice(i - blockSize, blockSize), components.Slice(i - blockSize, blockSize));
                }

                blockSize = 0;
            }
            else
            {
                blockSize++;
            }
        }

        if (blockSize > 0)
        {
            CopyBlock(entities.Slice(denseSet.Count - blockSize, blockSize), components.Slice(denseSet.Count - blockSize, blockSize));
        }
    }

    private void CopyBlock(Span<EntityId> entities, Span<T> components)
    {
#if DEBUG
        if (_count + components.Length >= _components.Length)
            ThrowHelper.ThrowMaxCapacityException<T>();
#endif

        entities.CopyTo(_entities.AsSpan(_count));
        components.CopyTo(_components.AsSpan(_count));

        for (int i = 0; i < entities.Length; i++)
        {
            var entityId = entities[i];
            _indexes[entityId.Index] = _count++;
        }
    }

    public void DeleteComponents(List<EntityId> entities)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            SwapAndPopComponent(entities[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _count = 0;
        new Span<int>(_indexes).Fill(InvalidIndex);
    }

    internal void Backup(out EntityId[] entities, out T[] components, out int[] indexes, out int count)
    {
        entities = _entities.ToArray();
        components = _components.ToArray();
        indexes = _indexes.ToArray();
        count = _count;
    }

    internal void Restore(EntityId[] entities, T[] components, int[] indexes, int count)
    {
        _count = count;
        entities.CopyTo(_entities.AsSpan());
        components.CopyTo(_components.AsSpan());
        indexes.CopyTo(_indexes.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetInternalIndex(EntityId entityId) => _indexes[entityId.Index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref T GetByIndex(int index)
    {
#if DEBUG
        if (index < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException<T>(index, "Should be non negative.");

        if (index >= _count)
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
        if (capacity <= _components.Length)
            return;

        Array.Resize(ref _components, capacity);
        Array.Resize(ref _entities, capacity);
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