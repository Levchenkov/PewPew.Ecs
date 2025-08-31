using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core.Internals;

internal sealed class SingletonStorage<T>
    where T : struct, ISingletonComponent
{
    private T _component;
    private bool _hasComponent;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent() => _hasComponent;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent()
    {
#if DEBUG
        if (!_hasComponent)
            ThrowHelper.ThrowComponentNotFoundException<T>();
#endif

        return ref _component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(out ComponentRef<T> componentRef)
    {
        if (!_hasComponent)
        {
            componentRef = default;

            return false;
        }

        componentRef = new ComponentRef<T>(ref _component);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(ref T component)
    {
        if (!_hasComponent)
            return false;

        component = ref _component;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddComponent()
    {
        _hasComponent = true;

        return ref _component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteComponent()
    {
        _hasComponent = false;
        _component = default;
    }
}