using PewPew.Ecs.Core;

namespace PewPew.Ecs.Filters;

public interface IHeavyFilterableQueryWithoutId<T1, T2> : IQueryWithoutId<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    HeavyFilterDefinition<T1, T2> HeavyFilter { get; }
}