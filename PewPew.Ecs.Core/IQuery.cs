namespace PewPew.Ecs.Core;
public interface IQuery<T1>
    where T1 : struct, IComponent
{
    void Update(EntityId entityId, ref T1 component1);
}

public interface IQuery<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    void Update(EntityId entityId, ref T1 component1, ref T2 component2);
}

public interface IQuery<T1, T2, T3>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    void Update(EntityId entityId, ref T1 component1, ref T2 component2, ref T3 component3);
}

public interface IQueryWithoutId<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    void Update(ref T1 component1, ref T2 component2);
}

public interface IQueryWithoutId<T1, T2, T3>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    void Update(ref T1 component1, ref T2 component2, ref T3 component3);
}

public interface IQueryWithTag<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, ITagComponent
{
    void Update(EntityId entityId, ref T1 component1);
}