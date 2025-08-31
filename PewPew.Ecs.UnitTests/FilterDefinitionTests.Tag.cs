using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters;

namespace PewPew.Ecs.UnitTests;

public class TagFilterDefinitionTests
{
    [Fact]
    public void Compile_WithoutTags_ExceptionExpected()
    {
        var filterDefinition = new FilterDefinition();
        filterDefinition.GlobalComponentIndexes.Should().BeEmpty();

        Action prepareAction = () => filterDefinition.Compile();

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_SingleTag_ExceptionExpected()
    {
        var filterDefinition = new FilterDefinition();
        filterDefinition.WithTag<Tag1>();

        Action prepareAction = () => filterDefinition.Compile();

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void With_TwiceTheSameTag_ExceptionExpected()
    {
        var filterDefinition = new FilterDefinition();
        filterDefinition.WithTag<Tag1>();

        Action withAction = () => filterDefinition.WithTag<Tag1>();

        withAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_MoreTagsThenAllowed_ExceptionExpected()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld(settings => settings.MaxAllowedUniqueComponentsCount = 2);

        var filterDefinition = new FilterDefinition();
        filterDefinition.WithTag<Tag1>();
        filterDefinition.WithTag<Tag2>();
        filterDefinition.WithTag<Tag3>();

        Action prepareAction = () => world.GetFilter(filterDefinition);

        prepareAction.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Compile_WithTags_ShouldBeOk()
    {
        ComponentMetadata<Tag1>.InitComponentMetadata();
        ComponentMetadata<Tag2>.InitComponentMetadata();

        var filterDefinition = new FilterDefinition();
        filterDefinition.WithTag<Tag2>();
        filterDefinition.WithTag<Tag1>();

        filterDefinition.IsCompiled.Should().BeFalse();
        filterDefinition.GlobalComponentIndexes.Count.Should().Be(2);
        filterDefinition.GlobalComponentIndexes[0].Should().Be(ComponentMetadata<Tag2>.GlobalIndex);
        filterDefinition.GlobalComponentIndexes[1].Should().Be(ComponentMetadata<Tag1>.GlobalIndex);

        filterDefinition.Compile();

        filterDefinition.IsCompiled.Should().BeTrue();
        filterDefinition.GlobalComponentIndexes[0].Should().Be(ComponentMetadata<Tag1>.GlobalIndex);
        filterDefinition.GlobalComponentIndexes[1].Should().Be(ComponentMetadata<Tag2>.GlobalIndex);
    }

    [Fact]
    public void CompareFilters_WithTags_ShouldBeOk()
    {
        var filterDefinition1 = new FilterDefinition();
        filterDefinition1.WithTag<Tag1>();
        filterDefinition1.WithTag<Tag2>();

        var filterDefinition2 = new FilterDefinition();
        filterDefinition2.WithTag<Tag2>();
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
        world.InitTag<Tag2>();

        var filterDefinition = new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>();

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
        world.InitTag<Tag2>();

        var filterDefinition1 = new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>();

        var filter1 = world.GetFilter(filterDefinition1);

        var filterDefinition2 = new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>();

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
        world.InitTag<Tag2>();

        var filterDefinition12 = new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>();

        var filter1 = world.GetFilter(filterDefinition12);

        var filterDefinition21 = new FilterDefinition().WithTag<Tag2>().WithTag<Tag1>();

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
        world1.InitTag<Tag2>();

        var filterDefinition = new FilterDefinition().WithTag<Tag1>().WithTag<Tag2>();

        var filter = world1.GetFilter(filterDefinition);

        var world2 = WorldFactory.Shared.CreateIndexedWorld();
        world2.InitTag<Tag1>();
        world2.InitTag<Tag2>();

        var action = () =>
        {
            world2.GetFilter(filterDefinition);
        };
        action.Should().Throw<Exception>();
    }

}