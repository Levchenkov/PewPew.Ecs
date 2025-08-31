using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class HeavyFilterDefinitionTests
{
    [Fact]
    public void Compile_ComponentsAreNotInited_ShouldBeOk()
    {
        var filterDefinition = new HeavyFilterDefinition<Component1, Component2>();

        filterDefinition.GlobalComponentIndexes.Should().NotBeEmpty();
        filterDefinition.GlobalComponentIndexes[0].Should().Be(ComponentMetadata<Component1>.GlobalIndex);
        filterDefinition.GlobalComponentIndexes[1].Should().Be(ComponentMetadata<Component2>.GlobalIndex);

    }

    [Fact]
    public void With_TwiceTheSameComponent_ExceptionExpected()
    {
        Action action = () =>
        {
            var filterDefinition = new HeavyFilterDefinition<Component1, Component1>();
        };

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_ComponentsAreInited_ShouldBeOk()
    {
        ComponentMetadata<Component1>.InitComponentMetadata();
        ComponentMetadata<Component2>.InitComponentMetadata();

        var filterDefinition = new HeavyFilterDefinition<Component2, Component1>();

        filterDefinition.GlobalComponentIndexes[0].Should().Be(ComponentMetadata<Component1>.GlobalIndex);
        filterDefinition.GlobalComponentIndexes[1].Should().Be(ComponentMetadata<Component2>.GlobalIndex);
    }

    [Fact]
    public void CompareFilters_WithComponents_ShouldBeOk()
    {
        var filterDefinition1 = new HeavyFilterDefinition<Component1, Component2>();
        var filterDefinition2 = new HeavyFilterDefinition<Component2, Component1>();

        filterDefinition1.GetHashCode().Should().Be(filterDefinition2.GetHashCode());
        filterDefinition1.Equals(filterDefinition2).Should().BeTrue();
    }

    [Fact]
    public void CreateHeavyFilters_FetchTwice_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filterDefinition = new HeavyFilterDefinition<Component1, Component2>();

        var filter1 = world.FetchHeavyFilter(filterDefinition);
        var filter2 = world.FetchHeavyFilter(filterDefinition);

        filter1.Id.Should().Be(filter2.Id);
        filter1.Entities.Length.Should().Be(filter2.Entities.Length);
        for (var index = 0; index < filter1.Entities.Length; index++)
        {
            var entity1 = filter1.Entities[index];
            var entity2 = filter2.Entities[index];
            entity1.Should().Be(entity2);
        }
    }

    [Fact]
    public void CreateHeavyFilters_FetchTwiceNewFilterDef_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filterDefinition1 = new HeavyFilterDefinition<Component1, Component2>();

        var filter1 = world.FetchHeavyFilter(filterDefinition1);

        var filterDefinition2 = new HeavyFilterDefinition<Component1, Component2>();

        var filter2 = world.FetchHeavyFilter(filterDefinition2);

        filter1.Id.Should().Be(filter2.Id);
        filter1.Entities.Length.Should().Be(filter2.Entities.Length);
        for (var index = 0; index < filter1.Entities.Length; index++)
        {
            var entity1 = filter1.Entities[index];
            var entity2 = filter2.Entities[index];
            entity1.Should().Be(entity2);
        }

        filter2 = world.FetchHeavyFilter(filterDefinition2);

        filter1.Id.Should().Be(filter2.Id);
        filter1.Entities.Length.Should().Be(filter2.Entities.Length);
        for (var index = 0; index < filter1.Entities.Length; index++)
        {
            var entity1 = filter1.Entities[index];
            var entity2 = filter2.Entities[index];
            entity1.Should().Be(entity2);
        }
    }

    [Fact]
    public void CreateHeavyFilters_FetchTwiceSwapComponents_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filterDefinition12 = new HeavyFilterDefinition<Component1, Component2>();

        var filter1 = world.FetchHeavyFilter(filterDefinition12);

        var filterDefinition21 = new HeavyFilterDefinition<Component2, Component1>();

        var action = () =>
        {
            world.FetchHeavyFilter(filterDefinition21);
        };
        action.Should().Throw<NotSupportedException>(); // todo: only supported hardcoded order! fix
    }

    [Fact]
    public void CreateFilters_FetchFromAnotherWorld_ShouldBeOk()
    {
        #if RELEASE
        return;
        #endif

        var world1 = WorldFactory.Shared.CreateIndexedWorld();
        world1.InitComponent<Component1>();
        world1.InitComponent<Component2>();

        var filterDefinition = new HeavyFilterDefinition<Component1, Component2>();

        var filter = world1.FetchHeavyFilter(filterDefinition);

        var world2 = WorldFactory.Shared.CreateIndexedWorld();
        world2.InitComponent<Component1>();
        world2.InitComponent<Component2>();

        var action = () =>
        {
            world2.FetchHeavyFilter(filterDefinition);
        };
        action.Should().Throw<Exception>();
    }

}