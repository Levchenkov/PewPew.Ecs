using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.Demo;

public sealed class CommandBufferPositionSystem : SystemBase<Position, Speed>
{
    private CommandBuffer<AliveComponent> _aliveCommandBuffer;
    private FilterDefinition _filterDefinition = new FilterDefinition().With<Position>().With<Speed>().With<AliveComponent>();

    public override void Update()
    {
        using (_aliveCommandBuffer = World2.GetCommandBufferFor<AliveComponent>())
        {
            var filter = World2.GetFilter(_filterDefinition);
            filter.ExecuteQuery<CommandBufferPositionSystem, Position, Speed>(this);
        }
    }

    public override void Update(EntityId entityId, ref Position position, ref Speed speed)
    {
        position.Vector += speed.Vector;
        if (position.Vector.X > 100)
        {
            _aliveCommandBuffer.QueueDeleteComponent(entityId);
        }
    }
}