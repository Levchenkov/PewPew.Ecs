using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters.Internals;

internal sealed class FilterInstance<TMask>
    where TMask : struct, IBitMask<TMask>
{
    private const int InvalidIndex = -1;

    internal readonly int Id;
    internal readonly TMask Mask;
    internal readonly IndexedWorld<TMask> World;

    private readonly EntityId[] _entities;
    private readonly int[] _entitiesIndexes;
    private int _count;

    internal int[] InternalIndexes => _entitiesIndexes;

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_entities, 0, _count);
    }

    public FilterInstance(int id, int maxEntities, int maxEntitiesFilter, TMask mask, IndexedWorld<TMask> world)
    {
        Id = id;
        Mask = mask;
        World = world;

        _entities = new EntityId[maxEntitiesFilter];
        _entitiesIndexes = new int[maxEntities];

        Reset();
    }

    internal void Reset()
    {
        _count = 0;
        new Span<int>(_entitiesIndexes).Fill(InvalidIndex);
    }

    internal void Restore(EntityId[] entities, int[] indexes, int count)
    {
        _count = count;
        entities.CopyTo(_entities.AsSpan());
        indexes.CopyTo(_entitiesIndexes.AsSpan());
    }

    internal void Backup(out EntityId[] entities, out int[] indexes, out int count)
    {
        entities = _entities.ToArray();
        indexes = _entitiesIndexes.ToArray();
        count = _count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddEntity(EntityId entityId)
    {
        ref var index = ref _entitiesIndexes[entityId.Index];
        if (index != InvalidIndex)
            return;

        _entitiesIndexes[entityId.Index] = _count;
        _entities[_count] = entityId;
        _count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void DeleteEntity(EntityId entityId)
    {
        var index = _entitiesIndexes[entityId.Index];
        if (index == InvalidIndex)
            return;

        _count--;

        var replacedEntityId = _entities[index] = _entities[_count];
        _entitiesIndexes[replacedEntityId.Index] = index;
        _entitiesIndexes[entityId.Index] = InvalidIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntity(EntityId entityId) => _entitiesIndexes[entityId.Index] != InvalidIndex;
}