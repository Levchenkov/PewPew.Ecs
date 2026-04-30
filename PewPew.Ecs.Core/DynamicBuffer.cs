using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public ref struct DynamicBuffer<T>
    where T : struct, IDynamicBufferComponent
{
    private readonly DynamicBufferInstance<T> _instance;

    public Span<T> Components
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _instance.Components;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _instance.Count;
    }

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _instance.Capacity;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _instance.IsEmpty;
    }

    internal DynamicBuffer(DynamicBufferInstance<T> instance)
    {
        _instance = instance;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddLast(T component) => _instance.AddLast(component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T RemoveLast() => _instance.RemoveLast();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SwapAndPop(int index) => _instance.SwapAndPop(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _instance.Clear();

    public void EnsureCapacity(int minCapacity) => _instance.EnsureCapacity(minCapacity);

    public void TrimExcess() => _instance.TrimExcess();
}
