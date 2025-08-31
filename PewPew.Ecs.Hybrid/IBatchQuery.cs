using PewPew.Ecs.Core;

namespace PewPew.Ecs.Hybrid;

public interface IBatchQuery<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    void BatchUpdate(Span<T1> component1, Span<T2> component2);

    void SparseUpdate(ref T1 component1, ref T2 component2);
}