using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core.Internals;

internal class DenseSet<T>
    where T : struct
{
    public const int MinCapacity = 10;

    private T[] _components;
    private EntityId[] _entities;
    private int _count;

    private readonly IResizeStrategy _resizeStrategy;

    public int Count => _count;

    public Span<T> Components => new(_components, 0, _count);

    public Span<EntityId> Entities => new (_entities, 0, _count);

    public DenseSet(int capacity = MinCapacity, IResizeStrategy? resizeStrategy = null)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Should be non negative.");
        _resizeStrategy = resizeStrategy ?? new DoubleSizeStrategy(MinCapacity);

        capacity = Math.Max(capacity, MinCapacity);
        _components = new T[capacity];
        _entities = new EntityId[capacity];
        _count = 0;
    }

    public void Reset()
    {
        _count = 0;
    }

    public void EnsureCapacity(int capacity)
    {
        if (capacity <= _components.Length)
            return;

        Array.Resize(ref _components, capacity);
        Array.Resize(ref _entities, capacity);
    }

    public ref T Add(EntityId entityId)
    {
        if (_count == _components.Length)
        {
            var newSize = _resizeStrategy.GetNewSize(_count);
            EnsureCapacity(newSize);
        }

        _entities[_count] = entityId;
        return ref _components[_count++];
    }

    public int FindIndex(EntityId entityId)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_entities[i] == entityId)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Deletes component by index.
    /// </summary>
    /// <returns>EntityId that replace deleted entity</returns>
    public EntityId Delete(int index)
    {
        _count--;

        _components[index] = _components[_count];
        _components[_count] = default;

        var replacedId = _entities[index] = _entities[_count];

        return replacedId;
    }
}