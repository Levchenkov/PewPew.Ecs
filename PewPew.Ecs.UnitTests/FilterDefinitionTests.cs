using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class FilterDefinitionTests
{
    [Fact]
    public void Compile_WithoutComponents_ExceptionExpected()
    {
        var filterDefinition = new FilterDefinition();
        filterDefinition.GlobalComponentIndexes.Should().BeEmpty();

        Action prepareAction = () => filterDefinition.Compile();

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_SingleComponent_ExceptionExpected()
    {
        var filterDefinition = new FilterDefinition();
        filterDefinition.With<Component1>();

        Action prepareAction = () => filterDefinition.Compile();

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void With_TwiceTheSameComponent_ExceptionExpected()
    {
        var filterDefinition = new FilterDefinition();
        filterDefinition.With<Component1>();

        Action withAction = () => filterDefinition.With<Component1>();

        withAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_MoreComponentsThenAllowed_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld(settings => settings.MaxAllowedUniqueComponentsCount = 2);

        var filterDefinition = new FilterDefinition();
        filterDefinition.With<Component1>();
        filterDefinition.With<Component2>();
        filterDefinition.With<Component3>();

        Action prepareAction = () => world.GetFilter(filterDefinition);

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_WithComponents_ShouldBeOk()
    {
        ComponentMetadata<Component1>.InitComponentMetadata();
        ComponentMetadata<Component2>.InitComponentMetadata();

        var filterDefinition = new FilterDefinition();
        filterDefinition.With<Component2>();
        filterDefinition.With<Component1>();

        filterDefinition.IsCompiled.Should().BeFalse();
        filterDefinition.GlobalComponentIndexes.Count.Should().Be(2);
        filterDefinition.GlobalComponentIndexes[0].Should().Be(ComponentMetadata<Component2>.GlobalIndex);
        filterDefinition.GlobalComponentIndexes[1].Should().Be(ComponentMetadata<Component1>.GlobalIndex);

        filterDefinition.Compile();

        filterDefinition.IsCompiled.Should().BeTrue();
        filterDefinition.GlobalComponentIndexes[0].Should().Be(ComponentMetadata<Component1>.GlobalIndex);
        filterDefinition.GlobalComponentIndexes[1].Should().Be(ComponentMetadata<Component2>.GlobalIndex);
    }

    [Fact]
    public void CompareFilters_WithComponents_ShouldBeOk()
    {
        var filterDefinition1 = new FilterDefinition();
        filterDefinition1.With<Component1>();
        filterDefinition1.With<Component2>();

        var filterDefinition2 = new FilterDefinition();
        filterDefinition2.With<Component2>();
        filterDefinition2.With<Component1>();

        filterDefinition1.GetHashCode().Should().NotBe(filterDefinition2.GetHashCode());
        filterDefinition1.Equals(filterDefinition2).Should().BeFalse();

        filterDefinition1.Compile();
        filterDefinition2.Compile();

        filterDefinition1.GetHashCode().Should().Be(filterDefinition2.GetHashCode());
        filterDefinition1.Equals(filterDefinition2).Should().BeTrue();
    }

    [Fact]
    public void CreateFilters_FetchTwice_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filterDefinition = new FilterDefinition().With<Component1>().With<Component2>();

        var filter1 = world.GetFilter(filterDefinition);

        var filter2 = world.GetFilter(filterDefinition);

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
    public void CreateFilters_FetchTwiceNewFilterDef_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filterDefinition1 = new FilterDefinition().With<Component1>().With<Component2>();

        var filter1 = world.GetFilter(filterDefinition1);

        var filterDefinition2 = new FilterDefinition().With<Component1>().With<Component2>();

        var filter2 = world.GetFilter(filterDefinition2);

        filter1.Id.Should().Be(filter2.Id);
        filter1.Entities.Length.Should().Be(filter2.Entities.Length);
        for (var index = 0; index < filter1.Entities.Length; index++)
        {
            var entity1 = filter1.Entities[index];
            var entity2 = filter2.Entities[index];
            entity1.Should().Be(entity2);
        }

        filter2 = world.GetFilter(filterDefinition2);

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
    public void CreateFilters_FetchTwiceSwappedComponents_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitComponent<Component2>();

        var filterDefinition12 = new FilterDefinition().With<Component1>().With<Component2>();

        var filter1 = world.GetFilter(filterDefinition12);

        var filterDefinition21 = new FilterDefinition().With<Component2>().With<Component1>();

        var filter2 = world.GetFilter(filterDefinition21);

        filter1.Id.Should().Be(filter2.Id);
        filter1.Entities.Length.Should().Be(filter2.Entities.Length);
        for (var index = 0; index < filter1.Entities.Length; index++)
        {
            var entity1 = filter1.Entities[index];
            var entity2 = filter2.Entities[index];
            entity1.Should().Be(entity2);
        }

        filter2 = world.GetFilter(filterDefinition21);

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
    public void CreateFilters_FetchFromAnotherWorld_ShouldBeOk()
    {
        #if RELEASE
        return;
        #endif

        var world1 = WorldFactory.Shared.CreateIndexedWorld();
        world1.InitComponent<Component1>();
        world1.InitComponent<Component2>();

        var filterDefinition = new FilterDefinition().With<Component1>().With<Component2>();

        var filter = world1.GetFilter(filterDefinition);

        var world2 = WorldFactory.Shared.CreateIndexedWorld();
        world2.InitComponent<Component1>();
        world2.InitComponent<Component2>();

        var action = () =>
        {
            world2.GetFilter(filterDefinition);
        };
        action.Should().Throw<Exception>();
    }

}