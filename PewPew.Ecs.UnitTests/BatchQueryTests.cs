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
        Span<float> floatPositions = MemoryMarshal.Cast<Position, float>(positions);
        Span<float> floatSpeeds = MemoryMarshal.Cast<Speed, float>(speeds);

        int length = floatPositions.Length - floatPositions.Length % 8;

        Span<Vector256<float>> ints = MemoryMarshal.Cast<float, Vector256<float>>(floatPositions.Slice(0, length));
        Span<Vector256<float>> a = MemoryMarshal.Cast<float, Vector256<float>>(floatSpeeds.Slice(0, length));

        for (int i = 0; i < ints.Length; i++)
            ints[i] += a[i];

        for (int i = length; i < floatPositions.Length; i++)
            floatPositions[i] += floatSpeeds[i];
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