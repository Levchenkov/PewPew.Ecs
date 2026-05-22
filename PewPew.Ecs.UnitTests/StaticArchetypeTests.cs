using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid;

namespace PewPew.Ecs.UnitTests;

public class StaticArchetypeTests
{
    [Fact]
    public void StaticArchetype_MoveEntityToArchetypeWithSameComponent_ComponentShouldBeMoved()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();

        world.InitStaticArchetype<Component1, Component2>();
        world.InitStaticArchetype<Component2, Component3>();

        var archetype12 = world.GetStaticArchetype<Component1, Component2>();
        var archetype23 = world.GetStaticArchetype<Component2, Component3>();

        var entityId = world.CreateEntityId();
        archetype12.Add(entityId).Component2.Value = 42;

        archetype12.Has(entityId).Should().BeTrue();
        archetype23.Has(entityId).Should().BeFalse();

        archetype12.MoveEntityTo(entityId, archetype23);

        archetype12.Has(entityId).Should().BeFalse();
        archetype23.Has(entityId).Should().BeTrue();
        archetype23.Get(entityId).Component1.Value.Should().Be(42);
    }

    [Fact]
    public void StaticArchetype_MoveEntityToArchetypeWithoutSameComponent_EntityShouldBeMoved()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();

        world.InitStaticArchetype<Component1, Component2>();
        world.InitStaticArchetype<Component3, Component4>();

        var archetype12 = world.GetStaticArchetype<Component1, Component2>();
        var archetype34 = world.GetStaticArchetype<Component3, Component4>();

        var entityId = world.CreateEntityId();
        archetype12.Add(entityId);

        archetype12.Has(entityId).Should().BeTrue();
        archetype34.Has(entityId).Should().BeFalse();

        archetype12.MoveEntityTo(entityId, archetype34);

        archetype12.Has(entityId).Should().BeFalse();
        archetype34.Has(entityId).Should().BeTrue();
    }

    [Fact]
    public void StaticArchetype_MoveEntityToWorld_ComponentsShouldBeMoved()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();

        world.InitStaticArchetype<Component1, Component2>();

        var archetype12 = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        var component = archetype12.Add(entityId);
        component.Component1.Value = 42;
        component.Component2.Value = 69;

        archetype12.Has(entityId).Should().BeTrue();

        world.TryGetComponent<Component1>(entityId, out var componentRef1).Should().BeTrue();
        componentRef1.Component.Value.Should().Be(42);
        world.TryGetComponent<Component2>(entityId, out var componentRef2).Should().BeTrue();
        componentRef2.Component.Value.Should().Be(69);

        archetype12.MoveEntityToWorld(entityId);

        archetype12.Has(entityId).Should().BeFalse();

        world.TryGetComponent<Component1>(entityId, out componentRef1).Should().BeTrue();
        componentRef1.Component.Value.Should().Be(42);
        world.TryGetComponent<Component2>(entityId, out componentRef2).Should().BeTrue();
        componentRef2.Component.Value.Should().Be(69);
    }

    [Fact]
    public void StaticArchetype_MoveEntityToArchetypeWithTag_ComponentShouldBeMoved()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();

        world.InitStaticArchetypeWithTag<Component1, Component2, Tag3>();
        world.InitStaticArchetypeWithTag<Component1, Component2, Tag4>();

        StaticArchetype<BitMask64, Component1, Component2> archetype1 = world.GetStaticArchetypeWithTag<Component1, Component2, Tag3>();
        StaticArchetype<BitMask64, Component1, Component2> archetype2 = world.GetStaticArchetypeWithTag<Component1, Component2, Tag4>();

        var entityId = world.CreateEntityId();
        var tuple = archetype1.Add(entityId);
        tuple.Component1.Value = 42;
        tuple.Component2.Value = 69;

        archetype1.Has(entityId).Should().BeTrue();
        archetype2.Has(entityId).Should().BeFalse();

        archetype1.MoveEntityTo(entityId, archetype2);

        archetype1.Has(entityId).Should().BeFalse();
        archetype2.Has(entityId).Should().BeTrue();
        var tuple2 = archetype2.Get(entityId);
        tuple2.Component1.Value.Should().Be(42);
        tuple2.Component2.Value.Should().Be(69);
    }

    [Fact]
    public void StaticArchetype_CrossArity_MoveEntityTo_SharedComponentsCopied_NewComponentsDefault()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();

        world.InitStaticArchetype<Component1, Component2>();
        world.InitStaticArchetype<Component1, Component2, Component3>();

        var archetype2 = world.GetStaticArchetype<Component1, Component2>();
        var archetype3 = world.GetStaticArchetype<Component1, Component2, Component3>();

        var entityId = world.CreateEntityId();
        var added = archetype2.Add(entityId);
        added.Component1.Value = 10;
        added.Component2.Value = 20;

        archetype2.Has(entityId).Should().BeTrue();
        archetype3.Has(entityId).Should().BeFalse();

        archetype2.MoveEntityTo(entityId, archetype3);

        archetype2.Has(entityId).Should().BeFalse();
        archetype3.Has(entityId).Should().BeTrue();

        var result = archetype3.Get(entityId);
        result.Component1.Value.Should().Be(10);
        result.Component2.Value.Should().Be(20);
        result.Component3.Value.Should().Be(default(int)); // new component, default value
    }

    [Fact]
    public void StaticArchetype_CrossArity_MoveEntityTo_T3ToT2_SharedComponentCopied()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();

        world.InitStaticArchetype<Component1, Component2, Component3>();
        world.InitStaticArchetype<Component2, Component3>();

        var archetype3 = world.GetStaticArchetype<Component1, Component2, Component3>();
        var archetype2 = world.GetStaticArchetype<Component2, Component3>();

        var entityId = world.CreateEntityId();
        var added = archetype3.Add(entityId);
        added.Component1.Value = 5;
        added.Component2.Value = 15;
        added.Component3.Value = 25;

        archetype3.Has(entityId).Should().BeTrue();
        archetype2.Has(entityId).Should().BeFalse();

        archetype3.MoveEntityTo(entityId, archetype2);

        archetype3.Has(entityId).Should().BeFalse();
        archetype2.Has(entityId).Should().BeTrue();

        var result = archetype2.Get(entityId);
        result.Component1.Value.Should().Be(15); // Component2 from source maps to Component1 in target
        result.Component2.Value.Should().Be(25); // Component3 from source maps to Component2 in target
    }
}