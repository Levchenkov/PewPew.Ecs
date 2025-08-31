using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core.Features;

// custom feature will be in c#13. It requires 'allows ref struct' generic constrains
public readonly ref struct NameFeature
{
    private readonly EntityIdPool _entityIdPool;
    private readonly Data _data;

    internal NameFeature(EntityIdPool entityIdPool, Data data)
    {
        _entityIdPool = entityIdPool;
        _data = data;
    }

    public EntityId CreateEntityId(string name)
    {
        var entityId = _entityIdPool.Get();

        SetName(entityId, name);

        return entityId;
    }

    public void SetName(EntityId entityId, string name)
    {
        if (_data.NameToEntityIdMap.ContainsKey(name))
            throw new NotSupportedException($"Entity with {name} exists. The name should be unique.");

        _data.NameToEntityIdMap[name] = entityId;
        _data.EntityIdToNameMap[entityId] = name;
    }

    public EntityId GetEntityId(string name) => _data.NameToEntityIdMap[name];

    public string GetName(EntityId entityId) => _data.EntityIdToNameMap[entityId];

    public string WorldName
    {
        get => _data.WorldName;
        set => _data.WorldName = value;
    }

    public class Data
    {
        public readonly Dictionary<string, EntityId> NameToEntityIdMap = new();
        public readonly Dictionary<EntityId, string> EntityIdToNameMap = new();
        public string WorldName;
    }
}