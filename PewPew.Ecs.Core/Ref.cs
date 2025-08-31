using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PewPew.Ecs.Core;

public ref struct Ref<T>
{
#if NETSTANDARD2_1 || NET7_0

    private Span<T> _span;

    public Ref(ref T value)
    {
        _span = MemoryMarshal.CreateSpan(ref value, 1);
    }

    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span);
    }

#else

    public Ref(ref T value)
    {
        Value = ref value;
    }

    public ref T Value;

#endif
}