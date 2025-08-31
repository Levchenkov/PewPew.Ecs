using PewPew.Ecs.Core;

namespace PewPew.Ecs.Filters;

public interface IFilterableQueryWithoutId<T1, T2> : IQueryWithoutId<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    FilterDefinition<T1, T2> Filter { get; }
}