using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core.Internals;

internal class DynamicBufferInstance<T>
    where T : struct, IDynamicBufferComponent
{
    private readonly int _initialCapacity;
    private readonly IResizeStrategy _resizeStrategy;
    private T[] _components;
    private int _count;

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _count;
    }

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _components.Length;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _count == 0;
    }

    public Span<T> Components
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_components, 0, _count);
    }

    internal DynamicBufferInstance(int initialCapacity, IResizeStrategy resizeStrategy)
    {
        _initialCapacity = Math.Max(1, initialCapacity);
        _resizeStrategy = resizeStrategy;
        _components = new T[_initialCapacity];
        _count = 0;
    }

    internal void Invalidate()
    {
        _components = null!;
        _count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddLast(T component)
    {
        if (_count == _components.Length)
            Grow();

        _components[_count++] = component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T RemoveLast()
    {
#if DEBUG
        if (_count == 0)
            ThrowHelper.ThrowCollectionIsEmptyException<T>();
#endif
        return _components[--_count];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SwapAndPop(int index)
    {
#if DEBUG
        if (index >= _count || index < 0)
            ThrowHelper.ThrowCollectionIsEmptyException<T>();
#endif
        _count--;

        if (index == _count)
            return _components[index];

        var result = _components[index];
        _components[index] = _components[_count];
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        _count = 0;
    }

    public void EnsureCapacity(int minCapacity)
    {
        if (minCapacity < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException(minCapacity, "Capacity cannot be negative.");

        if (_components.Length >= minCapacity)
            return;

        var newArray = new T[minCapacity];
        if (_count > 0)
            Array.Copy(_components, newArray, _count);
        _components = newArray;
    }

    public void TrimExcess()
    {
        var newSize = Math.Max(_count, _initialCapacity);
        if (_components.Length == newSize)
            return;

        var newArray = new T[newSize];
        if (_count > 0)
            Array.Copy(_components, newArray, _count);
        _components = newArray;
    }

    private void Grow()
    {
        var newSize = _resizeStrategy.GetNewSize(_components.Length);
        var newArray = new T[newSize];
        if (_count > 0)
            Array.Copy(_components, newArray, _count);
        _components = newArray;
    }
}
