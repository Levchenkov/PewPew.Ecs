using PewPew.Ecs.Core;

namespace PewPew.Ecs.Hybrid;

public interface IBatchQuery<T1>
    where T1 : struct, IComponent
{
    void BatchUpdate(Span<T1> component1);

    void SparseUpdate(ref T1 component1);
}

public interface IBatchQuery<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    void BatchUpdate(Span<T1> component1, Span<T2> component2);

    void SparseUpdate(ref T1 component1, ref T2 component2);
}

public interface IBatchQuery<T1, T2, T3>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    void BatchUpdate(Span<T1> component1, Span<T2> component2, Span<T3> component3);

    void SparseUpdate(ref T1 component1, ref T2 component2, ref T3 component3);
}