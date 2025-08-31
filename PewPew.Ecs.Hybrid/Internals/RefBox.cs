namespace PewPew.Ecs.Hybrid.Internals;

internal class RefBox<T>
    where T : struct
{
    public T Ref;
}