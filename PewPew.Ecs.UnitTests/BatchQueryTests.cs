using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Hybrid;

namespace PewPew.Ecs.UnitTests;

public class BatchQueryTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void Test(int count)
    {
        HybridWorld world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Position, Speed>();
        var staticArchetype = world.GetStaticArchetype<Position, Speed>();

        var alivePlayerDefinition = new FilterDefinition().With<Position>().With<Speed>();

        for (int i = 0; i < count; i++)
        {
            var entityId = world.CreateEntityId();
            staticArchetype.Add(entityId, new Position { Vector = Vector3.Zero }, new Speed { Vector = Vector3.One });
        }

        world.ExecuteBatchQuery<MovementBatchQuery, Position, Speed>(alivePlayerDefinition, default);

        foreach (var entityId in staticArchetype.Entities)
        {
            var componentRef = staticArchetype.Get(entityId);
            componentRef.Component1.Vector.Should().Be(Vector3.One);
            componentRef.Component2.Vector.Should().Be(Vector3.One);
        }
    }
}


public readonly struct MovementBatchQuery : IBatchQuery<Position, Speed>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BatchUpdate(Span<Position> positions, Span<Speed> speeds)
    {
        int vectorCount = Vector<float>.Count; // 8
        int length = positions.Length - positions.Length % vectorCount;

        Span<Vector<float>> positionVectors = MemoryMarshal.Cast<Position, Vector<float>>(positions.Slice(0, length));
        Span<Vector<float>> speedVectors = MemoryMarshal.Cast<Speed, Vector<float>>(speeds.Slice(0, length));

        for (int i = 0; i < positionVectors.Length; i++)
            positionVectors[i] += speedVectors[i];

        for (int i = length; i < positions.Length; i++)
            positions[i].Vector += speeds[i].Vector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SparseUpdate(ref Position position, ref Speed speed)
    {
        position.Vector += speed.Vector;
    }
}

public struct Position : IComponent
{
    public Vector3 Vector;
}

public struct Speed : IComponent
{
    public Vector3 Vector;
}