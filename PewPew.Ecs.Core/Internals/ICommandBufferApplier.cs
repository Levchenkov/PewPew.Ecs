namespace PewPew.Ecs.Core.Internals;

internal interface ICommandBufferApplier
{
    void Apply<T>(CommandBuffer<T> buffer) where T : struct, IComponent;
}