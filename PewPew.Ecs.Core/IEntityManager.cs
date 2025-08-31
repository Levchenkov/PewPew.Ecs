namespace PewPew.Ecs.Core;

public interface IEntityManager
{
    int EntityCapacity { get; }

    Span<EntityId> Entities { get; }

    EntityId CreateEntityId();

    void DeleteEntityId(EntityId entityId);

    bool IsAlive(EntityId entityId);
}