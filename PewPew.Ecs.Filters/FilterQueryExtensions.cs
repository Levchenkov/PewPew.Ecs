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

public static class FilterQueryExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<TQ, T1, T2>(this Filter<BitMask> filter, TQ query)
        where TQ : IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        ExecuteQuery<BitMask, TQ, T1, T2>(filter, query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<T1, T2>(this Filter<BitMask> filter, QueryAction<T1, T2> query)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        ExecuteQuery<BitMask, T1, T2>(filter, query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<TMask, T1, T2>(this Filter<TMask> filter, QueryAction<T1, T2> query)
        where TMask : struct, IBitMask<TMask>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        var setRef1 = filter.World.GetSparseSet<T1>();
        var setRef2 = filter.World.GetSparseSet<T2>();
        var entities = filter.Entities;

        var indexes1 = setRef1.InternalIndexes;
        var components1 = setRef1.InternalData;

        var indexes2 = setRef2.InternalIndexes;
        var components2 = setRef2.InternalData;

        var count = entities.Length;
        for (int i = 0; i < count; i++)
        {
            var entityId = entities[i];
            var entityIndex = entityId.Index;
            var index1 = indexes1[entityIndex];
            ref var c1 = ref components1[index1];
            var index2 = indexes2[entityIndex];
            ref var c2 = ref components2[index2];

            query(entityId, ref c1, ref c2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQuery<TMask, TQ, T1, T2>(this Filter<TMask> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        var setRef1 = filter.World.GetSparseSet<T1>();
        var setRef2 = filter.World.GetSparseSet<T2>();
        var entities = filter.Entities;

        var indexes1 = setRef1.InternalIndexes;
        var components1 = setRef1.InternalData;

        var indexes2 = setRef2.InternalIndexes;
        var components2 = setRef2.InternalData;

        var count = entities.Length;
        for (int i = 0; i < count; i++)
        {
            var entityId = entities[i];
            var entityIndex = entityId.Index;
            var index1 = indexes1[entityIndex];
            ref var c1 = ref components1[index1];
            var index2 = indexes2[entityIndex];
            ref var c2 = ref components2[index2];

            query.Update(entityId, ref c1, ref c2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryUnrolled<TMask, TQ, T1, T2>(this Filter<TMask> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : IQuery<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        var setRef1 = filter.World.GetSparseSet<T1>();
        var setRef2 = filter.World.GetSparseSet<T2>();
        var entities = filter.Entities;

        var indexes1 = setRef1.InternalIndexes;
        var components1 = setRef1.InternalData;

        var indexes2 = setRef2.InternalIndexes;
        var components2 = setRef2.InternalData;

        var count = entities.Length;
        int i = 0;
        int remainder = count % 4;
        int unrollLimit = count - remainder;

// Unrolled loop
        for (; i < unrollLimit; i += 4)
        {
            var entityId0 = entities[i];
            var entityIndex0 = entityId0.Index;
            var index10 = indexes1[entityIndex0];
            ref var c10 = ref components1[index10];
            var index20 = indexes2[entityIndex0];
            ref var c20 = ref components2[index20];
            query.Update(entityId0, ref c10, ref c20);

            var entityId1 = entities[i + 1];
            var entityIndex1 = entityId1.Index;
            var index11 = indexes1[entityIndex1];
            ref var c11 = ref components1[index11];
            var index21 = indexes2[entityIndex1];
            ref var c21 = ref components2[index21];
            query.Update(entityId1, ref c11, ref c21);

            var entityId2 = entities[i + 2];
            var entityIndex2 = entityId2.Index;
            var index12 = indexes1[entityIndex2];
            ref var c12 = ref components1[index12];
            var index22 = indexes2[entityIndex2];
            ref var c22 = ref components2[index22];
            query.Update(entityId2, ref c12, ref c22);

            var entityId3 = entities[i + 3];
            var entityIndex3 = entityId3.Index;
            var index13 = indexes1[entityIndex3];
            ref var c13 = ref components1[index13];
            var index23 = indexes2[entityIndex3];
            ref var c23 = ref components2[index23];
            query.Update(entityId3, ref c13, ref c23);
        }

        for (; i < count; i++)
        {
            var entityId = entities[i];
            var entityIndex = entityId.Index;
            var index1 = indexes1[entityIndex];
            ref var c1 = ref components1[index1];
            var index2 = indexes2[entityIndex];
            ref var c2 = ref components2[index2];
            query.Update(entityId, ref c1, ref c2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutId<T1, T2>(this Filter<BitMask> filter, QueryActionWithoutId<T1, T2> query)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        var setRef1 = filter.World.GetSparseSet<T1>();
        var setRef2 = filter.World.GetSparseSet<T2>();
        var entities = filter.Entities;

        var indexes1 = setRef1.InternalIndexes;
        var components1 = setRef1.InternalData;

        var indexes2 = setRef2.InternalIndexes;
        var components2 = setRef2.InternalData;

        var count = entities.Length;
        for (int i = 0; i < count; i++)
        {
            var entityIndex = entities[i].Index;
            var index1 = indexes1[entityIndex];
            ref var c1 = ref components1[index1];
            var index2 = indexes2[entityIndex];
            ref var c2 = ref components2[index2];

            query(ref c1, ref c2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutId<TQ, T1, T2>(this Filter<BitMask> filter, TQ query)
        where TQ : IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        ExecuteQueryWithoutId<BitMask, TQ, T1, T2>(filter, query);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutId<TMask, TQ, T1, T2>(this Filter<TMask> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        var setRef1 = filter.World.GetSparseSet<T1>();
        var setRef2 = filter.World.GetSparseSet<T2>();
        var entities = filter.Entities;

        var indexes1 = setRef1.InternalIndexes;
        var components1 = setRef1.InternalData;

        var indexes2 = setRef2.InternalIndexes;
        var components2 = setRef2.InternalData;

        var count = entities.Length;
        for (int i = 0; i < count; i++)
        {
            var entityIndex = entities[i].Index;
            var index1 = indexes1[entityIndex];
            ref var c1 = ref components1[index1];
            var index2 = indexes2[entityIndex];
            ref var c2 = ref components2[index2];

            query.Update(ref c1, ref c2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteQueryWithoutIdUnrolled<TMask, TQ, T1, T2>(this Filter<TMask> filter, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        var setRef1 = filter.World.GetSparseSet<T1>();
        var setRef2 = filter.World.GetSparseSet<T2>();
        var entities = filter.Entities;

        var indexes1 = setRef1.InternalIndexes;
        var components1 = setRef1.InternalData;

        var indexes2 = setRef2.InternalIndexes;
        var components2 = setRef2.InternalData;

        var count = entities.Length;
        int i = 0;
        int remainder = count % 4;
        int unrollLimit = count - remainder;

        for (; i < unrollLimit; i += 4)
        {
            var entityId0 = entities[i].Index;
            var index10 = indexes1[entityId0];
            ref var c10 = ref components1[index10];
            var index20 = indexes2[entityId0];
            ref var c20 = ref components2[index20];
            query.Update(ref c10, ref c20);

            var entityId1 = entities[i + 1].Index;
            var index11 = indexes1[entityId1];
            ref var c11 = ref components1[index11];
            var index21 = indexes2[entityId1];
            ref var c21 = ref components2[index21];
            query.Update(ref c11, ref c21);

            var entityId2 = entities[i + 2].Index;
            var index12 = indexes1[entityId2];
            ref var c12 = ref components1[index12];
            var index22 = indexes2[entityId2];
            ref var c22 = ref components2[index22];
            query.Update(ref c12, ref c22);

            var entityId3 = entities[i + 3].Index;
            var index13 = indexes1[entityId3];
            ref var c13 = ref components1[index13];
            var index23 = indexes2[entityId3];
            ref var c23 = ref components2[index23];
            query.Update(ref c13, ref c23);
        }

        for (; i < count; i++)
        {
            var entityId = entities[i].Index;
            var index1 = indexes1[entityId];
            ref var c1 = ref components1[index1];
            var index2 = indexes2[entityId];
            ref var c2 = ref components2[index2];
            query.Update(ref c1, ref c2);
        }
    }

    public static void ExecuteQueryWithTag<TQ, T1, T2>(this Filter<BitMask> filter, TQ query)
        where TQ : IQueryWithTag<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, ITagComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        var setRef1 = filter.World.GetSparseSet<T1>();
        var entities = filter.Entities;

        var indexes1 = setRef1.InternalIndexes;
        var components1 = setRef1.InternalData;

        var count = entities.Length;
        for (int i = 0; i < count; i++)
        {
            var entityId = entities[i];
            var entityIndex = entityId.Index;
            var index1 = indexes1[entityIndex];
            ref var c1 = ref components1[index1];

            query.Update(entityId, ref c1);
        }
    }

    public static void ExecuteQueryWithTag<T1, T2>(this Filter<BitMask> filter, QueryActionWithTag<T1, T2> queryAction)
        where T1 : struct, IComponent
        where T2 : struct, ITagComponent
    {
        filter.DebugValidateFilterComponent<T1>();
        filter.DebugValidateFilterComponent<T2>();

        var setRef1 = filter.World.GetSparseSet<T1>();
        var entities = filter.Entities;

        var indexes1 = setRef1.InternalIndexes;
        var components1 = setRef1.InternalData;

        var count = entities.Length;
        for (int i = 0; i < count; i++)
        {
            var entityId = entities[i];
            var entityIndex = entityId.Index;
            var index1 = indexes1[entityIndex];
            ref var c1 = ref components1[index1];

            queryAction(entityId, ref c1);
        }
    }
}