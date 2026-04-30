using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class IndexedWorldTests
{
    [Fact]
    public void InitSparseSetFor_InitComponentTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        world.InitComponent<Component1>();
        world.IsComponentInitialized<Component1>().Should().BeTrue();
        world.GetComponents<Component1>();

        var action = () => world.InitComponent<Component1>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitSparseSetFor_InitComponentsMoreThenAllowed_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld(x => x.MaxAllowedUniqueComponentsCount = 2);

        world.InitComponent<Component1>();
        world.InitComponent<Component2>();
        var action = () => world.InitComponent<Component3>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitSparseSetFor_TwoWorldInitComponents_ShouldBeOk()
    {
        var world1 = WorldFactory.Shared.CreateIndexedWorld(x => x.MaxAllowedUniqueComponentsCount = 3);
        var world2 = WorldFactory.Shared.CreateIndexedWorld(x => x.MaxAllowedUniqueComponentsCount = 2);

        world1.InitComponent<Component1>();
        world1.InitComponent<Component2>();
        world1.InitComponent<Component3>();

        var action = () => world2.InitComponent<Component3>();

        action.Should().NotThrow();
    }

    [Fact]
    public void InitSparseSetFor_TwoWorldInitTheSameComponents_ShouldBeOk()
    {
        var world1 = WorldFactory.Shared.CreateIndexedWorld(x => x.MaxAllowedUniqueComponentsCount = 2);
        var world2 = WorldFactory.Shared.CreateIndexedWorld(x => x.MaxAllowedUniqueComponentsCount = 2);

        world1.InitComponent<Component1>();
        world1.InitComponent<Component2>();

        world2.InitComponent<Component1>();
        world2.InitComponent<Component2>();
    }

    [Fact]
    public void InitSparseSetFor_InitSingletonTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        world.InitSingleton<HealthComponent>();
        world.IsComponentInitialized<HealthComponent>().Should().BeTrue();
        world.HasSingleton<HealthComponent>();

        var action = () => world.InitSingleton<HealthComponent>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitSparseSetFor_InitTagTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        world.InitTag<EmptyComponent>();
        world.IsComponentInitialized<EmptyComponent>().Should().BeTrue();
        world.GetTags<EmptyComponent>();

        var action = () => world.InitTag<EmptyComponent>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitSparseSetFor_InitStaticBufferTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        world.InitStaticBuffer<Damage>();
        world.IsComponentInitialized<Damage>().Should().BeTrue();
        world.GetStaticBuffers<Damage>();

        var action = () => world.InitStaticBuffer<Damage>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitSparseSetFor_InitDynamicBufferTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        world.InitDynamicBuffer<DynamicDamage>();
        world.IsComponentInitialized<DynamicDamage>().Should().BeTrue();
        world.GetDynamicBuffers<DynamicDamage>();

        var action = () => world.InitDynamicBuffer<DynamicDamage>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitNameFeature_InitTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        world.InitNameFeature();

        var action = () => world.InitNameFeature();;

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void GetComponents_ComponentIsNotInitedBefore_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.IsComponentInitialized<Component1>().Should().BeFalse();

        var action = () =>
        {
            world.GetComponents<Component1>();
        };

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void GetSingleton_ComponentIsNotInitedBefore_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.IsComponentInitialized<HealthComponent>().Should().BeFalse();

        var action = () =>
        {
            world.GetSingleton<HealthComponent>();
        };

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void GetTags_ComponentIsNotInitedBefore_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.IsComponentInitialized<EmptyComponent>().Should().BeFalse();

        var action = () =>
        {
            world.GetTags<EmptyComponent>();
        };

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void GetStaticBuffers_ComponentIsNotInitedBefore_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.IsComponentInitialized<Damage>().Should().BeFalse();

        var action = () =>
        {
            world.GetStaticBuffers<Damage>();
        };

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void GetDynamicBuffers_ComponentIsNotInitedBefore_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.IsComponentInitialized<DynamicDamage>().Should().BeFalse();

        var action = () =>
        {
            world.GetDynamicBuffers<DynamicDamage>();
        };

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void TryGetDynamicBuffer_ComponentIsNotInitedBefore_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        var entityId = world.CreateEntityId();

        var action = () => world.TryGetDynamicBuffer<DynamicDamage>(entityId, out _);

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void GetFeature_ComponentIsNotInitedBefore_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        var action = () =>
        {
            world.GetNameFeature();
        };

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitCommandBufferCache_ComponentIsNotInited_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();

        var action = () =>
        {
            world.InitCommandBufferCache<Component1>();
        };

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitCommandBufferCache_InitTwice_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitCommandBufferCache<Component1>();

        var action = () =>
        {
            world.InitCommandBufferCache<Component1>();
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void InitCommandBufferCache_AfterGetCommandBufferFor_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.GetCommandBufferFor<Component1>();

        var action = () =>
        {
            world.InitCommandBufferCache<Component1>();
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void ApplyCommandBuffer_ComponentIsNotInited_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.IsComponentInitialized<Component1>().Should().BeFalse();

        var action = () =>
        {
            ((ICommandBufferApplier)world).Apply(default(CommandBuffer<Component1>));
        };

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void ApplyCommandBuffer_Default_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();

        var action = () =>
        {
            ((ICommandBufferApplier)world).Apply(default(CommandBuffer<Component1>));
        };

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void ApplyCommandBuffer_Empty_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        var commandBuffer = world.GetCommandBufferFor<Component1>();

        var action = () =>
        {
            ((ICommandBufferApplier)world).Apply(commandBuffer);
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void DynamicBuffer_TryGetAddDelete_ShouldWork()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitDynamicBuffer<DynamicDamage>();

        var entityId = world.CreateEntityId();

        world.TryGetDynamicBuffer<DynamicDamage>(entityId, out _).Should().BeFalse();

        world.AddDynamicBuffer<DynamicDamage>(entityId).AddLast(new DynamicDamage { Value = 55 });
        world.TryGetDynamicBuffer<DynamicDamage>(entityId, out var existing).Should().BeTrue();
        existing.Components[0].Value.Should().Be(55);

        world.DeleteDynamicBuffer<DynamicDamage>(entityId);
        world.TryGetDynamicBuffer<DynamicDamage>(entityId, out _).Should().BeFalse();
    }

    [DebugOnlyFact]
    public void DynamicBuffer_TryGet_EntityFromAnotherWorld_ExceptionExpected()
    {
        var world1 = WorldFactory.Shared.CreateIndexedWorld();
        world1.InitDynamicBuffer<DynamicDamage>();

        var world2 = WorldFactory.Shared.CreateIndexedWorld();
        world2.InitDynamicBuffer<DynamicDamage>();

        var entityFromWorld2 = world2.CreateEntityId();
        world2.AddDynamicBuffer<DynamicDamage>(entityFromWorld2);

        var action = () => world1.TryGetDynamicBuffer<DynamicDamage>(entityFromWorld2, out _);

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void DynamicBuffer_TryGet_DeadEntity_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitDynamicBuffer<DynamicDamage>();

        var entity = world.CreateEntityId();
        world.AddDynamicBuffer<DynamicDamage>(entity);
        world.DeleteEntityId(entity);

        var action = () => world.TryGetDynamicBuffer<DynamicDamage>(entity, out _);

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void GetComponents_ComponentBigGlobalIndex_ShouldBeOk()
    {
        ComponentMetadata<BigGlobalIndex>.DebugSetGlobalIndex(100);

        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<BigGlobalIndex>();

        Action action = () => world.GetComponents<BigGlobalIndex>();
        action.Should().NotThrow();

        var entityId = world.CreateEntityId();
        action = () => world.AddComponent<BigGlobalIndex>(entityId);
        action.Should().NotThrow();
    }

    public struct BigGlobalIndex : IComponent
    {
    }

    // more ApplyCommandBuffer tests you can find at IndexedCommandBufferTests.cs

    // For tests AddComponent and DeleteComponent look at FilterTests.cs
    // For tests AddTag and DeleteTag look at FilterTests.Tag.cs
}