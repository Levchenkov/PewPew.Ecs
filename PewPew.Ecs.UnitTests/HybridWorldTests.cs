using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid;

namespace PewPew.Ecs.UnitTests;

public class HybridWorldTests
{
    [Fact]
    public void StaticArchetype_InitStaticArchetypeMultipleTimes_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var action = () => world.InitStaticArchetype<Component1, Component2>();
        action.Should().NotThrow();

        action = () => world.InitStaticArchetype<Component2, Component1>();
        action.Should().NotThrow();
    }

    [Fact]
    public void StaticArchetype_InitStaticArchetypeWithTagMultipleTimes_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var action = () => world.InitStaticArchetype<Component1, Component2>(); // the same
        action.Should().NotThrow();

        action = () => world.InitStaticArchetype<Component2, Component1>();
        action.Should().NotThrow();

        action = () => world.InitStaticArchetypeWithTag<Component1, Component2, Tag1>();
        action.Should().NotThrow();

        action = () => world.InitStaticArchetypeWithTag<Component1, Component2, Tag1>(); // the same
        action.Should().NotThrow();

        action = () => world.InitStaticArchetypeWithTag<Component1, Component2, Tag2>();
        action.Should().NotThrow();

        action = () => world.InitStaticArchetypeWithTag<Component2, Component1, Tag1>();
        action.Should().NotThrow();

        action = () => world.InitStaticArchetypeWithTag<Component2, Component1, Tag2>();
        action.Should().NotThrow();
    }

    [Fact]
    public void GetStaticArchetypeWithTag_StaticArchetypeWithTagIsNotInited_ExceptionShouldBeThrown()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();

        Action action = () => world.GetStaticArchetypeWithTag<Component1, Component2, Tag1>();
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void GetStaticArchetypeWithTag_StaticArchetypeWithTag_ShouldBeDifferentIds()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();
        world.InitStaticArchetypeWithTag<Component1, Component2, Tag1>();
        world.InitStaticArchetypeWithTag<Component1, Component2, Tag2>();

        StaticArchetype<BitMask64, Component1, Component2> staticArchetype12 = world.GetStaticArchetype<Component1, Component2>();

        StaticArchetype<BitMask64, Component1, Component2> staticArchetype121 = world.GetStaticArchetypeWithTag<Component1, Component2, Tag1>();

        staticArchetype121.Id.Should().NotBe(staticArchetype12.Id);

        StaticArchetype<BitMask64, Component1, Component2> staticArchetype122 = world.GetStaticArchetypeWithTag<Component1, Component2, Tag2>();

        staticArchetype122.Id.Should().NotBe(staticArchetype121.Id);
        staticArchetype122.Id.Should().NotBe(staticArchetype12.Id);
    }

    [Fact]
    public void GetStaticArchetypeWithTag_PermutateGenericParameters_ShouldGetTheSameArchetype()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetypeWithTag<Component1, Component2, Tag1>();
        world.InitStaticArchetypeWithTag<Component1, Component2, Tag2>();

        StaticArchetype<BitMask64, Component1, Component2> staticArchetype121 = world.GetStaticArchetypeWithTag<Component1, Component2, Tag1>();
        StaticArchetype<BitMask64, Component2, Component1> staticArchetype211 = world.GetStaticArchetypeWithTag<Component2, Component1, Tag1>();

        staticArchetype121.Id.Should().Be(staticArchetype211.Id);

        StaticArchetype<BitMask64, Component1, Component2> staticArchetype122 = world.GetStaticArchetypeWithTag<Component1, Component2, Tag2>();
        StaticArchetype<BitMask64, Component2, Component1> staticArchetype212 = world.GetStaticArchetypeWithTag<Component2, Component1, Tag2>();

        staticArchetype122.Id.Should().Be(staticArchetype212.Id);
        staticArchetype122.Id.Should().NotBe(staticArchetype121.Id);
    }

    [Fact]
    public void StaticArchetype_AddEntityWithComponents_ArchetypeShouldHaveEntity()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        archetype.Has(entityId).Should().BeTrue();
    }

    [Fact]
    public void StaticArchetype_AddEntityWithoutComponents_ArchetypeShouldHaveEntity()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId);

        archetype.Has(entityId).Should().BeTrue();
    }

    [Fact]
    public void StaticArchetype_AddEntityTwice_ComponentShouldBeUpdated()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 41 }, new Component2 { Value = 68 });
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        archetype.Has(entityId).Should().BeTrue();
        archetype.GetComponent1(entityId).Value.Should().Be(42);
        archetype.GetComponent2(entityId).Value.Should().Be(69);
    }

    [Fact]
    public void StaticArchetype_AddEntityTwiceAsTuple_ComponentShouldBeUpdated()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        var tuple1 = archetype.Add(entityId);
        var tuple2 = archetype.Add(entityId);

        tuple1.Component1.Value = 42;
        tuple2.Component1.Value.Should().Be(42);

        tuple2.Component1.Value = 69;
        tuple1.Component1.Value.Should().Be(69);

        archetype.GetComponent1(entityId).Value.Should().Be(69);
    }

    [Fact]
    public void StaticArchetype_AddEntityTwiceMixed_ComponentShouldBeUpdated()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        var tuple1 = archetype.Add(entityId);

        tuple1.Component1.Value = 41;
        tuple1.Component2.Value = 68;

        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        archetype.GetComponent1(entityId).Value.Should().Be(42);
        archetype.GetComponent2(entityId).Value.Should().Be(69);

        var tuple2 = archetype.Get(entityId);

        tuple2.Component1.Value.Should().Be(tuple1.Component1.Value);
        tuple2.Component2.Value.Should().Be(tuple1.Component2.Value);
    }

    [Fact]
    public void AddComponent_AddComponentToWorldButNotInStaticArchetype_ArchetypeShouldNotHaveEntity()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        world.AddComponent<Component1>(entityId);
        world.AddComponent<Component2>(entityId);

        world.HasComponent<Component1>(entityId).Should().BeTrue();
        world.HasComponent<Component2>(entityId).Should().BeTrue();

        archetype.Has(entityId).Should().BeFalse();
    }

    [Fact]
    public void AddComponent_AddComponentToStaticArchetype_ArchetypeShouldHaveEntity()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        archetype.Has(entityId).Should().BeTrue();

        world.HasComponent<Component1>(entityId).Should().BeTrue();
        world.HasComponent<Component2>(entityId).Should().BeTrue();
    }

    [Fact]
    public void AddComponent_AddComponentWorldAndToStaticArchetype_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        world.AddComponent<Component1>(entityId).Value = 42;

        var action = () =>
        {
            var archetype = world.GetStaticArchetype<Component1, Component2>();
            archetype.Add(entityId, new Component1 { Value = 69 }, new Component2 { Value = 69 });
        };

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void AddComponent_AddComponentToStaticArchetypeAndToWorld_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 69 }, new Component2 { Value = 69 });

        var action = () => world.AddComponent<Component1>(entityId);

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void AddComponent_AddToMoreThenOneStaticArchetypeWithIntersection_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();
        world.InitStaticArchetype<Component2, Component3>();

        var entityId = world.CreateEntityId();

        var archetype1 = world.GetStaticArchetype<Component1, Component2>();
        archetype1.Add(entityId, new Component1 { Value = 69 }, new Component2 { Value = 69 });

        var action = () =>
        {
            var archetype2 = world.GetStaticArchetype<Component2, Component3>(); // has intersection
            archetype2.Add(entityId, new Component2 { Value = 69 }, new Component3 { Value = 69 });
        };

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void AddComponent_AddToMoreThenOneStaticArchetypeNoIntersection_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();
        world.InitStaticArchetype<Component3, Component4>();

        var entityId = world.CreateEntityId();

        var archetype1 = world.GetStaticArchetype<Component1, Component2>();
        archetype1.Add(entityId, new Component1 { Value = 69 }, new Component2 { Value = 69 });

        var action = () =>
        {
            var archetype2 = world.GetStaticArchetype<Component3, Component4>(); // no intersection
            archetype2.Add(entityId, new Component3 { Value = 69 }, new Component4 { Value = 69 });
        };

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void AddComponent_AddMultipleEntities_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();
        world.InitStaticArchetype<Component3, Component4>();

        var entityId1 = world.CreateEntityId();
        var archetype1 = world.GetStaticArchetype<Component1, Component2>();
        archetype1.Add(entityId1, new Component1 { Value = 42 }, new Component2 { Value = 42 });

        var entityId2 = world.CreateEntityId();
        var archetype2 = world.GetStaticArchetype<Component3, Component4>();
        archetype2.Add(entityId2, new Component3 { Value = 69 }, new Component4 { Value = 69 });

        var entityId3 = world.CreateEntityId();

        world.AddComponent<Component1>(entityId3).Value = 100500;
        world.AddComponent<Component2>(entityId3).Value = 100500;
        world.AddComponent<Component3>(entityId3).Value = 100500;
        world.AddComponent<Component4>(entityId3).Value = 100500;

        archetype1.Count.Should().Be(1);
        archetype2.Count.Should().Be(1);

        archetype1.Has(entityId1).Should().Be(true);
        archetype1.Has(entityId2).Should().Be(false);
        archetype1.Has(entityId3).Should().Be(false);

        archetype2.Has(entityId1).Should().Be(false);
        archetype2.Has(entityId2).Should().Be(true);
        archetype2.Has(entityId3).Should().Be(false);

        archetype1.GetComponent<Component1>(entityId1).Value.Should().Be(42);
        archetype1.GetComponent<Component2>(entityId1).Value.Should().Be(42);
        archetype2.GetComponent<Component3>(entityId2).Value.Should().Be(69);
        archetype2.GetComponent<Component4>(entityId2).Value.Should().Be(69);

        archetype1.TryGetComponent<Component1>(entityId1, out var component1).Should().BeTrue();
        component1.Component.Value.Should().Be(42);

        archetype1.TryGetComponent<Component2>(entityId1, out var component2).Should().BeTrue();
        component2.Component.Value.Should().Be(42);

        archetype1.TryGetComponent<Component1>(entityId3, out component1).Should().BeFalse();
        archetype1.TryGetComponent<Component2>(entityId3, out component2).Should().BeFalse();

        archetype2.TryGetComponent<Component3>(entityId2, out var component3).Should().BeTrue();
        component3.Component.Value.Should().Be(69);

        archetype2.TryGetComponent<Component4>(entityId2, out var component4).Should().BeTrue();
        component4.Component.Value.Should().Be(69);

        archetype2.TryGetComponent<Component3>(entityId3, out component3).Should().BeFalse();
        archetype2.TryGetComponent<Component4>(entityId3, out component4).Should().BeFalse();
    }

    [Fact]
    public void HasComponent_ArchetypeHasComponent_ShouldBeTrue()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 69 }, new Component2 { Value = 69 });

        world.HasComponent<Component1>(entityId).Should().BeTrue();
        world.HasComponent<Component2>(entityId).Should().BeTrue();
    }

    [Fact]
    public void HasComponent_ArchetypeDoesNotHaveComponent_ShouldBeFalse()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 69 }, new Component2 { Value = 69 });

        world.HasComponent<Component3>(entityId).Should().BeFalse();
    }

    [Fact]
    public void HasComponent_WorldHasComponent_ShouldBeTrue()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 69 }, new Component2 { Value = 69 });

        var action = () => world.AddComponent<Component3>(entityId);
        action.Should().Throw<Exception>();

        world.HasComponent<Component3>(entityId).Should().BeFalse();
    }

    [Fact]
    public void HasComponent_WorldDoesNotHaveComponent_ShouldBeFalse()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();

        world.HasComponent<Component3>(entityId).Should().BeFalse();
    }

    [Fact]
    public void GetComponent_ArchetypeHasComponent_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        world.GetComponent<Component1>(entityId).Value.Should().Be(42);
        world.GetComponent<Component2>(entityId).Value.Should().Be(69);
    }

    [Fact]
    public void GetComponent_WorldHasComponent_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        world.AddComponent<Component1>(entityId).Value = 42;
        world.AddComponent<Component2>(entityId).Value = 69;

        world.GetComponent<Component1>(entityId).Value.Should().Be(42);
        world.GetComponent<Component2>(entityId).Value.Should().Be(69);
    }

    [Fact]
    public void GetComponent_ArchetypeDoesNotAndWorldHaveComponent_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        var action = () => world.AddComponent<Component3>(entityId).Value = 100500;
        action.Should().Throw<Exception>();

        var action2 = () => world.GetComponent<Component3>(entityId);
        action2.Should().Throw<Exception>();
    }

    [Fact]
    public void GetComponent_ArchetypeAndWorldDoNotHaveComponent_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        var action = () => world.GetComponent<Component3>(entityId);
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void TryGetComponent_ArchetypeHasComponent_ShouldBeTrue()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        world.TryGetComponent<Component1>(entityId, out var component1).Should().BeTrue();
        component1.Component.Value.Should().Be(42);

        world.TryGetComponent<Component2>(entityId, out var component2).Should().BeTrue();
        component2.Component.Value.Should().Be(69);
    }

    [Fact]
    public void TryGetComponent_WorldHasComponent_ShouldBeTrue()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        world.AddComponent<Component1>(entityId).Value = 42;
        world.AddComponent<Component2>(entityId).Value = 69;

        world.TryGetComponent<Component1>(entityId, out var component1).Should().BeTrue();
        component1.Component.Value.Should().Be(42);

        world.TryGetComponent<Component2>(entityId, out var component2).Should().BeTrue();
        component2.Component.Value.Should().Be(69);
    }

    [Fact]
    public void TryGetComponent_ArchetypeDoesNotAndWorldHaveComponent_ShouldBeTrue()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });
        var action = () => world.AddComponent<Component3>(entityId).Value = 100500;
        action.Should().Throw<Exception>();

        world.TryGetComponent<Component3>(entityId, out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetComponent_ArchetypeAndWorldDoNotHaveComponent_ShouldBeFalse()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        world.TryGetComponent<Component3>(entityId, out _).Should().BeFalse();
    }

    [Fact]
    public void ArchetypeDelete_EntityExists_EntityShouldBeDeleted()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var archetype = world.GetStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        archetype.Has(entityId).Should().BeTrue();
        world.HasComponent<Component1>(entityId).Should().BeTrue();
        world.HasComponent<Component2>(entityId).Should().BeTrue();

        archetype.Delete(entityId);

        archetype.Has(entityId).Should().BeFalse();
        world.HasComponent<Component1>(entityId).Should().BeFalse();
        world.HasComponent<Component2>(entityId).Should().BeFalse();
    }

    [Fact]
    public void ArchetypeDelete_EntityDoesNotExist_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();

        var archetype = world.GetStaticArchetype<Component1, Component2>();
        archetype.Has(entityId).Should().BeFalse();
        world.HasComponent<Component1>(entityId).Should().BeFalse();
        world.HasComponent<Component2>(entityId).Should().BeFalse();

        archetype.Delete(entityId);
    }

    [Fact]
    public void DeleteComponent_ArchetypeHasComponent_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();

        var archetype = world.GetStaticArchetype<Component1, Component2>();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        world.HasComponent<Component1>(entityId).Should().BeTrue();

        var action = () => world.DeleteComponent<Component1>(entityId);
        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void DeleteComponent_ArchetypeDoesNotHaveComponent_ShouldDelete()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();

        var archetype = world.GetStaticArchetype<Component1, Component2>();
        archetype.Add(entityId, new Component1 { Value = 42 }, new Component2 { Value = 69 });

        var action = () => world.AddComponent<Component3>(entityId);
        action.Should().Throw<Exception>();

        world.HasComponent<Component3>(entityId).Should().BeFalse();

        var action2 = () => world.DeleteComponent<Component3>(entityId);
        action2.Should().Throw<Exception>();

        world.HasComponent<Component3>(entityId).Should().BeFalse();
    }

    [Fact]
    public void DeleteComponent_NoArchetype_ShouldDelete()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();
        world.AddComponent<Component3>(entityId);

        world.HasComponent<Component3>(entityId).Should().BeTrue();

        world.DeleteComponent<Component3>(entityId);

        world.HasComponent<Component3>(entityId).Should().BeFalse();
    }

    [Fact]
    public void DeleteComponent_NoComponent_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitComponent<Component3>();
        world.InitStaticArchetype<Component1, Component2>();

        var entityId = world.CreateEntityId();

        world.HasComponent<Component1>(entityId).Should().BeFalse();

        world.DeleteComponent<Component3>(entityId);

        world.HasComponent<Component1>(entityId).Should().BeFalse();
    }

    [Fact]
    public void GetStaticArchetype_NotInitialized_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();

        var action = () =>
        {
            world.GetStaticArchetype<Component1, Component2>();
        };

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void GetStaticArchetype_PermutateGenericParameters_ShouldGetTheSameArchetype()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetype<Component1, Component2>();
        var entityId1 = world.CreateEntityId();

        var archetype1 = world.GetStaticArchetype<Component1, Component2>();

        var tuple1 = archetype1.Add(entityId1);
        tuple1.Component1.Value = 42;
        tuple1.Component2.Value = 69;

        var archetype2 = world.GetStaticArchetype<Component2, Component1>();

        archetype2.Id.Should().Be(archetype1.Id);
        archetype2.Count.Should().Be(1);
        archetype2.Has(entityId1).Should().BeTrue();
        archetype2.Entities[0].Should().Be(entityId1);
        var tuple2 = archetype2.Get(entityId1);
        tuple2.Component1.Value.Should().Be(69);
        tuple2.Component2.Value.Should().Be(42);

        // change tuple
        tuple2.Component2.Value = 100500;
        tuple1.Component1.Value.Should().Be(100500);

        var entityId2 = world.CreateEntityId();
        var tuple3 = archetype2.Add(entityId2);
        tuple3.Component1.Value = 42;
        tuple3.Component2.Value = 69;

        var tuple4 = archetype1.Get(entityId2);
        tuple4.Component1.Value.Should().Be(69);
        tuple4.Component2.Value.Should().Be(42);

        archetype1.Count.Should().Be(2);
        archetype2.Count.Should().Be(2);
    }

    [Fact]
    public void GetStaticArchetypeWithTag_PermutateGenericParameters_ShouldGetTheSameArchetype2()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetypeWithTag<Component1, Component2, Tag3>();
        world.InitStaticArchetypeWithTag<Component1, Component2, Tag4>();
        var entityId1 = world.CreateEntityId();

        StaticArchetype<BitMask64, Component1, Component2> archetype1 = world.GetStaticArchetypeWithTag<Component1, Component2, Tag3>();

        var tuple1 = archetype1.Add(entityId1);
        tuple1.Component1.Value = 42;
        tuple1.Component2.Value = 69;

        StaticArchetype<BitMask64, Component2, Component1> archetype2 = world.GetStaticArchetypeWithTag<Component2, Component1, Tag3>();

        archetype2.Id.Should().Be(archetype1.Id);
        archetype2.Count.Should().Be(1);
        archetype2.Has(entityId1).Should().BeTrue();
        archetype2.Entities[0].Should().Be(entityId1);

        var tuple2 = archetype2.Get(entityId1);
        tuple2.Component1.Value.Should().Be(69);
        tuple2.Component2.Value.Should().Be(42);

        // change tuple
        tuple2.Component2.Value = 100500;
        tuple1.Component1.Value.Should().Be(100500);

        var entityId2 = world.CreateEntityId();
        var tuple3 = archetype2.Add(entityId2);
        tuple3.Component1.Value = 42;
        tuple3.Component2.Value = 69;

        var tuple4 = archetype1.Get(entityId2);
        tuple4.Component1.Value.Should().Be(69);
        tuple4.Component2.Value.Should().Be(42);

        archetype1.Count.Should().Be(2);
        archetype2.Count.Should().Be(2);

        StaticArchetype<BitMask64, Component1, Component2> archetype3 = world.GetStaticArchetypeWithTag<Component1, Component2, Tag4>();
        var entityId3 = world.CreateEntityId();
        archetype3.Add(entityId3);

        archetype3.Id.Should().NotBe(archetype1.Id);
        archetype3.Count.Should().Be(1);
        archetype3.Has(entityId1).Should().BeFalse();
        archetype3.Has(entityId2).Should().BeFalse();
        archetype3.Has(entityId3).Should().BeTrue();

        var filterDefinition = new FilterDefinition().With<Component1>().With<Component2>().WithTag<Tag3>();
        var hybridFilter = world.GetFilter(filterDefinition);
        hybridFilter.HasEntity(entityId1).Should().BeTrue();
        hybridFilter.HasEntity(entityId2).Should().BeTrue();
        hybridFilter.HasEntity(entityId3).Should().BeFalse();

        var filterDefinition2 = new FilterDefinition().With<Component1>().With<Component2>().WithTag<Tag4>();
        var hybridFilter2 = world.GetFilter(filterDefinition2);
        hybridFilter2.HasEntity(entityId1).Should().BeFalse();
        hybridFilter2.HasEntity(entityId2).Should().BeFalse();
        hybridFilter2.HasEntity(entityId3).Should().BeTrue();
    }

    // todo: add GetStaticArchetype_PermutateGenericParameters_ShouldGetTheSameArchetype for StaticArchetype<T1, T2, T3>
}