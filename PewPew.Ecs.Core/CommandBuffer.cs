using System.Diagnostics;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public struct CommandBuffer<T> : IDisposable
    where T : struct, IComponent
{
    internal readonly DenseSet<T> AddedComponents;

    internal readonly List<EntityId> DeletedEntities;

    internal readonly HashSet<EntityId> AddedEntities;

    internal readonly bool IsInitialized;

    private readonly ICommandBufferApplier _commandBufferApplier;
    private readonly IEntityManager _entityManager;

    internal CommandBuffer(ICommandBufferApplier commandBufferApplier, IEntityManager entityManager, DenseSet<T> addedComponents, HashSet<EntityId> addedEntities, List<EntityId> deletedEntities)
    {
        _commandBufferApplier = commandBufferApplier;
        _entityManager = entityManager;
        AddedComponents = addedComponents;
        DeletedEntities = deletedEntities;
        AddedEntities = addedEntities;
        IsInitialized = true;
    }

    public ref T QueueAddComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        AddedEntities.Add(entityId);
        return ref AddedComponents.Add(entityId);
    }

    public void QueueDeleteComponent(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        DeletedEntities.Add(entityId);

        if (AddedEntities.Remove(entityId))
        {
            var index = AddedComponents.FindIndex(entityId);
            AddedComponents.Delete(index);
        }
    }

    public void Apply()
    {
        _commandBufferApplier.Apply(this);
    }

    public void Dispose()
    {
        Apply();
    }

    [Conditional("DEBUG")]
    private void DebugValidateEntityId(EntityId entityId)
    {
        if(!_entityManager.IsAlive(entityId))
            ThrowHelper.ThrowNotSupportedException($"Component {typeof(T).Name}. Entity {entityId} is not alive.");
    }
}