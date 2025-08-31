using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PewPew.Ecs.Core;

namespace PewPew.Ecs.Hybrid;

public ref struct ComponentRef<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
#if NETSTANDARD2_1 || NET7_0

    private Span<T1> _span1;
    private Span<T2> _span2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2)
    {
        _span1 = MemoryMarshal.CreateSpan(ref c1, 1);
        _span2 = MemoryMarshal.CreateSpan(ref c2, 1);
    }

    public ref T1 Component1
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span1);
    }

    public ref T2 Component2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span2);
    }

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2)
    {
        Component1 = ref c1;
        Component2 = ref c2;
    }

    public ref T1 Component1;
    public ref T2 Component2;

#endif
}

public ref struct ComponentRef<T1, T2, T3>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
#if NETSTANDARD2_1 || NET7_0

    private Span<T1> _span1;
    private Span<T2> _span2;
    private Span<T3> _span3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2, ref T3 c3)
    {
        _span1 = MemoryMarshal.CreateSpan(ref c1, 1);
        _span2 = MemoryMarshal.CreateSpan(ref c2, 1);
        _span3 = MemoryMarshal.CreateSpan(ref c3, 1);
    }

    public ref T1 Component1
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span1);
    }

    public ref T2 Component2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span2);
    }

    public ref T3 Component3
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span3);
    }

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2, ref T3 c3)
    {
        Component1 = ref c1;
        Component2 = ref c2;
        Component3 = ref c3;
    }

    public ref T1 Component1;
    public ref T2 Component2;
    public ref T3 Component3;

#endif
}

public ref struct ComponentRef<T1, T2, T3, T4>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
#if NETSTANDARD2_1 || NET7_0

    private Span<T1> _span1;
    private Span<T2> _span2;
    private Span<T3> _span3;
    private Span<T4> _span4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4)
    {
        _span1 = MemoryMarshal.CreateSpan(ref c1, 1);
        _span2 = MemoryMarshal.CreateSpan(ref c2, 1);
        _span3 = MemoryMarshal.CreateSpan(ref c3, 1);
        _span4 = MemoryMarshal.CreateSpan(ref c4, 1);
    }

    public ref T1 Component1
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span1);
    }

    public ref T2 Component2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span2);
    }

    public ref T3 Component3
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span3);
    }

    public ref T4 Component4
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span4);
    }

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4)
    {
        Component1 = ref c1;
        Component2 = ref c2;
        Component3 = ref c3;
        Component4 = ref c4;
    }

    public ref T1 Component1;
    public ref T2 Component2;
    public ref T3 Component3;
    public ref T4 Component4;

#endif
}

public ref struct ComponentRef<T1, T2, T3, T4, T5>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
#if NETSTANDARD2_1 || NET7_0

    private Span<T1> _span1;
    private Span<T2> _span2;
    private Span<T3> _span3;
    private Span<T4> _span4;
    private Span<T5> _span5;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5)
    {
        _span1 = MemoryMarshal.CreateSpan(ref c1, 1);
        _span2 = MemoryMarshal.CreateSpan(ref c2, 1);
        _span3 = MemoryMarshal.CreateSpan(ref c3, 1);
        _span4 = MemoryMarshal.CreateSpan(ref c4, 1);
        _span5 = MemoryMarshal.CreateSpan(ref c5, 1);
    }

    public ref T1 Component1
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span1);
    }

    public ref T2 Component2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span2);
    }

    public ref T3 Component3
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span3);
    }

    public ref T4 Component4
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span4);
    }

    public ref T5 Component5
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span5);
    }

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5)
    {
        Component1 = ref c1;
        Component2 = ref c2;
        Component3 = ref c3;
        Component4 = ref c4;
        Component5 = ref c5;
    }

    public ref T1 Component1;
    public ref T2 Component2;
    public ref T3 Component3;
    public ref T4 Component4;
    public ref T5 Component5;

#endif
}

public ref struct ComponentRef<T1, T2, T3, T4, T5, T6>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
#if NETSTANDARD2_1 || NET7_0

    private Span<T1> _span1;
    private Span<T2> _span2;
    private Span<T3> _span3;
    private Span<T4> _span4;
    private Span<T5> _span5;
    private Span<T6> _span6;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6)
    {
        _span1 = MemoryMarshal.CreateSpan(ref c1, 1);
        _span2 = MemoryMarshal.CreateSpan(ref c2, 1);
        _span3 = MemoryMarshal.CreateSpan(ref c3, 1);
        _span4 = MemoryMarshal.CreateSpan(ref c4, 1);
        _span5 = MemoryMarshal.CreateSpan(ref c5, 1);
        _span6 = MemoryMarshal.CreateSpan(ref c6, 1);
    }

    public ref T1 Component1
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span1);
    }

    public ref T2 Component2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span2);
    }

    public ref T3 Component3
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span3);
    }

    public ref T4 Component4
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span4);
    }

    public ref T5 Component5
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span5);
    }

    public ref T6 Component6
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span6);
    }

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef(ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6)
    {
        Component1 = ref c1;
        Component2 = ref c2;
        Component3 = ref c3;
        Component4 = ref c4;
        Component5 = ref c5;
        Component6 = ref c6;
    }

    public ref T1 Component1;
    public ref T2 Component2;
    public ref T3 Component3;
    public ref T4 Component4;
    public ref T5 Component5;
    public ref T6 Component6;

#endif
}