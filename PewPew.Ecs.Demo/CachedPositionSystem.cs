using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Demo;

public sealed class CachedPositionSystem : SystemBaseWithoutId<Position, Speed>
{
    private FilterDefinition _filterDefinition = new FilterDefinition().With<Position>().With<Speed>();

    public override void Update()
    {
        var filter = World2.GetFilter(_filterDefinition);
        filter.ExecuteQueryWithoutId<CachedPositionSystem, Position, Speed>(this);
    }

    public override void Update(ref Position position, ref Speed speed)
    {
        position.Vector += speed.Vector;
    }
}