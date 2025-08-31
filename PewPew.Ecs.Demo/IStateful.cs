using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.Demo;

public interface IStateful
{
    void SetState(World world1, IndexedWorld world2);
}