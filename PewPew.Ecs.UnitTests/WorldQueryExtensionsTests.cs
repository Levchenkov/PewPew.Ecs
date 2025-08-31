using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class WorldQueryExtensionsTests
{
    private World _world;

    public WorldQueryExtensionsTests()
    {
        _world = new World();
        _world.InitComponent<Component1>();
        _world.InitComponent<Component2>();
        _world.InitComponent<Component3>();
    }

    [Fact]
    public void OneComponent()
    {
        var systemOne = new SystemOne();
        systemOne.World = _world;

        var entityId = _world.CreateEntityId();
        _world.GetComponents<Component1>().AddComponent(entityId).Value = 100500;
        systemOne.Update();
        _world.GetComponents<Component1>().GetComponent(entityId).Value.Should().Be(entityId.Index);
    }

    public class SystemOne : IQuery<Component1>
    {
        public World World;

        public void Update()
        {
            World.ExecuteQuery<SystemOne, Component1>(this);
        }

        public void Update(EntityId entityId, ref Component1 component1)
        {
            component1.Value = entityId.Index;
        }
    }

    [Fact]
    public void TwoComponents()
    {
        var systemOne = new SystemTwo();
        systemOne.World = _world;

        for (int i = 0; i < 100; i++)
        {
            var entityId = _world.CreateEntityId();
            if (entityId.Index % 2 == 0)
            {
                _world.GetComponents<Component1>().AddComponent(entityId).Value = entityId.Index;
            }

            if (entityId.Index % 3 == 0)
            {
                _world.GetComponents<Component2>().AddComponent(entityId).Value = entityId.Index;
            }
        }
        systemOne.Update();

        foreach (var entityId in _world.Entities)
        {
            if (_world.GetComponents<Component1>().TryGetComponent(entityId, out var componentRef1)
                && _world.GetComponents<Component2>().TryGetComponent(entityId, out var componentRef2))
            {
                componentRef1.Component.Value.Should().Be(entityId.Index * 2);
                componentRef2.Component.Value.Should().Be(entityId.Index * 3);
            }
        }
    }

    public class SystemTwo : IQuery<Component1, Component2>
    {
        public World World;

        public void Update()
        {
            World.ExecuteQuery<SystemTwo, Component1, Component2>(this);
        }

        public void Update(EntityId entityId, ref Component1 component1, ref Component2 component2)
        {
            component1.Value *= 2;
            component2.Value *= 3;
        }
    }

    [Fact]
    public void TwoComponentsNoEntity()
    {
        var systemOne = new SystemTwoNoEntity();
        systemOne.World = _world;

        for (int i = 0; i < 100; i++)
        {
            var entityId = _world.CreateEntityId();
            if (entityId.Index % 2 == 0)
            {
                _world.GetComponents<Component1>().AddComponent(entityId).Value = entityId.Index;
            }

            if (entityId.Index % 3 == 0)
            {
                _world.GetComponents<Component2>().AddComponent(entityId).Value = entityId.Index;
            }
        }
        systemOne.Update();

        foreach (var entityId in _world.Entities)
        {
            if (_world.GetComponents<Component1>().TryGetComponent(entityId, out var componentRef1)
                && _world.GetComponents<Component2>().TryGetComponent(entityId, out var componentRef2))
            {
                componentRef1.Component.Value.Should().Be(entityId.Index * 2);
                componentRef2.Component.Value.Should().Be(entityId.Index * 3);
            }
        }
    }

    public class SystemTwoNoEntity : IQueryWithoutId<Component1, Component2>
    {
        public World World;

        public void Update()
        {
            World.ExecuteQueryWithoutId<SystemTwoNoEntity, Component1, Component2>(this);
        }

        public void Update(ref Component1 component1, ref Component2 component2)
        {
            component1.Value *= 2;
            component2.Value *= 3;
        }
    }

    [Fact]
    public void ThreeComponents()
    {
        var systemOne = new SystemThree();
        systemOne.World = _world;

        for (int i = 0; i < 1000; i++)
        {
            var entityId = _world.CreateEntityId();
            if (entityId.Index % 2 == 0)
            {
                _world.GetComponents<Component1>().AddComponent(entityId).Value = entityId.Index;
            }

            if (entityId.Index % 3 == 0)
            {
                _world.GetComponents<Component2>().AddComponent(entityId).Value = entityId.Index;
            }

            if (entityId.Index % 5 == 0)
            {
                _world.GetComponents<Component3>().AddComponent(entityId).Value = entityId.Index;
            }
        }
        systemOne.Update();

        foreach (var entityId in _world.Entities)
        {
            if (_world.GetComponents<Component1>().TryGetComponent(entityId, out var componentRef1)
                && _world.GetComponents<Component2>().TryGetComponent(entityId, out var componentRef2)
                && _world.GetComponents<Component3>().TryGetComponent(entityId, out var componentRef3))
            {
                componentRef1.Component.Value.Should().Be(entityId.Index * 2);
                componentRef2.Component.Value.Should().Be(entityId.Index * 3);
                componentRef3.Component.Value.Should().Be(entityId.Index * 4);
            }
        }
    }

    public class SystemThree : IQuery<Component1, Component2, Component3>
    {
        public World World;

        public void Update()
        {
            World.ExecuteQuery<SystemThree, Component1, Component2, Component3>(this);
        }

        public void Update(EntityId entityId, ref Component1 component1, ref Component2 component2, ref Component3 component3)
        {
            component1.Value *= 2;
            component2.Value *= 3;
            component3.Value *= 4;
        }
    }

    [Fact]
    public void ExecuteQuery_Filter_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitTag<Tag1>();
        world.InitComponent<Component2>();

        for (int i = 0; i < world.EntityCapacity; i++)
        {
            var entityId = world.CreateEntityId();

            world.AddTag<Tag1>(entityId);
            world.AddComponent<Component2>(entityId).Value = entityId.Index;
        }

        world.ExecuteQueryWithTag<QueryWithTag, Component2, Tag1>(default);

        foreach (var entityId in world.Entities)
        {
            world.HasTag<Tag1>(entityId).Should().Be(true);
            world.GetComponent<Component2>(entityId).Value.Should().Be(entityId.Index * 2);
        }
    }

    private struct QueryWithTag : IQueryWithTag<Component2, Tag1>
    {
        public void Update(EntityId entityId, ref Component2 component2)
        {
            component2.Value *= 2;
        }
    }

    [Fact]
    public void ExecuteQueryAction_Filter_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitTag<Tag1>();
        world.InitComponent<Component2>();

        for (int i = 0; i < world.EntityCapacity; i++)
        {
            var entityId = world.CreateEntityId();

            world.AddTag<Tag1>(entityId);
            world.AddComponent<Component2>(entityId).Value = entityId.Index;
        }

        world.ExecuteQueryWithTag<Component2, Tag1>((EntityId _, ref Component2 component2) => component2.Value *= 2);

        foreach (var entityId in world.Entities)
        {
            world.HasTag<Tag1>(entityId).Should().Be(true);
            world.GetComponent<Component2>(entityId).Value.Should().Be(entityId.Index * 2);
        }
    }
}