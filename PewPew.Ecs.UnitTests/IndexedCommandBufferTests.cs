using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class IndexedCommandBufferTests
{
    private IndexedWorld _world;
    private FilterDefinition _filterDefinition;

    public IndexedCommandBufferTests()
    {
        _world = WorldFactory.Shared.CreateIndexedWorld();
        _world.InitComponent<HealthComponent>();
        _world.InitComponent<Component1>();

        _filterDefinition = new FilterDefinition().With<HealthComponent>().With<Component1>();
    }

    [Fact]
    public void QueueAddComponent_ComponentDoesNotExist_ComponentShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_AddTwice_LastValueShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
            buffer.QueueAddComponent(entityId).Current = 69;

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_ComponentExists_ComponentShouldBeUpdated()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        var healths = _world.GetComponents<HealthComponent>();
        healths.AddComponent(entityId).Current = 69;

        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
            filter.HasEntity(entityId).Should().Be(true);
        }

        healths.GetComponent(entityId).Current.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponentAndQueueDeleteComponent_ComponentDoesNotExist_ComponentShouldNotExist()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;

            filter.HasEntity(entityId).Should().Be(false);

            buffer.QueueDeleteComponent(entityId);

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponents<HealthComponent>().HasComponent(entityId).Should().BeFalse();
        filter.HasEntity(entityId).Should().Be(false);
    }

    [Fact]
    public void QueueDeleteComponentAndQueueAddComponent_ComponentDoesNotExist_ComponentShouldExists()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueDeleteComponent(entityId);

            filter.HasEntity(entityId).Should().Be(false);

            buffer.QueueAddComponent(entityId).Current = 42;

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponents<HealthComponent>().TryGetComponent(entityId, out var wrapper).Should().BeTrue();
        wrapper.Component.Current.Should().Be(42);

        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponentAndQueueDeleteComponent_ComponentExists_ComponentShouldNotExist()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        var healths = _world.GetComponents<HealthComponent>();
        healths.AddComponent(entityId).Current = 69;

        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
            buffer.QueueDeleteComponent(entityId);

            filter.HasEntity(entityId).Should().Be(true);
        }

        _world.GetComponents<HealthComponent>().HasComponent(entityId).Should().BeFalse();
        filter.HasEntity(entityId).Should().Be(false);
    }

    [Fact]
    public void QueueDeleteComponentAndQueueAddComponent_ComponentExists_ComponentShouldExists() // ?
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        var healths = _world.GetComponents<HealthComponent>();
        healths.AddComponent(entityId).Current = 69;

        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueDeleteComponent(entityId);

            filter.HasEntity(entityId).Should().Be(true);

            buffer.QueueAddComponent(entityId).Current = 42;

            filter.HasEntity(entityId).Should().Be(true);
        }

        _world.GetComponents<HealthComponent>().TryGetComponent(entityId, out var wrapper).Should().BeTrue();
        wrapper.Component.Current.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueDeleteComponent_ComponentDoesNotExist_ComponentShouldNotExist()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueDeleteComponent(entityId);
            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponents<HealthComponent>().HasComponent(entityId).Should().BeFalse();
        filter.HasEntity(entityId).Should().Be(false);
    }

    [Fact]
    public void QueueDeleteComponent_ComponentExists_ComponentShouldNotExist()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        var healths = _world.GetComponents<HealthComponent>();
        healths.AddComponent(entityId).Current = 69;

        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueDeleteComponent(entityId);
            filter.HasEntity(entityId).Should().Be(true);
        }

        _world.GetComponents<HealthComponent>().HasComponent(entityId).Should().BeFalse();
        filter.HasEntity(entityId).Should().Be(false);
    }

    [Fact]
    public void QueueAddComponent_ManyComponents_ComponentsShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        filter.Reset();

        var healths = _world.GetComponents<HealthComponent>();
        healths.Reset();

        var count = 20;

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            for (int i = 0; i < count; i++)
            {
                var entityId = _world.CreateEntityId();
                _world.AddComponent<Component1>(entityId);

                buffer.QueueAddComponent(entityId).Current = i;
            }
        }

        healths.Count.Should().Be(count);
        for (int i = 0; i < count; i++)
        {
            filter.HasEntity(healths.Entities[i]).Should().Be(true);
            healths.Components[i].Current.Should().Be(i);
        }
    }

    [Fact]
    public void QueueAddComponent_TwoBuffers_ComponentShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 42;
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer.QueueAddComponent(entityId).Current = 69;
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_NestedBuffers_LastShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer1 = _world.GetCommandBufferFor<HealthComponent>())
        using (var buffer2 = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer1.QueueAddComponent(entityId).Current = 42;
            buffer2.QueueAddComponent(entityId).Current = 69;

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_NestedBuffersDifferentOrder_LastShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer1 = _world.GetCommandBufferFor<HealthComponent>())
        using (var buffer2 = _world.GetCommandBufferFor<HealthComponent>())
        {
            buffer2.QueueAddComponent(entityId).Current = 69;
            buffer1.QueueAddComponent(entityId).Current = 42;

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_NestedBuffersComplexCase_LastShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer1 = _world.GetCommandBufferFor<HealthComponent>())
        {
            using (var buffer2 = _world.GetCommandBufferFor<HealthComponent>())
            {
                buffer1.QueueAddComponent(entityId).Current = 42;
                buffer2.QueueAddComponent(entityId).Current = 69;
                filter.HasEntity(entityId).Should().Be(false);
            }

            _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
            filter.HasEntity(entityId).Should().Be(true);
        }

        _world.GetComponents<HealthComponent>().GetComponent(entityId).Current.Should().Be(69);
        filter.HasEntity(entityId).Should().Be(true);
    }
}