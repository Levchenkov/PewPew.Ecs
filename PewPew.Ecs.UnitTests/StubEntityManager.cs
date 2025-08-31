using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class StubEntityManager : IEntityManager
{
    private int _count;

    public int EntityCapacity => throw new NotSupportedException();

    public Span<EntityId> Entities => throw new NotSupportedException();

    public EntityId CreateEntityId()
    {
        return new EntityId(_count++, 1, 0);
    }

    public void DeleteEntityId(EntityId entityId)
    {
    }

    public bool IsAlive(EntityId entityId)
    {
        return true;
    }
}