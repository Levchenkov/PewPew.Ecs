using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class MultiWorldTests
{
    [Fact]
    public void InitComponent_TheSameComponent_ShouldBeOk()
    {
        var worldFactory = new WorldFactory();

        var world1 = worldFactory.CreateWorld();
        world1.InitComponent<HealthComponent>();

        var world2 = worldFactory.CreateWorld();
        world2.InitComponent<HealthComponent>();

        world1.Id.Should().Be(1);
        world1.IsComponentInitialized<HealthComponent>().Should().BeTrue();

        world2.Id.Should().Be(2);
        world2.IsComponentInitialized<HealthComponent>().Should().BeTrue();
    }

    [Fact]
    public void InitTag_TheSameComponent_ShouldBeOk()
    {
        var world1 = WorldFactory.Shared.CreateWorld();
        world1.InitTag<EmptyComponent>();

        var world2 = WorldFactory.Shared.CreateWorld();
        world2.InitTag<EmptyComponent>();

        world1.IsComponentInitialized<EmptyComponent>().Should().BeTrue();

        world2.IsComponentInitialized<EmptyComponent>().Should().BeTrue();
    }

    [Fact]
    public void InitSingleton_TheSameComponent_ShouldBeOk()
    {
        var world1 = WorldFactory.Shared.CreateWorld();
        world1.InitSingleton<SingletonComponent>();

        var world2 = WorldFactory.Shared.CreateWorld();
        world2.InitSingleton<SingletonComponent>();

        world1.IsComponentInitialized<SingletonComponent>().Should().BeTrue();

        world2.IsComponentInitialized<SingletonComponent>().Should().BeTrue();
    }

    [Fact]
    public void InitStaticBuffer_TheSameComponent_ShouldBeOk()
    {
        var world1 = WorldFactory.Shared.CreateWorld();
        world1.InitStaticBuffer<Damage>();

        var world2 = WorldFactory.Shared.CreateWorld();
        world2.InitStaticBuffer<Damage>();

        world1.IsComponentInitialized<Damage>().Should().BeTrue();

        world2.IsComponentInitialized<Damage>().Should().BeTrue();
    }

    [Fact]
    public void InitComponent_MoreThenConfigured_ExceptionExpected()
    {
        var world1 = WorldFactory.Shared.CreateWorld(settings => settings.MaxAllowedUniqueComponentsCount = 2);
        world1.InitComponent<Component1>();
        world1.InitComponent<Component2>();

        var addOneMoreComponent = () =>
        {
            world1.InitComponent<Component3>();
        };
        addOneMoreComponent.Should().Throw<Exception>();

        var world2 = WorldFactory.Shared.CreateWorld(settings => settings.MaxAllowedUniqueComponentsCount = 2);
        world2.InitComponent<Component3>();
        world2.InitComponent<Component4>();

        addOneMoreComponent = () =>
        {
            world2.InitComponent<Component5>();
        };
        addOneMoreComponent.Should().Throw<Exception>();

        world1.IsComponentInitialized<Component1>().Should().BeTrue();
        world1.IsComponentInitialized<Component2>().Should().BeTrue();
        world1.IsComponentInitialized<Component3>().Should().BeFalse();

        world2.IsComponentInitialized<Component3>().Should().BeTrue();
        world2.IsComponentInitialized<Component4>().Should().BeTrue();
        world2.IsComponentInitialized<Component5>().Should().BeFalse();
    }

    [Fact]
    public void InitNonMaskableComponents_MoreThenConfigured_ShouldBeOk()
    {
        var world1 = WorldFactory.Shared.CreateWorld(settings => settings.MaxAllowedUniqueComponentsCount = 2);
        world1.InitComponent<Component1>();
        world1.InitComponent<Component2>();

        var addTag = () =>
        {
            world1.InitTag<EmptyComponent>();
        };
        addTag.Should().Throw<Exception>();

        var addStaticBuffer = () =>
        {
            world1.InitStaticBuffer<Damage>();
        };
        addStaticBuffer.Should().NotThrow<Exception>();

        var addSingleton = () =>
        {
            world1.InitSingleton<SingletonComponent>();
        };
        addSingleton.Should().NotThrow<Exception>();

        world1.IsComponentInitialized<Component1>().Should().BeTrue();
        world1.IsComponentInitialized<Component2>().Should().BeTrue();
        world1.IsComponentInitialized<EmptyComponent>().Should().BeFalse();
        world1.IsComponentInitialized<Damage>().Should().BeTrue();
        world1.IsComponentInitialized<SingletonComponent>().Should().BeTrue();
    }
}