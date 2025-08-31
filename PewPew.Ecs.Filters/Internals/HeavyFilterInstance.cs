using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters.Internals;

internal abstract class HeavyFilterInstanceBase<TMask>
    where TMask : struct, IBitMask<TMask>
{
    private const int InvalidIndex = -1;

    protected internal TMask Mask;

    private readonly int[] _entitiesIndexes;
    private readonly EntityId[] _entities;
    private readonly int[] _componentIndexes;

    private readonly List<int> _globalComponentIndexes;
    private readonly int[][] _entitiesIndexesPerComponent;// _internalIndexesPerComponent[i], i - is not component index
    private readonly int _componentCount;
    private readonly int[] _componentIndexToTypeMap;
    private int _count;

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_entities, 0, _count);
    }

    internal Span<int> ComponentIndexes => new(_componentIndexes, 0, _count * _componentCount);

    internal int[] InternalIndexes => _entitiesIndexes;

    internal IndexedWorld<TMask> World { get; }

    internal int Id { get; }

    public HeavyFilterInstanceBase(
        int id,
        int maxEntities,
        int maxEntitiesFilter,
        TMask mask,
        IndexedWorld<TMask> world,
        List<int> globalComponentIndexes)
    {
        Id = id;
        Mask = mask;
        _globalComponentIndexes = globalComponentIndexes;
        World = world;
        _componentCount = globalComponentIndexes.Count;
        _entitiesIndexesPerComponent = new int[_componentCount][];
        _componentIndexToTypeMap = new int[_componentCount];
        for (int i = 0; i < _componentCount; i++)
        {
            var componentIndex = globalComponentIndexes[i];
            _entitiesIndexesPerComponent[i] = ((IContinuousSparseSet)World.GetComponentCollectionByGlobalIndex(componentIndex)).InternalIndexes;
            _componentIndexToTypeMap[i] = componentIndex;
        }

        _entitiesIndexes = new int[maxEntities];
        _entities = new EntityId[maxEntitiesFilter];
        _componentIndexes = new int[maxEntitiesFilter * _componentCount];

        Reset();
    }

    internal void Reset()
    {
        _count = 0;
        new Span<int>(_entitiesIndexes).Fill(InvalidIndex);
    }

    internal void Restore(EntityId[] entities, int[] indexes, int[] componentIndexes, int count)
    {
        _count = count;
        entities.CopyTo(_entities.AsSpan());
        indexes.CopyTo(_entitiesIndexes.AsSpan());
        componentIndexes.CopyTo(_componentIndexes.AsSpan());
    }

    internal void Backup(out EntityId[] entities, out int[] indexes, out int[] componentIndexes, out int count)
    {
        entities = _entities.ToArray();
        indexes = _entitiesIndexes.ToArray();
        componentIndexes = _componentIndexes.ToArray();
        count = _count;
    }

    internal void AddEntity(EntityId entityId)
    {
        ref var index = ref _entitiesIndexes[entityId.Index];
        if (index != InvalidIndex)
            return;

        var entitiesIndexesPerComponent = _entitiesIndexesPerComponent;
        var componentIndexes = _componentIndexes;

        for (int i = 0; i < _componentCount; i++)
        {
            var internalIndexes = entitiesIndexesPerComponent[i];
            int internalIndex = internalIndexes[entityId.Index];
            componentIndexes[_count * _componentCount + i] = internalIndex;
        }

        _entitiesIndexes[entityId.Index] = _count;
        _entities[_count] = entityId;
        _count++;
    }

    internal void DeleteEntity(EntityId entityId)
    {
        var index = _entitiesIndexes[entityId.Index];
        if (index == InvalidIndex)
            return;

        _count--;

        var replacedEntityId = _entities[index] = _entities[_count];
        _entitiesIndexes[replacedEntityId.Index] = index;
        _entitiesIndexes[entityId.Index] = InvalidIndex;

        var entitiesIndexesPerComponent = _entitiesIndexesPerComponent;
        var componentIndexes = _componentIndexes;
        for (int i = 0; i < _componentCount; i++)
        {
            var internalIndexes = entitiesIndexesPerComponent[i];
            int internalIndex = internalIndexes[replacedEntityId.Index];
            componentIndexes[index * _componentCount + i] = internalIndex;
        }
    }

    internal void UpdateIndex(EntityId entityId, int globalComponentIndex)
    {
        var index = _entitiesIndexes[entityId.Index];
        if (index == InvalidIndex)
            return;

        var globalComponentIndexes = _globalComponentIndexes;
        for (int i = 0; i < globalComponentIndexes.Count; i++)
        {
            if (globalComponentIndexes[i] == globalComponentIndex)
            {
                var internalIndexes = _entitiesIndexesPerComponent[i];
                int internalIndex = internalIndexes[entityId.Index];
                _componentIndexes[index * _componentCount + i] = internalIndex;

                break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntity(EntityId entityId) => _entitiesIndexes[entityId.Index] != InvalidIndex;

    protected int GetComponentShift(int globalComponentIndex)
    {
        for (int i = 0; i < _componentIndexToTypeMap.Length; i++)
        {
            if (_componentIndexToTypeMap[i] == globalComponentIndex)
                return i;
        }

        ThrowHelper.ThrowComponentNotFoundException(globalComponentIndex);

        return -1;
    }
}

internal sealed class HeavyFilterInstance<TMask, T1, T2> : HeavyFilterInstanceBase<TMask>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct
    where T2 : struct
{
    private const int ComponentCount = 2;

    public HeavyFilterInstance(
        int id,
        int maxEntities,
        int maxEntitiesFilter,
        TMask mask,
        IndexedWorld<TMask> world,
        List<int> globalComponentIndexes)
        : base(id, maxEntities, maxEntitiesFilter, mask, world, globalComponentIndexes)
    {
    }

    internal ref T GetComponent<T>(int index)
        where T : struct, IComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        int componentShift = GetComponentShift(globalIndex);
        var componentIndex = ComponentIndexes[index * ComponentCount + componentShift];
        var components = World.GetSparseSet<T>(globalIndex).InternalData;

        return ref components[componentIndex];
    }
}

internal sealed class HeavyFilterInstance<TMask, T1, T2, T3> : HeavyFilterInstanceBase<TMask>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct
    where T2 : struct
    where T3 : struct
{
    private const int ComponentCount = 3;

    public HeavyFilterInstance(
        int id,
        int maxEntities,
        int maxEntitiesFilter,
        TMask mask,
        IndexedWorld<TMask> world,
        List<int> globalComponentIndexes)
        : base(id, maxEntities, maxEntitiesFilter, mask, world, globalComponentIndexes)
    {
    }

    internal ref T GetComponent<T>(int index)
        where T : struct, IComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        int componentShift = GetComponentShift(globalIndex);
        var componentIndex = ComponentIndexes[index * ComponentCount + componentShift];
        var components = World.GetSparseSet<T>(globalIndex).InternalData;

        return ref components[componentIndex];
    }
}