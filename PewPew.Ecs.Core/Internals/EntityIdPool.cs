namespace PewPew.Ecs.Core.Internals;

internal class EntityIdPool : IdPool<EntityId> , IEntityManager
{
    public EntityIdPool(ushort worldId, int maxCount) : base(worldId, maxCount, EntityId.Invalid)
    {
    }

    protected override EntityId Create(int index, ushort initialGen, ushort worldId)
    {
        return new EntityId(index, initialGen, worldId);
    }

    protected override ushort GetNextGen(EntityId entityId)
    {
        return (ushort)(entityId.Generation == ushort.MaxValue ? InitialGen : entityId.Generation + 1);
    }

    public int EntityCapacity => Capacity;

    public Span<EntityId> Entities => Values;

    public EntityId CreateEntityId() => Get();

    public void DeleteEntityId(EntityId entityId) => Return(entityId);
}