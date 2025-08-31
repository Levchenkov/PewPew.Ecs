using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.UnitTests;

public class TagHeavyFilterDefinitionTests
{
    [Fact]
    public void With_TwiceTheSameTag_ExceptionExpected()
    {
        Action action = () =>
        {
            var filterDefinition = new HeavyFilterDefinition<Tag1, Tag1>();
        };

        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void CompareFilters_WithTags_ShouldBeOk()
    {
        var filterDefinition1 = new HeavyFilterDefinition<Component1, Tag1>();
        var filterDefinition2 = new HeavyFilterDefinition<Tag1, Component1>();

        filterDefinition1.GetHashCode().Should().Be(filterDefinition2.GetHashCode());
        filterDefinition1.Equals(filterDefinition2).Should().BeTrue();
    }

    [Fact]
    public void CreateFilters_SingleFilterDefinition_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var filterDefinition = new HeavyFilterDefinition<Component1, Tag1>();

        HeavyFilter<BitMask64, Component1, Tag1> filter1 = world.FetchHeavyFilter(filterDefinition);
        filter1.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void CreateHeavyFilters_FetchTwice_ShouldBeOk()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Component1>();
        world.InitTag<Tag1>();

        var filterDefinition = new HeavyFilterDefinition<Component1, Tag1>();

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
        world.InitTag<Tag1>();

        var filterDefinition1 = new HeavyFilterDefinition<Component1, Tag1>();

        var filter1 = world.FetchHeavyFilter(filterDefinition1);

        var filterDefinition2 = new HeavyFilterDefinition<Component1, Tag1>();

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
        world.InitTag<Tag1>();

        var filterDefinition12 = new HeavyFilterDefinition<Component1, Tag1>();

        var filter1 = world.FetchHeavyFilter(filterDefinition12);

        var filterDefinition21 = new HeavyFilterDefinition<Tag1, Component1>();

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
        world1.InitTag<Tag1>();

        var filterDefinition = new HeavyFilterDefinition<Component1, Tag1>();

        var filter = world1.FetchHeavyFilter(filterDefinition);

        var world2 = WorldFactory.Shared.CreateIndexedWorld();
        world2.InitComponent<Component1>();
        world2.InitTag<Tag1>();

        var action = () =>
        {
            world2.FetchHeavyFilter(filterDefinition);
        };
        action.Should().Throw<Exception>();
    }

}