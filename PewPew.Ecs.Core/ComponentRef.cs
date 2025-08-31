using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PewPew.Ecs.Core;

public ref struct ComponentRef<T>
{
    #if NETSTANDARD2_1

    private Span<T> _span;

    public ComponentRef(ref T component)
    {
        _span = MemoryMarshal.CreateSpan(ref component, 1);
    }

    public ref T Component
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(_span);
    }

    #else

    public ComponentRef(ref T component)
    {
        Component = ref component;
    }

    public ref T Component;

    #endif
}