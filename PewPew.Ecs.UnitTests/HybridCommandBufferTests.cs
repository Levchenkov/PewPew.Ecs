using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Hybrid;

namespace PewPew.Ecs.UnitTests;

public class HybridCommandBufferTests
{
    private HybridWorld _world;
    private FilterDefinition _filterDefinition;

    public HybridCommandBufferTests()
    {
        _world = WorldFactory.Shared.CreateHybridWorld();
        _world.InitComponent<Component1>();
        _world.InitComponent<Component2>();
        _world.InitStaticArchetype<Component1, Component2>();

        _filterDefinition = new FilterDefinition().With<Component2>().With<Component1>();
    }

    [Fact]
    public void QueueAddComponent_ComponentDoesNotExist_ComponentShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueAddComponent(entityId).Value = 42;
            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponent<Component2>(entityId).Value.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_EntityHasArchetype_ExceptionExpected()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddStaticArchetype<Component1, Component2>(entityId);

        filter.HasEntity(entityId).Should().Be(true);

        var action = () =>
        {
            using var buffer = _world.GetCommandBufferFor<Component2>();
            buffer.QueueAddComponent(entityId).Value = 42;
        };
        action.Should().Throw<Exception>();

        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueDeleteComponent_EntityHasArchetype_ExceptionExpected()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddStaticArchetype<Component1, Component2>(entityId);

        filter.HasEntity(entityId).Should().Be(true);

        var action = () =>
        {
            using var buffer = _world.GetCommandBufferFor<Component2>();
            buffer.QueueDeleteComponent(entityId);
        };
        action.Should().Throw<Exception>();

        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_AddTwice_LastValueShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueAddComponent(entityId).Value = 42;
            buffer.QueueAddComponent(entityId).Value = 69;

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponent<Component2>(entityId).Value.Should().Be(69);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_ComponentExists_ComponentShouldBeUpdated()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        _world.AddComponent<Component2>(entityId).Value = 69;

        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueAddComponent(entityId).Value = 42;
            filter.HasEntity(entityId).Should().Be(true);
        }

        _world.GetComponent<Component2>(entityId).Value.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponentAndQueueDeleteComponent_ComponentDoesNotExist_ComponentShouldNotExist()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueAddComponent(entityId).Value = 42;

            filter.HasEntity(entityId).Should().Be(false);

            buffer.QueueDeleteComponent(entityId);

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.HasComponent<Component2>(entityId).Should().BeFalse();
        filter.HasEntity(entityId).Should().Be(false);
    }

    [Fact]
    public void QueueDeleteComponentAndQueueAddComponent_ComponentDoesNotExist_ComponentShouldExists()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueDeleteComponent(entityId);

            filter.HasEntity(entityId).Should().Be(false);

            buffer.QueueAddComponent(entityId).Value = 42;

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.TryGetComponent<Component2>(entityId, out var wrapper).Should().BeTrue();
        wrapper.Component.Value.Should().Be(42);

        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponentAndQueueDeleteComponent_ComponentExists_ComponentShouldNotExist()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        _world.AddComponent<Component2>(entityId).Value = 69;

        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueAddComponent(entityId).Value = 42;
            buffer.QueueDeleteComponent(entityId);

            filter.HasEntity(entityId).Should().Be(true);
        }

        _world.HasComponent<Component2>(entityId).Should().BeFalse();
        filter.HasEntity(entityId).Should().Be(false);
    }

    [Fact]
    public void QueueDeleteComponentAndQueueAddComponent_ComponentExists_ComponentShouldExists() // ?
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        _world.AddComponent<Component2>(entityId).Value = 69;

        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueDeleteComponent(entityId);

            filter.HasEntity(entityId).Should().Be(true);

            buffer.QueueAddComponent(entityId).Value = 42;

            filter.HasEntity(entityId).Should().Be(true);
        }

        _world.TryGetComponent<Component2>(entityId, out var wrapper).Should().BeTrue();
        wrapper.Component.Value.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueDeleteComponent_ComponentDoesNotExist_ComponentShouldNotExist()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueDeleteComponent(entityId);
            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.HasComponent<Component2>(entityId).Should().BeFalse();
        filter.HasEntity(entityId).Should().Be(false);
    }

    [Fact]
    public void QueueDeleteComponent_ComponentExists_ComponentShouldNotExist()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        _world.AddComponent<Component2>(entityId).Value = 69;

        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueDeleteComponent(entityId);
            filter.HasEntity(entityId).Should().Be(true);
        }

        _world.HasComponent<Component2>(entityId).Should().BeFalse();
        filter.HasEntity(entityId).Should().Be(false);
    }

    [Fact]
    public void QueueAddComponent_TwoBuffers_ComponentShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueAddComponent(entityId).Value = 42;
        }

        _world.GetComponent<Component2>(entityId).Value.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);

        using (var buffer = _world.GetCommandBufferFor<Component2>())
        {
            buffer.QueueAddComponent(entityId).Value = 69;
        }

        _world.GetComponent<Component2>(entityId).Value.Should().Be(69);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_NestedBuffers_LastShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer1 = _world.GetCommandBufferFor<Component2>())
        using (var buffer2 = _world.GetCommandBufferFor<Component2>())
        {
            buffer1.QueueAddComponent(entityId).Value = 42;
            buffer2.QueueAddComponent(entityId).Value = 69;

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponent<Component2>(entityId).Value.Should().Be(69);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_NestedBuffersDifferentOrder_LastShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer1 = _world.GetCommandBufferFor<Component2>())
        using (var buffer2 = _world.GetCommandBufferFor<Component2>())
        {
            buffer2.QueueAddComponent(entityId).Value = 69;
            buffer1.QueueAddComponent(entityId).Value = 42;

            filter.HasEntity(entityId).Should().Be(false);
        }

        _world.GetComponent<Component2>(entityId).Value.Should().Be(42);
        filter.HasEntity(entityId).Should().Be(true);
    }

    [Fact]
    public void QueueAddComponent_NestedBuffersComplexCase_LastShouldBeAdded()
    {
        var filter = _world.GetFilter(_filterDefinition);
        var entityId = _world.CreateEntityId();
        _world.AddComponent<Component1>(entityId);

        filter.HasEntity(entityId).Should().Be(false);

        using (var buffer1 = _world.GetCommandBufferFor<Component2>())
        {
            using (var buffer2 = _world.GetCommandBufferFor<Component2>())
            {
                buffer1.QueueAddComponent(entityId).Value = 42;
                buffer2.QueueAddComponent(entityId).Value = 69;
                filter.HasEntity(entityId).Should().Be(false);
            }

            _world.GetComponent<Component2>(entityId).Value.Should().Be(69);
            filter.HasEntity(entityId).Should().Be(true);
        }

        _world.GetComponent<Component2>(entityId).Value.Should().Be(69);
        filter.HasEntity(entityId).Should().Be(true);
    }
}