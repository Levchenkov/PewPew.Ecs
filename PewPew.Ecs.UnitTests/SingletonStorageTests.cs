using PewPew.Ecs.Core.Exceptions;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class SingletonStorageTests
{
    [Fact]
    public void HasComponent_EmptySet_ShouldBeFalse()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        var result = sparseSet.HasComponent();

        result.Should().BeFalse();
    }

    [Fact]
    public void HasComponent_ComponentExists_ShouldBeTrue()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        sparseSet.AddComponent();

        var result = sparseSet.HasComponent();

        result.Should().BeTrue();
    }

    [Fact]
    public void GetComponent_OneComponentExists_ShouldGet()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        sparseSet.AddComponent().Max = 42;

        ref var c = ref sparseSet.GetComponent();
        c.Max.Should().Be(42);
    }

    [Fact]
    public void GetComponent_AddTwice_ShouldGetTheSecond()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        sparseSet.AddComponent().Max = 42;
        sparseSet.AddComponent().Max = 69;

        ref var c = ref sparseSet.GetComponent();
        c.Max.Should().Be(69);
    }

    [Fact]
    public void GetComponent_ComponentDoesntExist_ShouldThrow()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        var action = () => sparseSet.GetComponent();

#if DEBUG
        action.Should().ThrowExactly<ComponentNotFoundException>();
#endif

    }

    [Fact]
    public void AddComponent_AddTwiceTheSameEntity_ShouldReturnExisting()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        sparseSet.AddComponent().Max = 42;
        sparseSet.AddComponent().Max.Should().Be(42);
    }

    [Fact]
    public void DeleteComponent_ComponentDoesNotExist_ShouldBeOk()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        sparseSet.HasComponent().Should().BeFalse();
        sparseSet.DeleteComponent();
        sparseSet.HasComponent().Should().BeFalse();
    }

    [Fact]
    public void DeleteComponent_ComponentExists_ShouldBeOk()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        sparseSet.AddComponent().Max = 42;
        sparseSet.DeleteComponent();

        sparseSet.HasComponent().Should().BeFalse();
    }

    [Fact]
    public void TryGet_Empty_ShouldBeFalse()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        var hasComponent = sparseSet.TryGetComponent(out var wrapper);

        hasComponent.Should().BeFalse();
    }

    [Fact]
    public void TryGet_ComponentExists_ShouldBeTrue()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        sparseSet.AddComponent().Max = 42;

        var hasComponent = sparseSet.TryGetComponent(out var wrapper);

        hasComponent.Should().BeTrue();

        wrapper.Component.Max.Should().Be(42);
    }

    [Fact]
    public void TryGet_ComponentHasBeenDeleted_ShouldBeFalse()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        sparseSet.AddComponent().Max = 42;
        sparseSet.DeleteComponent();

        var hasComponent = sparseSet.TryGetComponent(out var wrapper);

        hasComponent.Should().BeFalse();
    }

    [Fact]
    public void TryGetAnd_ComponentExistsAndHasBeenChanged_ShouldBeTrue()
    {
        var sparseSet = new SingletonStorage<HealthComponent>();

        sparseSet.AddComponent().Max = 42;

        var hasComponent = sparseSet.TryGetComponent(out var wrapper);

        hasComponent.Should().BeTrue();

        wrapper.Component.Max.Should().Be(42);

        wrapper.Component.Max = 69;

        sparseSet.GetComponent().Max.Should().Be(69);
    }
}