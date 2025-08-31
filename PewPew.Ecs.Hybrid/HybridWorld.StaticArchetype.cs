using System.Runtime.InteropServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.Hybrid;

public partial class HybridWorld<TMask>
    where TMask : struct, IBitMask<TMask>
{
    private void InitStaticArchetype<T1, T2>(
        int maxComponentsPerArchetype, Type key, List<int> globalComponentIndexes)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        if (_isAnyFilterCreated)
        {
            throw new NotSupportedException("Init all static archetypes first before fetching any filter");
        }

        if (_typeToArchetypeMap.ContainsKey(key))
            return;

        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if (_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
        {
            var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2>(parts!);
            _typeToArchetypeMap[key] = permutatedArchetype;
        }
        else
        {
            var mask = GetMask(globalComponentIndexes);

            var staticArchetype = new StaticArchetypeInstance<TMask, T1, T2>(_typeToArchetypeMap.Count, mask, _entityIndexToArchetypeDenseIndexMap, maxComponentsPerArchetype, new RefBox<int>(), this, _world.ResizeStrategy);

            parts = _universalKeyToPartsMap[universalKey] = new Dictionary<Type, object>();

            parts[typeof(int)] = staticArchetype.Id;
            parts[typeof(TMask)] = mask;
            parts[typeof(RefBox<int>)] = staticArchetype.CountBox;
            parts[typeof(EntityId[])] = staticArchetype.Entities;
            parts[typeof(T1[])] = staticArchetype.Components1;
            parts[typeof(T2[])] = staticArchetype.Components2;

            _typeToArchetypeMap[key] = staticArchetype;
        }
    }

    private void InitStaticArchetype<T1, T2, T3>(
        int maxComponentsPerArchetype, Type key, List<int> globalComponentIndexes)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        if (_isAnyFilterCreated)
        {
            throw new NotSupportedException("Init all static archetypes first before fetching any filter");
        }

        if (_typeToArchetypeMap.ContainsKey(key))
            return;

        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if (_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
        {
            var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2, T3>(parts!);
            _typeToArchetypeMap[key] = permutatedArchetype;
        }
        else
        {
            var mask = GetMask(globalComponentIndexes);

            var staticArchetype = new StaticArchetypeInstance<TMask, T1, T2, T3>(_typeToArchetypeMap.Count, mask, _entityIndexToArchetypeDenseIndexMap, maxComponentsPerArchetype, new RefBox<int>(), this, _world.ResizeStrategy);

            parts = _universalKeyToPartsMap[universalKey] = new Dictionary<Type, object>();

            parts[typeof(int)] = staticArchetype.Id;
            parts[typeof(TMask)] = mask;
            parts[typeof(RefBox<int>)] = staticArchetype.CountBox;
            parts[typeof(EntityId[])] = staticArchetype.Entities;
            parts[typeof(T1[])] = staticArchetype.Components1;
            parts[typeof(T2[])] = staticArchetype.Components2;
            parts[typeof(T3[])] = staticArchetype.Components3;

            _typeToArchetypeMap[key] = staticArchetype;
        }
    }

    private void InitStaticArchetype<T1, T2, T3, T4>(
        int maxComponentsPerArchetype, Type key, List<int> globalComponentIndexes)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
    {
        if (_isAnyFilterCreated)
        {
            throw new NotSupportedException("Init all static archetypes first before fetching any filter");
        }

        if (_typeToArchetypeMap.ContainsKey(key))
            return;

        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if (_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
        {
            var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2, T3, T4>(parts!);
            _typeToArchetypeMap[key] = permutatedArchetype;
        }
        else
        {
            var mask = GetMask(globalComponentIndexes);

            var staticArchetype = new StaticArchetypeInstance<TMask, T1, T2, T3, T4>(_typeToArchetypeMap.Count, mask, _entityIndexToArchetypeDenseIndexMap, maxComponentsPerArchetype, new RefBox<int>(), this, _world.ResizeStrategy);

            parts = _universalKeyToPartsMap[universalKey] = new Dictionary<Type, object>();

            parts[typeof(int)] = staticArchetype.Id;
            parts[typeof(TMask)] = mask;
            parts[typeof(RefBox<int>)] = staticArchetype.CountBox;
            parts[typeof(EntityId[])] = staticArchetype.Entities;
            parts[typeof(T1[])] = staticArchetype.Components1;
            parts[typeof(T2[])] = staticArchetype.Components2;
            parts[typeof(T3[])] = staticArchetype.Components3;
            parts[typeof(T4[])] = staticArchetype.Components4;

            _typeToArchetypeMap[key] = staticArchetype;
        }
    }

    private void InitStaticArchetype<T1, T2, T3, T4, T5>(
        int maxComponentsPerArchetype, Type key, List<int> globalComponentIndexes)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
    {
        if (_isAnyFilterCreated)
        {
            throw new NotSupportedException("Init all static archetypes first before fetching any filter");
        }

        if (_typeToArchetypeMap.ContainsKey(key))
            return;

        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if (_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
        {
            var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2, T3, T4, T5>(parts!);
            _typeToArchetypeMap[key] = permutatedArchetype;
        }
        else
        {
            var mask = GetMask(globalComponentIndexes);

            var staticArchetype = new StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5>(_typeToArchetypeMap.Count, mask, _entityIndexToArchetypeDenseIndexMap, maxComponentsPerArchetype, new RefBox<int>(), this, _world.ResizeStrategy);

            parts = _universalKeyToPartsMap[universalKey] = new Dictionary<Type, object>();

            parts[typeof(int)] = staticArchetype.Id;
            parts[typeof(TMask)] = mask;
            parts[typeof(RefBox<int>)] = staticArchetype.CountBox;
            parts[typeof(EntityId[])] = staticArchetype.Entities;
            parts[typeof(T1[])] = staticArchetype.Components1;
            parts[typeof(T2[])] = staticArchetype.Components2;
            parts[typeof(T3[])] = staticArchetype.Components3;
            parts[typeof(T4[])] = staticArchetype.Components4;
            parts[typeof(T5[])] = staticArchetype.Components5;

            _typeToArchetypeMap[key] = staticArchetype;
        }
    }

    private void InitStaticArchetype<T1, T2, T3, T4, T5, T6>(
        int maxComponentsPerArchetype, Type key, List<int> globalComponentIndexes)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
    {
        if (_isAnyFilterCreated)
        {
            throw new NotSupportedException("Init all static archetypes first before fetching any filter");
        }

        if (_typeToArchetypeMap.ContainsKey(key))
            return;

        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if (_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
        {
            var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2, T3, T4, T5, T6>(parts!);
            _typeToArchetypeMap[key] = permutatedArchetype;
        }
        else
        {
            var mask = GetMask(globalComponentIndexes);

            var staticArchetype = new StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>(_typeToArchetypeMap.Count, mask, _entityIndexToArchetypeDenseIndexMap, maxComponentsPerArchetype, new RefBox<int>(), this, _world.ResizeStrategy);

            parts = _universalKeyToPartsMap[universalKey] = new Dictionary<Type, object>();

            parts[typeof(int)] = staticArchetype.Id;
            parts[typeof(TMask)] = mask;
            parts[typeof(RefBox<int>)] = staticArchetype.CountBox;
            parts[typeof(EntityId[])] = staticArchetype.Entities;
            parts[typeof(T1[])] = staticArchetype.Components1;
            parts[typeof(T2[])] = staticArchetype.Components2;
            parts[typeof(T3[])] = staticArchetype.Components3;
            parts[typeof(T4[])] = staticArchetype.Components4;
            parts[typeof(T5[])] = staticArchetype.Components5;
            parts[typeof(T6[])] = staticArchetype.Components6;

            _typeToArchetypeMap[key] = staticArchetype;
        }
    }

    private TMask GetMask(List<int> globalComponentIndexes)
    {
        TMask mask = default;

        foreach (var globalIndex in globalComponentIndexes)
        {
            mask.SetBit(GetLocalIndex(globalIndex));
        }

        return mask;
    }

    private static int ComputeUniversalKey(List<int> globalComponentIndexes)
    {
        globalComponentIndexes.Sort();

        var hashCode = new HashCode();
#if NETSTANDARD2_1
        for (var i = 0; i < globalComponentIndexes.Count; i++)
        {
            var index = globalComponentIndexes[i];
            hashCode.Add(index);
        }
#else
        var intSpan = CollectionsMarshal.AsSpan(globalComponentIndexes);
        var byteSpan = MemoryMarshal.Cast<int, byte>(intSpan);
        hashCode.AddBytes(byteSpan);
#endif

        return hashCode.ToHashCode();
    }

    private StaticArchetypeInstance<TMask, T1, T2> CreateStaticArchetypeFromParts<T1, T2>(Dictionary<Type, object> parts)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var entities = (EntityId[])parts[typeof(EntityId[])];
        var components1 = (T1[])parts[typeof(T1[])];
        var components2 = (T2[])parts[typeof(T2[])];
        var countBox = (RefBox<int>)parts[typeof(RefBox<int>)];
        var id = (int)parts[typeof(int)];
        var mask = (TMask)parts[typeof(TMask)];

        var archetype = new StaticArchetypeInstance<TMask, T1, T2>(
            id,
            mask,
            _entityIndexToArchetypeDenseIndexMap,
            entities,
            components1,
            components2,
            countBox,
            this,
            _world.ResizeStrategy);

        return archetype;
    }

    private StaticArchetypeInstance<TMask, T1, T2, T3> CreateStaticArchetypeFromParts<T1, T2, T3>(Dictionary<Type, object> parts)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var entities = (EntityId[])parts[typeof(EntityId[])];
        var components1 = (T1[])parts[typeof(T1[])];
        var components2 = (T2[])parts[typeof(T2[])];
        var components3 = (T3[])parts[typeof(T3[])];
        var countBox = (RefBox<int>)parts[typeof(RefBox<int>)];
        var id = (int)parts[typeof(int)];
        var mask = (TMask)parts[typeof(TMask)];

        var archetype = new StaticArchetypeInstance<TMask, T1, T2, T3>(
            id,
            mask,
            _entityIndexToArchetypeDenseIndexMap,
            entities,
            components1,
            components2,
            components3,
            countBox,
            this,
            _world.ResizeStrategy);

        return archetype;
    }

    private StaticArchetypeInstance<TMask, T1, T2, T3, T4> CreateStaticArchetypeFromParts<T1, T2, T3, T4>(Dictionary<Type, object> parts)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
    {
        var entities = (EntityId[])parts[typeof(EntityId[])];
        var components1 = (T1[])parts[typeof(T1[])];
        var components2 = (T2[])parts[typeof(T2[])];
        var components3 = (T3[])parts[typeof(T3[])];
        var components4 = (T4[])parts[typeof(T4[])];
        var countBox = (RefBox<int>)parts[typeof(RefBox<int>)];
        var id = (int)parts[typeof(int)];
        var mask = (TMask)parts[typeof(TMask)];

        var archetype = new StaticArchetypeInstance<TMask, T1, T2, T3, T4>(
            id,
            mask,
            _entityIndexToArchetypeDenseIndexMap,
            entities,
            components1,
            components2,
            components3,
            components4,
            countBox,
            this,
            _world.ResizeStrategy);

        return archetype;
    }

    private StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5> CreateStaticArchetypeFromParts<T1, T2, T3, T4, T5>(Dictionary<Type, object> parts)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
    {
        var entities = (EntityId[])parts[typeof(EntityId[])];
        var components1 = (T1[])parts[typeof(T1[])];
        var components2 = (T2[])parts[typeof(T2[])];
        var components3 = (T3[])parts[typeof(T3[])];
        var components4 = (T4[])parts[typeof(T4[])];
        var components5 = (T5[])parts[typeof(T5[])];
        var countBox = (RefBox<int>)parts[typeof(RefBox<int>)];
        var id = (int)parts[typeof(int)];
        var mask = (TMask)parts[typeof(TMask)];

        var archetype = new StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5>(
            id,
            mask,
            _entityIndexToArchetypeDenseIndexMap,
            entities,
            components1,
            components2,
            components3,
            components4,
            components5,
            countBox,
            this,
            _world.ResizeStrategy);

        return archetype;
    }

    private StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6> CreateStaticArchetypeFromParts<T1, T2, T3, T4, T5, T6>(Dictionary<Type, object> parts)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
    {
        var entities = (EntityId[])parts[typeof(EntityId[])];
        var components1 = (T1[])parts[typeof(T1[])];
        var components2 = (T2[])parts[typeof(T2[])];
        var components3 = (T3[])parts[typeof(T3[])];
        var components4 = (T4[])parts[typeof(T4[])];
        var components5 = (T5[])parts[typeof(T5[])];
        var components6 = (T6[])parts[typeof(T6[])];
        var countBox = (RefBox<int>)parts[typeof(RefBox<int>)];
        var id = (int)parts[typeof(int)];
        var mask = (TMask)parts[typeof(TMask)];

        var archetype = new StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>(
            id,
            mask,
            _entityIndexToArchetypeDenseIndexMap,
            entities,
            components1,
            components2,
            components3,
            components4,
            components5,
            components6,
            countBox,
            this,
            _world.ResizeStrategy);

        return archetype;
    }

    private StaticArchetype<TMask, T1, T2> GetStaticArchetype<T1, T2>(Type key, Func<List<int>> globalComponentIndexesFunc)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        if (_typeToArchetypeMap.TryGetValue(key, out var staticArchetype))
            return new StaticArchetype<TMask, T1, T2>((StaticArchetypeInstance<TMask, T1, T2>)staticArchetype);

        var globalComponentIndexes = globalComponentIndexesFunc();
        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if(!_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
            ThrowHelper.ThrowNotSupportedException($"Archetype {key.FullName} is not initialized!");

        var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2>(parts!);

        _typeToArchetypeMap[key] = permutatedArchetype;

        return new StaticArchetype<TMask, T1, T2>(permutatedArchetype);
    }

    private StaticArchetype<TMask, T1, T2, T3> GetStaticArchetype<T1, T2, T3>(Type key, Func<List<int>> globalComponentIndexesFunc)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        if (_typeToArchetypeMap.TryGetValue(key, out var staticArchetype))
            return new StaticArchetype<TMask, T1, T2, T3>((StaticArchetypeInstance<TMask, T1, T2, T3>)staticArchetype);

        var globalComponentIndexes = globalComponentIndexesFunc();
        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if(!_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
            ThrowHelper.ThrowNotSupportedException($"Archetype {key.FullName} is not initialized!");

        var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2, T3>(parts!);

        _typeToArchetypeMap[key] = permutatedArchetype;

        return new StaticArchetype<TMask, T1, T2, T3>(permutatedArchetype);
    }

    private StaticArchetype<TMask, T1, T2, T3, T4> GetStaticArchetype<T1, T2, T3, T4>(Type key, Func<List<int>> globalComponentIndexesFunc)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
    {
        if (_typeToArchetypeMap.TryGetValue(key, out var staticArchetype))
            return new StaticArchetype<TMask, T1, T2, T3, T4>((StaticArchetypeInstance<TMask, T1, T2, T3, T4>)staticArchetype);

        var globalComponentIndexes = globalComponentIndexesFunc();
        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if(!_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
            ThrowHelper.ThrowNotSupportedException($"Archetype {key.FullName} is not initialized!");

        var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2, T3, T4>(parts!);

        _typeToArchetypeMap[key] = permutatedArchetype;

        return new StaticArchetype<TMask, T1, T2, T3, T4>(permutatedArchetype);
    }

    private StaticArchetype<TMask, T1, T2, T3, T4, T5> GetStaticArchetype<T1, T2, T3, T4, T5>(Type key, Func<List<int>> globalComponentIndexesFunc)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
    {
        if (_typeToArchetypeMap.TryGetValue(key, out var staticArchetype))
            return new StaticArchetype<TMask, T1, T2, T3, T4, T5>((StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5>)staticArchetype);

        var globalComponentIndexes = globalComponentIndexesFunc();
        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if(!_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
            ThrowHelper.ThrowNotSupportedException($"Archetype {key.FullName} is not initialized!");

        var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2, T3, T4, T5>(parts!);

        _typeToArchetypeMap[key] = permutatedArchetype;

        return new StaticArchetype<TMask, T1, T2, T3, T4, T5>(permutatedArchetype);
    }

    private StaticArchetype<TMask, T1, T2, T3, T4, T5, T6> GetStaticArchetype<T1, T2, T3, T4, T5, T6>(Type key, Func<List<int>> globalComponentIndexesFunc)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
    {
        if (_typeToArchetypeMap.TryGetValue(key, out var staticArchetype))
            return new StaticArchetype<TMask, T1, T2, T3, T4, T5, T6>((StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>)staticArchetype);

        var globalComponentIndexes = globalComponentIndexesFunc();
        var universalKey = ComputeUniversalKey(globalComponentIndexes);

        if(!_universalKeyToPartsMap.TryGetValue(universalKey, out var parts))
            ThrowHelper.ThrowNotSupportedException($"Archetype {key.FullName} is not initialized!");

        var permutatedArchetype = CreateStaticArchetypeFromParts<T1, T2, T3, T4, T5, T6>(parts!);

        _typeToArchetypeMap[key] = permutatedArchetype;

        return new StaticArchetype<TMask, T1, T2, T3, T4, T5, T6>(permutatedArchetype);
    }
}