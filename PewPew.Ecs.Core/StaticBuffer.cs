using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public ref struct StaticBuffer<T>
    where T : struct
{
    private readonly T[] _components;
    private readonly int _start;

#if NETSTANDARD2_1
    private Ref<int> _count;
#else
    private ref int _count;
#endif

    public Span<T> Components => new(_components, _start, Count);

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
#if NETSTANDARD2_1
        return _count.Value;
#else
        return _count;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
#if NETSTANDARD2_1
            _count.Value = value;
#else
            _count = value;
#endif
        }
    }

    public int Capacity { get; }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Count == 0;
    }

    public bool IsFull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Count == Capacity;
    }

    public StaticBuffer(T[] components, int start, ref int count, int maxCount)
    {
        _components = components;
        _start = start;

#if NETSTANDARD2_1
        _count = new Ref<int>(ref count);
#else
         _count = ref count;
#endif

        Capacity = maxCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddLast(T component)
    {
#if DEBUG
        if(Count == Capacity)
            ThrowHelper.ThrowMaxCapacityException<T>();
#endif

        _components[_start + Count++] = component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T RemoveLast()
    {
#if DEBUG
        if(Count == 0)
            ThrowHelper.ThrowCollectionIsEmptyException<T>();
#endif

        Count--;

        return _components[_start + Count];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SwapAndPop(int index)
    {
#if DEBUG
        if (index >= Count || index < 0)
            ThrowHelper.ThrowCollectionIsEmptyException<T>();
#endif

        Count--;

        var absoluteIndex = _start + index;
        if (index == Count)
            return _components[absoluteIndex];

        var result = _components[absoluteIndex];
        _components[absoluteIndex] = _components[_start + Count];

        return result;
    }

    public void Clear()
    {
        Count = 0;
    }
}