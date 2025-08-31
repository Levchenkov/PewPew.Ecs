using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

using BitMask =
#if BIT_MASK_64_DEFAULT
    PewPew.Ecs.Filters.Masks.BitMask64;
#elif BIT_MASK_128_DEFAULT
    PewPew.Ecs.Filters.Masks.BitMask128;
#elif BIT_MASK_256_DEFAULT
    PewPew.Ecs.Filters.Masks.BitMask256;
#endif

namespace PewPew.Ecs.Filters;

public static class HeavyFilterQueryExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<TQ, T1, T2>(this HeavyFilter<BitMask, T1, T2> filter, TQ query)
        where TQ : struct, IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.ExecuteQuery<BitMask, TQ, T1, T2>(query);
    }

    public static void ExecuteQuery<TMask, TQ, T1, T2>(this HeavyFilter<TMask, T1, T2> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : struct, IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var components1 = filter.Instance.World.GetSparseSet<T1>().InternalData;
        var components2 = filter.Instance.World.GetSparseSet<T2>().InternalData;
        var entities = filter.Entities;

        var indexes = filter.Instance.ComponentIndexes;

        ref var firstComponent1 = ref components1[0];
        ref var firstComponent2 = ref components2[0];

        var count = entities.Length;
        ref var entityId = ref entities[0];
        ref var first = ref indexes[0];

        for (int i = 0; i < count; i++)
        {
            var index1 = first;
            var index2 = Unsafe.Add(ref first, 1);

            ref var component1 = ref Unsafe.Add(ref firstComponent1, index1);
            ref var component2 = ref Unsafe.Add(ref firstComponent2, index2);

            query.Update(entityId, ref component1, ref component2);

            entityId = ref Unsafe.Add(ref entityId, 1);
            first = ref Unsafe.Add(ref first, 2);
        }
    }

    public static void ExecuteQueryUnrolled<TMask, TQ, T1, T2>(this HeavyFilter<TMask, T1, T2> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : struct, IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var setRef1 = filter.Instance.World.GetComponents<T1>();
        var setRef2 = filter.Instance.World.GetComponents<T2>();
        var entities = filter.Entities;

        var indexes = filter.Instance.ComponentIndexes;

        var components1 = setRef1.InternalData;
        var components2 = setRef2.InternalData;

        ref var firstComponent1 = ref components1[0];
        ref var firstComponent2 = ref components2[0];

        var count = entities.Length;

        ref var entityId = ref entities[0];
        ref var first = ref indexes[0];
        int i = 0;

        for (; i <= count - 4; i += 4)
        {
            var entityId1 = entityId;
            var index1 = first;
            var index2 = Unsafe.Add(ref first, 1);
            ref var component1 = ref Unsafe.Add(ref firstComponent1, index1);
            ref var component2 = ref Unsafe.Add(ref firstComponent2, index2);
            query.Update(entityId1, ref component1, ref component2);

            var entityId2 = Unsafe.Add(ref entityId, 1);
            var index3 = Unsafe.Add(ref first, 2);
            var index4 = Unsafe.Add(ref first, 3);
            ref var component3 = ref Unsafe.Add(ref firstComponent1, index3);
            ref var component4 = ref Unsafe.Add(ref firstComponent2, index4);
            query.Update(entityId2, ref component3, ref component4);

            var entityId3 = Unsafe.Add(ref entityId, 2);
            var index5 = Unsafe.Add(ref first, 4);
            var index6 = Unsafe.Add(ref first, 5);
            ref var component5 = ref Unsafe.Add(ref firstComponent1, index5);
            ref var component6 = ref Unsafe.Add(ref firstComponent2, index6);
            query.Update(entityId3, ref component5, ref component6);

            var entityId4 = Unsafe.Add(ref entityId, 3);
            var index7 = Unsafe.Add(ref first, 6);
            var index8 = Unsafe.Add(ref first, 7);
            ref var component7 = ref Unsafe.Add(ref firstComponent1, index7);
            ref var component8 = ref Unsafe.Add(ref firstComponent2, index8);
            query.Update(entityId4, ref component7, ref component8);

            entityId = ref Unsafe.Add(ref entityId, 4);
            first = ref Unsafe.Add(ref first, 8);
        }

        for (; i < count; i++)
        {
            var index1 = first;
            var index2 = Unsafe.Add(ref first, 1);

            ref var component1 = ref components1[index1];
            ref var component2 = ref components2[index2];

            query.Update(entityId, ref component1, ref component2);

            entityId = ref Unsafe.Add(ref entityId, 1);
            first = ref Unsafe.Add(ref first, 2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutId<TQ, T1, T2>(this HeavyFilter<BitMask, T1, T2> filter, TQ query)
        where TQ : struct, IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.ExecuteQueryWithoutId<BitMask, TQ, T1, T2>(query);
    }

    public static void ExecuteQueryWithoutId<TMask, TQ, T1, T2>(this HeavyFilter<TMask, T1, T2> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : struct, IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var components1 = filter.Instance.World.GetSparseSet<T1>().InternalData;
        var components2 = filter.Instance.World.GetSparseSet<T2>().InternalData;
        var entities = filter.Entities;

        var indexes = filter.Instance.ComponentIndexes;

        ref var firstComponent1 = ref components1[0];
        ref var firstComponent2 = ref components2[0];

        var count = entities.Length;
        ref var first = ref indexes[0];

        for (int i = 0; i < count; i++)
        {
            var index1 = first;
            var index2 = Unsafe.Add(ref first, 1);

            ref var component1 = ref Unsafe.Add(ref firstComponent1, index1);
            ref var component2 = ref Unsafe.Add(ref firstComponent2, index2);

            query.Update(ref component1, ref component2);

            first = ref Unsafe.Add(ref first, 2);
        }
    }

    public static void ExecuteQueryWithoutIdUnrolled<TMask, TQ, T1, T2>(this HeavyFilter<TMask, T1, T2> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : struct, IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var setRef1 = filter.Instance.World.GetComponents<T1>();
        var setRef2 = filter.Instance.World.GetComponents<T2>();
        var entities = filter.Entities;

        var indexes = filter.Instance.ComponentIndexes;

        var components1 = setRef1.InternalData;
        var components2 = setRef2.InternalData;

        ref var firstComponent1 = ref components1[0];
        ref var firstComponent2 = ref components2[0];

        var count = entities.Length;

        ref var first = ref indexes[0];
        int i = 0;

        for (; i <= count - 4; i += 4)
        {
            var index1 = first;
            var index2 = Unsafe.Add(ref first, 1);
            ref var component1 = ref Unsafe.Add(ref firstComponent1, index1);
            ref var component2 = ref Unsafe.Add(ref firstComponent2, index2);
            query.Update(ref component1, ref component2);

            var index3 = Unsafe.Add(ref first, 2);
            var index4 = Unsafe.Add(ref first, 3);
            ref var component3 = ref Unsafe.Add(ref firstComponent1, index3);
            ref var component4 = ref Unsafe.Add(ref firstComponent2, index4);
            query.Update(ref component3, ref component4);

            var index5 = Unsafe.Add(ref first, 4);
            var index6 = Unsafe.Add(ref first, 5);
            ref var component5 = ref Unsafe.Add(ref firstComponent1, index5);
            ref var component6 = ref Unsafe.Add(ref firstComponent2, index6);
            query.Update(ref component5, ref component6);

            var index7 = Unsafe.Add(ref first, 6);
            var index8 = Unsafe.Add(ref first, 7);
            ref var component7 = ref Unsafe.Add(ref firstComponent1, index7);
            ref var component8 = ref Unsafe.Add(ref firstComponent2, index8);
            query.Update(ref component7, ref component8);

            first = ref Unsafe.Add(ref first, 8);
        }

        for (; i < count; i++)
        {
            var index1 = first;
            var index2 = Unsafe.Add(ref first, 1);

            ref var component1 = ref components1[index1];
            ref var component2 = ref components2[index2];

            query.Update(ref component1, ref component2);

            first = ref Unsafe.Add(ref first, 2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<T1, T2>(this HeavyFilter<BitMask, T1, T2> filter, QueryAction<T1, T2> query)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var components1 = filter.Instance.World.GetSparseSet<T1>().InternalData;
        var components2 = filter.Instance.World.GetSparseSet<T2>().InternalData;
        var entities = filter.Entities;

        var indexes = filter.Instance.ComponentIndexes;

        ref var firstComponent1 = ref components1[0];
        ref var firstComponent2 = ref components2[0];

        var count = entities.Length;
        ref var entityId = ref entities[0];
        ref var first = ref indexes[0];

        for (int i = 0; i < count; i++)
        {
            var index1 = first;
            var index2 = Unsafe.Add(ref first, 1);

            ref var component1 = ref Unsafe.Add(ref firstComponent1, index1);
            ref var component2 = ref Unsafe.Add(ref firstComponent2, index2);

            query(entityId, ref component1, ref component2);

            entityId = ref Unsafe.Add(ref entityId, 1);
            first = ref Unsafe.Add(ref first, 2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutId<T1, T2>(this HeavyFilter<BitMask, T1, T2> filter, QueryActionWithoutId<T1, T2> query)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var components1 = filter.Instance.World.GetSparseSet<T1>().InternalData;
        var components2 = filter.Instance.World.GetSparseSet<T2>().InternalData;
        var entities = filter.Entities;

        var indexes = filter.Instance.ComponentIndexes;

        ref var firstComponent1 = ref components1[0];
        ref var firstComponent2 = ref components2[0];

        var count = entities.Length;
        ref var first = ref indexes[0];

        for (int i = 0; i < count; i++)
        {
            var index1 = first;
            var index2 = Unsafe.Add(ref first, 1);

            ref var component1 = ref Unsafe.Add(ref firstComponent1, index1);
            ref var component2 = ref Unsafe.Add(ref firstComponent2, index2);

            query(ref component1, ref component2);

            first = ref Unsafe.Add(ref first, 2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithTag<TQ, T1, T2>(this HeavyFilter<BitMask, T1, T2> filter, TQ query)
        where TQ : struct, IQueryWithTag<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, ITagComponent
    {
        var components1 = filter.Instance.World.GetSparseSet<T1>().InternalData;
        var entities = filter.Entities;

        var indexes = filter.Instance.ComponentIndexes;

        ref var firstComponent1 = ref components1[0];

        var count = entities.Length;
        ref var entityId = ref entities[0];
        ref var first = ref indexes[0];

        for (int i = 0; i < count; i++)
        {
            var index1 = first;

            ref var component1 = ref Unsafe.Add(ref firstComponent1, index1);

            query.Update(entityId, ref component1);

            entityId = ref Unsafe.Add(ref entityId, 1);
            first = ref Unsafe.Add(ref first, 2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithTag<T1, T2>(this HeavyFilter<BitMask, T1, T2> filter, QueryActionWithTag<T1, T2> query)
        where T1 : struct, IComponent
        where T2 : struct, ITagComponent
    {
        var components1 = filter.Instance.World.GetSparseSet<T1>().InternalData;
        var entities = filter.Entities;

        var indexes = filter.Instance.ComponentIndexes;

        ref var firstComponent1 = ref components1[0];

        var count = entities.Length;
        ref var entityId = ref entities[0];
        ref var first = ref indexes[0];

        for (int i = 0; i < count; i++)
        {
            var index1 = first;

            ref var component1 = ref Unsafe.Add(ref firstComponent1, index1);

            query(entityId, ref component1);

            entityId = ref Unsafe.Add(ref entityId, 1);
            first = ref Unsafe.Add(ref first, 2);
        }
    }
}