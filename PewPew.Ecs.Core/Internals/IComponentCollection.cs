namespace PewPew.Ecs.Core.Internals;

internal interface IComponentCollection
{
    Span<EntityId> Entities { get; }
    bool HasComponent(EntityId entityId);
    void DeleteComponent(EntityId entityId);
}