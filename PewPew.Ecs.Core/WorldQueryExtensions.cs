using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core;

public static class WorldQueryExtensions
{
    public static void ExecuteQuery<T1>(this World world, QueryAction<T1> queryAction)
        where T1 : struct, IComponent
    {
        var sparseSet1 = world.GetSparseSet<T1>();
        var entities = sparseSet1.Entities;
        var components = sparseSet1.Components;

        for (int i = 0; i < sparseSet1.Count; i++)
        {
            var entityId = entities[i];
            ref var component = ref components[i];

            queryAction(entityId, ref component);
        }
    }

    public static void ExecuteQuery<T1, T2>(this World world, QueryAction<T1, T2> queryAction)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var sparseSet2 = world.GetSparseSet<T2>();
        var entities = sparseSet2.Entities;
        var components = sparseSet2.Components;

        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet2.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            queryAction(entityId, ref componentRef1.Component, ref components[i]);
        }
    }

    public static void ExecuteQuery<T1, T2, T3>(this World world, QueryAction<T1, T2, T3> queryAction)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var sparseSet3 = world.GetSparseSet<T3>();
        var entities = sparseSet3.Entities;
        var components = sparseSet3.Components;

        var sparseSet2 = world.GetSparseSet<T2>();
        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet3.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet2.TryGetComponent(entityId, out var componentRef2))
            {
                continue;
            }

            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            queryAction(entityId, ref componentRef1.Component, ref componentRef2.Component, ref components[i]);
        }
    }

    public static void ExecuteQueryWithoutId<T1>(this World world, QueryActionWithoutId<T1> queryAction)
        where T1 : struct, IComponent
    {
        var sparseSet1 = world.GetSparseSet<T1>();
        var components = sparseSet1.Components;
        for (int i = 0; i < sparseSet1.Count; i++)
        {
            queryAction(ref components[i]);
        }
    }

    public static void ExecuteQueryWithoutId<T1, T2>(this World world, QueryActionWithoutId<T1, T2> queryAction)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var sparseSet2 = world.GetSparseSet<T2>();
        var entities = sparseSet2.Entities;
        var components = sparseSet2.Components;

        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet2.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            queryAction(ref componentRef1.Component, ref components[i]);
        }
    }

    public static void ExecuteQueryWithoutId<T1, T2, T3>(this World world, QueryActionWithoutId<T1, T2, T3> queryAction)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var sparseSet3 = world.GetSparseSet<T3>();
        var entities = sparseSet3.Entities;
        var components = sparseSet3.Components;

        var sparseSet2 = world.GetSparseSet<T2>();
        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet3.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet2.TryGetComponent(entityId, out var componentRef2))
            {
                continue;
            }

            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            queryAction(ref componentRef1.Component, ref componentRef2.Component, ref components[i]);
        }
    }

    public static void ExecuteQuery<TQ, T1>(this World world, TQ query)
        where TQ : IQuery<T1>
        where T1 : struct, IComponent
    {
        var sparseSet1 = world.GetSparseSet<T1>();
        var entities = sparseSet1.Entities;
        var components = sparseSet1.Components;

        for (int i = 0; i < sparseSet1.Count; i++)
        {
            var entityId = entities[i];
            ref var component = ref components[i];

            query.Update(entityId, ref component);
        }
    }

    public static void ExecuteQuery<TQ, T1, T2>(this World world, TQ query)
        where TQ : IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var sparseSet2 = world.GetSparseSet<T2>();
        var entities = sparseSet2.Entities;
        var components = sparseSet2.Components;

        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet2.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            query.Update(entityId, ref componentRef1.Component, ref components[i]);
        }
    }

    public static void ExecuteQuery<TQ, T1, T2, T3>(this World world, TQ query)
        where TQ : IQuery<T1, T2, T3>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var sparseSet3 = world.GetSparseSet<T3>();
        var entities = sparseSet3.Entities;
        var components = sparseSet3.Components;

        var sparseSet2 = world.GetSparseSet<T2>();
        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet3.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet2.TryGetComponent(entityId, out var componentRef2))
            {
                continue;
            }

            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            query.Update(entityId, ref componentRef1.Component, ref componentRef2.Component, ref components[i]);
        }
    }

    public static void ExecuteQueryWithoutId<TQ, T1>(this World world, TQ query)
        where TQ : IQueryWithoutId<T1>
        where T1 : struct, IComponent
    {
        var sparseSet1 = world.GetSparseSet<T1>();
        var components = sparseSet1.Components;
        for (int i = 0; i < sparseSet1.Count; i++)
        {
            query.Update(ref components[i]);
        }
    }

    public static void ExecuteQueryWithoutId<TQ, T1, T2>(this World world, TQ query)
        where TQ : IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var sparseSet2 = world.GetSparseSet<T2>();
        var entities = sparseSet2.Entities;
        var components = sparseSet2.Components;

        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet2.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            query.Update(ref componentRef1.Component, ref components[i]);
        }
    }

    public static void ExecuteQueryWithoutId<TQ, T1, T2, T3>(this World world, TQ query)
        where TQ : IQueryWithoutId<T1, T2, T3>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var sparseSet3 = world.GetSparseSet<T3>();
        var entities = sparseSet3.Entities;
        var components = sparseSet3.Components;

        var sparseSet2 = world.GetSparseSet<T2>();
        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet3.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet2.TryGetComponent(entityId, out var componentRef2))
            {
                continue;
            }

            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            query.Update(ref componentRef1.Component, ref componentRef2.Component, ref components[i]);
        }
    }

    public static void ExecuteQueryWithTag<T1, T2>(this World world, QueryActionWithTag<T1, T2> queryAction)
        where T1 : struct, IComponent
        where T2 : struct, ITagComponent
    {
        var sparseSet2 = world.GetTagSparseSet<T2>();
        var entities = sparseSet2.Entities;

        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet2.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            queryAction(entityId, ref componentRef1.Component);
        }
    }

    public static void ExecuteQueryWithTag<TQ, T1, T2>(this World world, TQ query)
        where TQ : IQueryWithTag<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, ITagComponent
    {
        var sparseSet2 = world.GetTagSparseSet<T2>();
        var entities = sparseSet2.Entities;

        var sparseSet1 = world.GetSparseSet<T1>();
        for (int i = 0; i < sparseSet2.Count; i++)
        {
            var entityId = entities[i];
            if (!sparseSet1.TryGetComponent(entityId, out var componentRef1))
            {
                continue;
            }

            query.Update(entityId, ref componentRef1.Component);
        }
    }
}