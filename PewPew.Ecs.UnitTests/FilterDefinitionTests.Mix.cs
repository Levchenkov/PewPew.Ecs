using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class MixFilterDefinitionTests
{

    [Fact]
    public void Compile_MixMoreTagsThenAllowed_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld(settings => settings.MaxAllowedUniqueComponentsCount = 2);

        var filterDefinition = new FilterDefinition();
        filterDefinition.WithTag<Tag1>();
        filterDefinition.WithTag<Tag2>();
        filterDefinition.With<Component3>();

        Action prepareAction = () => world.GetFilter(filterDefinition);

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_Mix2MoreTagsThenAllowed_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld(settings => settings.MaxAllowedUniqueComponentsCount = 2);

        var filterDefinition = new FilterDefinition();
        filterDefinition.With<Component1>();
        filterDefinition.With<Component2>();
        filterDefinition.WithTag<Tag3>();

        Action prepareAction = () => world.GetFilter(filterDefinition);

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_Mix3MoreTagsThenAllowed_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld(settings => settings.MaxAllowedUniqueComponentsCount = 2);

        var filterDefinition = new FilterDefinition();
        filterDefinition.With<Component1>();
        filterDefinition.WithTag<Tag2>();
        filterDefinition.With<Component3>();

        Action prepareAction = () => world.GetFilter(filterDefinition);

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_Mix4MoreTagsThenAllowed_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld(settings => settings.MaxAllowedUniqueComponentsCount = 2);

        var filterDefinition = new FilterDefinition();
        filterDefinition.WithTag<Tag1>();
        filterDefinition.With<Component2>();
        filterDefinition.WithTag<Tag3>();

        Action prepareAction = () => world.GetFilter(filterDefinition);

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_WithTagsAndComponents_ShouldBeOk()
    {
        var filterDefinition = new FilterDefinition();
        filterDefinition.With<Component1>();
        filterDefinition.With<Component2>();
        filterDefinition.WithTag<Tag1>();
        filterDefinition.WithTag<Tag2>();

        filterDefinition.IsCompiled.Should().BeFalse();
        filterDefinition.GlobalComponentIndexes.Count.Should().Be(4);

        filterDefinition.Compile();

        filterDefinition.IsCompiled.Should().BeTrue();
    }

    [Fact]
    public void CompareFilters_WithTagAndComponent_ShouldBeOk()
    {
        var filterDefinition1 = new FilterDefinition();
        filterDefinition1.WithTag<Tag1>();
        filterDefinition1.With<Component1>();

        var filterDefinition2 = new FilterDefinition();
        filterDefinition2.With<Component1>();
        filterDefinition2.WithTag<Tag1>();

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
        world.InitTag<Tag1>();
        world.InitComponent<Component1>();

        var filterDefinition = new FilterDefinition().WithTag<Tag1>().With<Component1>();

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
        world.InitTag<Tag1>();
        world.InitComponent<Component1>();

        var filterDefinition1 = new FilterDefinition().WithTag<Tag1>().With<Component1>();

        var filter1 = world.GetFilter(filterDefinition1);

        var filterDefinition2 = new FilterDefinition().WithTag<Tag1>().With<Component1>();

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
        world.InitTag<Tag1>();
        world.InitComponent<Component1>();

        var filterDefinition12 = new FilterDefinition().WithTag<Tag1>().With<Component1>();

        var filter1 = world.GetFilter(filterDefinition12);

        var filterDefinition21 = new FilterDefinition().With<Component1>().WithTag<Tag1>();

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

    [DebugOnlyFact]
    public void CreateFilters_FetchFromAnotherWorld_ShouldBeOk()
    {
        var world1 = WorldFactory.Shared.CreateIndexedWorld();
        world1.InitTag<Tag1>();
        world1.InitComponent<Component1>();

        var filterDefinition = new FilterDefinition().WithTag<Tag1>().With<Component1>();

        var filter = world1.GetFilter(filterDefinition);

        var world2 = WorldFactory.Shared.CreateIndexedWorld();
        world2.InitTag<Tag1>();
        world2.InitComponent<Component1>();

        var action = () =>
        {
            world2.GetFilter(filterDefinition);
        };
        action.Should().Throw<Exception>();
    }

}