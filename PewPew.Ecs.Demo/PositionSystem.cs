using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.Demo;

public sealed class PositionSystem : IStateful, IUpdatableSystem, IQueryWithoutId<Position, Speed>
{
    private World _world;

    public void SetState(World world1, IndexedWorld world2)
    {
        _world = world1;
    }

    public void Update()
    {
        _world.ExecuteQueryWithoutId<PositionSystem, Position, Speed>(this);
    }

    public void Update(ref Position position, ref Speed speed)
    {
        position.Vector += speed.Vector;
    }
}