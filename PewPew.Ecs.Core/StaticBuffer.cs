using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public ref struct StaticBuffer<T>
    where T : struct
{
    private const int InvalidIndex = -1;

    private readonly T[] _components;

#if NETSTANDARD2_1
    private Ref<int> _index;
    private Ref<int> _count;
#else
    private ref int _index;
    private ref int _count;
#endif

    public Span<T> Components => new(_components, Start, Count);

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
        private set
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

    private int Start
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            DebugValidateBufferIsAlive();

#if NETSTANDARD2_1
            return _index.Value * Capacity;
#else
            return _index * Capacity;
#endif
        }
    }

    public StaticBuffer(T[] components, ref int index, ref int count, int maxCount)
    {
        _components = components;

#if NETSTANDARD2_1
        _index = new Ref<int>(ref index);
        _count = new Ref<int>(ref count);
#else
        _index = ref index;
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

        _components[Start + Count++] = component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T RemoveLast()
    {
#if DEBUG
        if(Count == 0)
            ThrowHelper.ThrowCollectionIsEmptyException<T>();
#endif

        --Count;

        return _components[Start + Count];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SwapAndPop(int index)
    {
#if DEBUG
        if (index >= Count || index < 0)
            ThrowHelper.ThrowCollectionIsEmptyException<T>();
#endif

        --Count;

        var absoluteIndex = Start + index;
        if (index == Count)
            return _components[absoluteIndex];

        var result = _components[absoluteIndex];
        _components[absoluteIndex] = _components[Start + Count];

        return result;
    }

    public void Clear()
    {
        Count = 0;
    }

    [Conditional("DEBUG")]
    private void DebugValidateBufferIsAlive()
    {
#if NETSTANDARD2_1
        if(_index.Value == InvalidIndex)
#else
        if(_index == InvalidIndex)
#endif
            ThrowHelper.ThrowNotSupportedException("This ref is stale. Retrieve a new buffer for the entity.");
    }
}