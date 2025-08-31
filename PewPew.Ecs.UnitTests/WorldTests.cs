using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class WorldTests
{
    [Fact]
    public void InitSparseSetFor_InitComponentTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateWorld();

        world.InitComponent<Component1>();
        world.IsComponentInitialized<Component1>().Should().BeTrue();
        world.GetComponents<Component1>();

        var action = () => world.InitComponent<Component1>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitSparseSetFor_InitComponentsMoreThenAllowed_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateWorld(x => x.MaxAllowedUniqueComponentsCount = 2);

        world.InitComponent<Component1>();
        world.InitComponent<Component2>();
        var action = () => world.InitComponent<Component3>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitSparseSetFor_TwoWorldInitComponents_ShouldBeOk()
    {
        var world1 = WorldFactory.Shared.CreateWorld(x => x.MaxAllowedUniqueComponentsCount = 3);
        var world2 = WorldFactory.Shared.CreateWorld(x => x.MaxAllowedUniqueComponentsCount = 2);

        world1.InitComponent<Component1>();
        world1.InitComponent<Component2>();
        world1.InitComponent<Component3>();

        var action = () => world2.InitComponent<Component3>();

        action.Should().NotThrow();
    }

    [Fact]
    public void InitSparseSetFor_TwoWorldInitTheSameComponents_ShouldBeOk()
    {
        var world1 = WorldFactory.Shared.CreateWorld(x => x.MaxAllowedUniqueComponentsCount = 2);
        var world2 = WorldFactory.Shared.CreateWorld(x => x.MaxAllowedUniqueComponentsCount = 2);

        world1.InitComponent<Component1>();
        world1.InitComponent<Component2>();

        world2.InitComponent<Component1>();
        world2.InitComponent<Component2>();
    }

    [Fact]
    public void InitSparseSetFor_InitSingletonTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateWorld();

        world.InitSingleton<HealthComponent>();
        world.IsComponentInitialized<HealthComponent>().Should().BeTrue();
        world.HasSingleton<HealthComponent>();

        var action = () => world.InitSingleton<HealthComponent>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitSparseSetFor_InitTagTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateWorld();

        world.InitTag<EmptyComponent>();
        world.IsComponentInitialized<EmptyComponent>().Should().BeTrue();
        world.GetTags<EmptyComponent>();

        var action = () => world.InitTag<EmptyComponent>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitSparseSetFor_InitStaticBufferTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateWorld();

        world.InitStaticBuffer<Damage>();
        world.IsComponentInitialized<Damage>().Should().BeTrue();
        world.GetStaticBuffers<Damage>();

        var action = () => world.InitStaticBuffer<Damage>();

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitNameFeature_InitTwice_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateWorld();

        world.InitNameFeature();

        var action = () => world.InitNameFeature();;

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void GetComponents_ComponentIsNotInitedBefore_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateWorld();
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
        var world = WorldFactory.Shared.CreateWorld();
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
        var world = WorldFactory.Shared.CreateWorld();
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
        var world = WorldFactory.Shared.CreateWorld();
        world.IsComponentInitialized<Damage>().Should().BeFalse();

        var action = () =>
        {
            world.GetStaticBuffers<Damage>();
        };

        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void GetFeature_ComponentIsNotInitedBefore_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateWorld();

        var action = () =>
        {
            world.GetNameFeature();
        };

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitCommandBufferCache_ComponentIsNotInited_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateWorld();

        var action = () =>
        {
            world.InitCommandBufferCache<Component1>();
        };

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void InitCommandBufferCache_InitTwice_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateWorld();
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
        var world = WorldFactory.Shared.CreateWorld();
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
        var world = WorldFactory.Shared.CreateWorld();
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
        var world = WorldFactory.Shared.CreateWorld();
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
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<Component1>();
        var commandBuffer = world.GetCommandBufferFor<Component1>();

        var action = () =>
        {
            ((ICommandBufferApplier)world).Apply(commandBuffer);
        };

        action.Should().NotThrow();
    }

    // more ApplyCommandBuffer tests you can find at CommandBufferTests

    [DebugOnlyFact]
    public void ComponentValidateEntityId_WrongWorld_ExceptionExpected()
    {
        var world1 = WorldFactory.Shared.CreateWorld();
        world1.InitComponent<HealthComponent>();

        var entityId1 = world1.CreateEntityId();
        world1.AddComponent<HealthComponent>(entityId1);

        var world2 = WorldFactory.Shared.CreateWorld();
        world2.InitComponent<HealthComponent>();

        var entityId2 = world2.CreateEntityId();
        world2.AddComponent<HealthComponent>(entityId2);

        world1.HasComponent<HealthComponent>(entityId1).Should().BeTrue();
        world2.HasComponent<HealthComponent>(entityId2).Should().BeTrue();

        Action action = () => world1.HasComponent<HealthComponent>(entityId2);
        action.Should().Throw<Exception>();

        action = () => world2.HasComponent<HealthComponent>(entityId1);
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.AddComponent<HealthComponent>(entityId2);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.GetComponent<HealthComponent>(entityId2);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.DeleteComponent<HealthComponent>(entityId2);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.TryGetComponent<HealthComponent>(entityId2, out _);
        };
        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void ComponentValidateEntityId_DeadEntityId_ExceptionExpected()
    {
        var world1 = WorldFactory.Shared.CreateWorld();
        world1.InitComponent<HealthComponent>();

        var entityId1 = world1.CreateEntityId();
        world1.AddComponent<HealthComponent>(entityId1);

        world1.DeleteEntityId(entityId1);

        Action action = () => world1.HasComponent<HealthComponent>(entityId1);
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.AddComponent<HealthComponent>(entityId1);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.GetComponent<HealthComponent>(entityId1);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.DeleteComponent<HealthComponent>(entityId1);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.TryGetComponent<HealthComponent>(entityId1, out _);
        };
        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void TagValidateEntityId_WrongWorld_ExceptionExpected()
    {
        var world1 = WorldFactory.Shared.CreateWorld();
        world1.InitTag<EmptyComponent>();

        var entityId1 = world1.CreateEntityId();
        world1.AddTag<EmptyComponent>(entityId1);

        var world2 = WorldFactory.Shared.CreateWorld();
        world2.InitTag<EmptyComponent>();

        var entityId2 = world2.CreateEntityId();
        world2.AddTag<EmptyComponent>(entityId2);

        world1.HasTag<EmptyComponent>(entityId1).Should().BeTrue();
        world2.HasTag<EmptyComponent>(entityId2).Should().BeTrue();

        Action action = () => world1.HasTag<EmptyComponent>(entityId2);
        action.Should().Throw<Exception>();

        action = () => world1.HasTag<EmptyComponent>(entityId2);
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.AddTag<EmptyComponent>(entityId2);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.DeleteTag<EmptyComponent>(entityId2);
        };
        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void TagValidateEntityId_DeadEntityId_ExceptionExpected()
    {
        var world1 = WorldFactory.Shared.CreateWorld();
        world1.InitTag<EmptyComponent>();

        var entityId1 = world1.CreateEntityId();
        world1.AddTag<EmptyComponent>(entityId1);

        world1.DeleteEntityId(entityId1);

        Action action = () => world1.HasTag<EmptyComponent>(entityId1);
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.AddTag<EmptyComponent>(entityId1);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.DeleteTag<EmptyComponent>(entityId1);
        };
        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void StaticBufferValidateEntityId_WrongWorld_ExceptionExpected()
    {
        var world1 = WorldFactory.Shared.CreateWorld();
        world1.InitStaticBuffer<Damage>();

        var entityId1 = world1.CreateEntityId();
        world1.AddStaticBuffer<Damage>(entityId1);

        var world2 = WorldFactory.Shared.CreateWorld();
        world2.InitStaticBuffer<Damage>();

        var entityId2 = world2.CreateEntityId();
        world2.AddStaticBuffer<Damage>(entityId2);

        world1.HasStaticBuffer<Damage>(entityId1).Should().BeTrue();
        world2.HasStaticBuffer<Damage>(entityId2).Should().BeTrue();

        Action action = () => world1.HasStaticBuffer<Damage>(entityId2);
        action.Should().Throw<Exception>();

        action = () => world2.HasStaticBuffer<Damage>(entityId1);
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.AddStaticBuffer<Damage>(entityId2);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.GetStaticBuffer<Damage>(entityId2);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.DeleteStaticBuffer<Damage>(entityId2);
        };
        action.Should().Throw<Exception>();
    }

    [DebugOnlyFact]
    public void StaticBufferValidateEntityId_DeadEntityId_ExceptionExpected()
    {
        var world1 = WorldFactory.Shared.CreateWorld();
        world1.InitStaticBuffer<Damage>();

        var entityId1 = world1.CreateEntityId();
        world1.AddStaticBuffer<Damage>(entityId1);

        world1.DeleteEntityId(entityId1);

        Action action = () => world1.HasStaticBuffer<Damage>(entityId1);
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.AddStaticBuffer<Damage>(entityId1);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.GetStaticBuffer<Damage>(entityId1);
        };
        action.Should().Throw<Exception>();

        action = () =>
        {
            world1.DeleteStaticBuffer<Damage>(entityId1);
        };
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void DeleteEntityId_EntityHasComponents_ShouldDeleteAllComponents()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<HealthComponent>();
        world.InitTag<EmptyComponent>();
        world.InitSingleton<SingletonComponent>();
        world.InitStaticBuffer<Damage>();

        var entityId = world.CreateEntityId();

        world.AddComponent<HealthComponent>(entityId);
        world.AddTag<EmptyComponent>(entityId);
        world.AddStaticBuffer<Damage>(entityId);
        world.AddSingleton<SingletonComponent>();

        world.DeleteEntityId(entityId);

        world.GetComponents<HealthComponent>().Count.Should().Be(0);
        world.GetTags<EmptyComponent>().Count.Should().Be(0);
        world.GetStaticBuffers<Damage>().BufferCount.Should().Be(0);
        world.HasSingleton<SingletonComponent>().Should().BeTrue();
    }
}