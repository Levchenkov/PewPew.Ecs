using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.Demo;

public abstract class SystemBaseWithoutId<T1, T2> : IQueryWithoutId<T1, T2>, IStateful, IUpdatableSystem
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    protected World World1;
    protected IndexedWorld World2;

    public void SetState(World world1, IndexedWorld world2)
    {
        World1 = world1;
        World2 = world2;
    }

    public abstract void Update();

    public abstract void Update(ref T1 component1, ref T2 component2);
}

public abstract class SystemBase<T1, T2> : IQuery<T1, T2>, IStateful, IUpdatableSystem
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    protected World World1;
    protected IndexedWorld World2;

    public void SetState(World world1, IndexedWorld world2)
    {
        World1 = world1;
        World2 = world2;
    }

    public abstract void Update();

    public abstract void Update(EntityId entityId, ref T1 component1, ref T2 component2);
}