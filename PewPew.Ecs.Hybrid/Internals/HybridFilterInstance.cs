using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid.Internals;

internal class HybridFilterInstance<TMask>
    where TMask : struct, IBitMask<TMask>
{
    private const int InvalidIndex = -1;

    private readonly EntityId[] _entities;
    private readonly int[] _entitiesIndexes;

    private int _sparseCount;

    internal readonly int Id;
    internal readonly TMask Mask;
    internal readonly HybridWorld<TMask> World;
    internal readonly IStaticArchetypeInstance<TMask>[] FullArchetypes;

    public HybridFilterInstance(
        int id,
        int maxEntities,
        int maxEntitiesFilter,
        TMask mask,
        HybridWorld<TMask> world,
        IStaticArchetypeInstance<TMask>[] fullArchetypes)
    {
        Id = id;
        Mask = mask;
        World = world;
        FullArchetypes = fullArchetypes;

        _entities = new EntityId[maxEntitiesFilter];
        _entitiesIndexes = new int[maxEntities];

        Reset();
    }

    internal Span<EntityId> SparseEntities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_entities, 0, _sparseCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntity(EntityId entityId) => World.HasMask(entityId, Mask);

    internal void Reset()
    {
        _sparseCount = 0;
        new Span<int>(_entitiesIndexes).Fill(InvalidIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddSparseEntity(EntityId entityId)
    {
        ref var index = ref _entitiesIndexes[entityId.Index];
        if (index != InvalidIndex)
            return;

        index = _sparseCount;
        _entities[index] = entityId;
        _sparseCount++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void DeleteEntity(EntityId entityId)
    {
        ref var index = ref _entitiesIndexes[entityId.Index];
        if (index == InvalidIndex)
            return;

        _sparseCount--;

        var replacedEntityId = _entities[index] = _entities[_sparseCount];
        _entitiesIndexes[replacedEntityId.Index] = index;
        index = InvalidIndex;
    }
}