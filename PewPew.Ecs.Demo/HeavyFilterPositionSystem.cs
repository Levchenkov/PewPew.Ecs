using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Demo;

public sealed class HeavyFilterPositionSystem : IStateful, IUpdatableSystem
{
    private FilterDefinition _filterDefinition = new FilterDefinition().With<Position>().With<Speed>();
    private FilterDefinition<Position,Speed> _filterDefinition5 = new();
    private HeavyFilterDefinition<Position,Speed> _heavyFilterDefinition = new();

    private Query _query;

    protected World World1;
    protected IndexedWorld World2;

    public void SetState(World world1, IndexedWorld world2)
    {
        World1 = world1;
        World2 = world2;
    }

    public void Update()
    {
        var filter = World2.GetFilter(_filterDefinition);
        filter.ExecuteQueryWithoutId<Query, Position, Speed>(_query);

        var heavyFilter = World2.FetchHeavyFilter(_heavyFilterDefinition);
        heavyFilter.ExecuteQueryWithoutId(_query);
    }

    private struct Query : IQueryWithoutId<Position, Speed>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(ref Position component1, ref Speed component2)
        {
            component1.Vector += component2.Vector;
        }
    }
}