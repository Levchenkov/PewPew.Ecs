using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class CommandBufferTests
{
    private World _world;

    public CommandBufferTests()
    {
        _world = new World();
        _world.InitComponent<HealthComponent>();
    }

    [Fact]
    public void QueueAddComponent_ComponentDoesNotExist_ComponentShouldBeAdded()
    {
        var entityId = _world.CreateEntityId();
        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(42);
    }

    [Fact]
    public void QueueAddComponent_AddTwice_LastValueShouldBeAdded()
    {
        var entityId = _world.CreateEntityId();
        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
            buffer.QueueAddComponent(entityId).Current = 69;
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
    }

    [Fact]
    public void QueueAddComponent_ComponentExists_ComponentShouldBeUpdated()
    {
        var entityId = _world.CreateEntityId();
        var healths = _world.GetComponents<HealthComponent>();
        healths.AddComponent(entityId).Current = 69;

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
        }

        healths.GetComponent(entityId).Current.Should().Be(42);
    }

    [Fact]
    public void QueueAddComponentAndQueueDeleteComponent_ComponentDoesNotExist_ComponentShouldNotExist()
    {
        var entityId = _world.CreateEntityId();
        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
            buffer.QueueDeleteComponent(entityId);
        }

        _world.GetComponents<HealthComponent>().HasComponent(entityId).Should().BeFalse();
    }

    [Fact]
    public void QueueDeleteComponentAndQueueAddComponent_ComponentDoesNotExist_ComponentShouldExists()
    {
        var entityId = _world.CreateEntityId();
        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueDeleteComponent(entityId);
            buffer.QueueAddComponent(entityId).Current = 42;
        }

        _world.GetComponents<HealthComponent>().TryGetComponent(entityId, out var wrapper).Should().BeTrue();
        wrapper.Component.Current.Should().Be(42);
    }

    [Fact]
    public void QueueAddComponentAndQueueDeleteComponent_ComponentExists_ComponentShouldNotExist()
    {
        var entityId = _world.CreateEntityId();
        var healths = _world.GetComponents<HealthComponent>();
        healths.AddComponent(entityId).Current = 69;

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
            buffer.QueueDeleteComponent(entityId);
        }

        _world.GetComponents<HealthComponent>().HasComponent(entityId).Should().BeFalse();
    }

    [Fact]
    public void QueueDeleteComponentAndQueueAddComponent_ComponentExists_ComponentShouldExists() // ?
    {
        var entityId = _world.CreateEntityId();
        var healths = _world.GetComponents<HealthComponent>();
        healths.AddComponent(entityId).Current = 69;

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueDeleteComponent(entityId);
            buffer.QueueAddComponent(entityId).Current = 42;
        }

        _world.GetComponents<HealthComponent>().TryGetComponent(entityId, out var wrapper).Should().BeTrue();
        wrapper.Component.Current.Should().Be(42);
    }

    [Fact]
    public void QueueDeleteComponent_ComponentDoesNotExist_ComponentShouldNotExist()
    {
        var entityId = _world.CreateEntityId();
        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueDeleteComponent(entityId);
        }

        _world.GetComponents<HealthComponent>().HasComponent(entityId).Should().BeFalse();
    }

    [Fact]
    public void QueueDeleteComponent_ComponentExists_ComponentShouldNotExist()
    {
        var entityId = _world.CreateEntityId();
        var healths = _world.GetComponents<HealthComponent>();
        healths.AddComponent(entityId).Current = 69;
        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueDeleteComponent(entityId);
        }

        _world.GetComponents<HealthComponent>().HasComponent(entityId).Should().BeFalse();
    }

    [Fact]
    public void QueueAddComponent_ManyComponents_ComponentsShouldBeAdded()
    {
        var healths = _world.GetComponents<HealthComponent>();
        healths.Reset();

        var count = 20;

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            for (int i = 0; i < count; i++)
            {
                var entityId = _world.CreateEntityId();
                buffer.QueueAddComponent(entityId).Current = i;
            }
        }

        healths.Count.Should().Be(count);
        for (int i = 0; i < count; i++)
        {
            healths.Components[i].Current.Should().Be(i);
        }
    }

    [Fact]
    public void QueueAddComponent_TwoBuffers_ComponentShouldBeAdded()
    {
        var entityId = _world.CreateEntityId();
        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(42);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 69;
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
    }

    [Fact]
    public void QueueAddComponent_NestedBuffers_LastShouldBeAdded()
    {
        var entityId = _world.CreateEntityId();
        using (var buffer1 = _world.GetCommandBufferFor<HealthComponent>())
        using (var buffer2 = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer1.QueueAddComponent(entityId).Current = 42;
            buffer2.QueueAddComponent(entityId).Current = 69;
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
    }

    [Fact]
    public void QueueAddComponent_NestedBuffersDifferentOrder_LastShouldBeAdded()
    {
        var entityId = _world.CreateEntityId();
        using (var buffer1 = _world.GetCommandBufferFor<HealthComponent>())
        using (var buffer2 = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer2.QueueAddComponent(entityId).Current = 69;
            buffer1.QueueAddComponent(entityId).Current = 42;
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(42);
    }

    [Fact]
    public void QueueAddComponent_NestedBuffersComplexCase_LastShouldBeAdded()
    {
        var entityId = _world.CreateEntityId();
        using (var buffer1 = _world.GetCommandBufferFor<HealthComponent>())
        {
            using (var buffer2 = _world.GetCommandBufferFor<HealthComponent>())
            {
                buffer1.QueueAddComponent(entityId).Current = 42;
                buffer2.QueueAddComponent(entityId).Current = 69;
            }

            _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
    }
}