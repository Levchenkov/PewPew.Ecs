namespace PewPew.Ecs.Core;

public delegate void QueryAction<T1>(EntityId entityId, ref T1 c1)
    where T1 : struct, IComponent;

public delegate void QueryAction<T1, T2>(EntityId entityId, ref T1 c1, ref T2 c2)
    where T1 : struct, IComponent
    where T2 : struct, IComponent;

public delegate void QueryAction<T1, T2, T3>(EntityId entityId, ref T1 c1, ref T2 c2, ref T3 c3)
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent;

public delegate void QueryActionWithoutId<T1>(ref T1 c1)
    where T1 : struct, IComponent;

public delegate void QueryActionWithoutId<T1, T2>(ref T1 c1, ref T2 c2)
    where T1 : struct, IComponent
    where T2 : struct, IComponent;

public delegate void QueryActionWithoutId<T1, T2, T3>(ref T1 c1, ref T2 c2, ref T3 c3)
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent;

public delegate void QueryActionWithTag<T1, T2>(EntityId entityId, ref T1 c1)
    where T1 : struct, IComponent
    where T2 : struct, ITagComponent;