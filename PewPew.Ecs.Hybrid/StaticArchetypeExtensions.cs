using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid;

public static class StaticArchetypeExtensions
{
    public static void ExecuteQueryWithoutId<TMask, TQ, T1, T2>(this StaticArchetype<TMask, T1, T2> archetype, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : struct, IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        ref var components1 = ref archetype.Components1[0];
        ref var components2 = ref archetype.Components2[0];

        for (int i = 0; i < archetype.Entities.Length; i++)
        {
            query.Update(ref components1, ref components2);

            components1 = ref Unsafe.Add(ref components1, 1);
            components2 = ref Unsafe.Add(ref components2, 1);
        }
    }

    internal static void ExecuteQueryWithoutIdUnrolled<TMask, TQ, T1, T2>(this StaticArchetype<TMask, T1, T2> archetype, TQ query)
        where TMask : struct, IBitMask<TMask>
        where TQ : struct, IQueryWithoutId<T1, T2>
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        ref var components1 = ref archetype.Components1[0];
        ref var components2 = ref archetype.Components2[0];

        int i = 0;
        var count = archetype.Entities.Length;
        int unrollCount = (count / 4) * 4; // Ensures we only unroll up to the nearest multiple of 4

        for (; i < unrollCount; i += 4)
        {
            query.Update(ref components1, ref components2);
            components1 = ref Unsafe.Add(ref components1, 1);
            components2 = ref Unsafe.Add(ref components2, 1);

            query.Update(ref components1, ref components2);
            components1 = ref Unsafe.Add(ref components1, 1);
            components2 = ref Unsafe.Add(ref components2, 1);

            query.Update(ref components1, ref components2);
            components1 = ref Unsafe.Add(ref components1, 1);
            components2 = ref Unsafe.Add(ref components2, 1);

            query.Update(ref components1, ref components2);
            components1 = ref Unsafe.Add(ref components1, 1);
            components2 = ref Unsafe.Add(ref components2, 1);
        }

// Handle remaining iterations if `count` isn't a multiple of 4
        for (; i < count; i++)
        {
            query.Update(ref components1, ref components2);
            components1 = ref Unsafe.Add(ref components1, 1);
            components2 = ref Unsafe.Add(ref components2, 1);
        }
    }
}