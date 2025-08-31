using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid.Internals;

internal interface IStaticArchetypeInstance<TMask>
    where TMask : struct, IBitMask<TMask>
{
    TMask Mask { get; }

    int Count { get; }

    bool HasEntity(EntityId entityId);

    ref T GetComponent<T>(EntityId entityId) where T : struct, IComponent;

    bool TryGetComponent<T>(EntityId entityId, out ComponentRef<T> componentRef) where T : struct, IComponent;

    void DeleteEntity(EntityId entityId);

    Span<EntityId> GetEntities();

    Span<T> GetComponents<T>() where T : struct, IComponent;
}