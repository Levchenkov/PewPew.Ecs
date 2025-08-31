using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Exceptions;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Hybrid;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.UnitTests;

public class CompactTagSparseSetTests
{
    private readonly DoubleSizeStrategy _resizeStrategy = new(10);
    private readonly StubEntityManager _stubEntityManager = new();

    [Fact]
    public void HasComponent_EmptySet_ShouldBeFalse()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var result = sparseSet.HasComponent(new EntityId(1, 1, 0));

        result.Should().BeFalse();
    }

    [Fact]
    public void HasComponent_ComponentDoesntExist_ShouldBeFalse()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(1, 1, 0));

        var result = sparseSet.HasComponent(new EntityId(2, 1, 0));

        result.Should().BeFalse();
    }

    [Fact]
    public void HasComponent_ComponentExists_ShouldBeTrue()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(1, 1, 0));

        var result = sparseSet.HasComponent(new EntityId(1, 1, 0));

        result.Should().BeTrue();
    }

    [Fact]
    public void AddComponent_AddMoreThanMaxComponentPerType_ShouldResize()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(0, 1, 0));
        sparseSet.AddComponent(new EntityId(1, 1, 0));
        var action = () => sparseSet.AddComponent(new EntityId(2, 1, 0));

        action.Should().NotThrow();
        sparseSet.Count.Should().Be(3);
    }

    [Fact]
    public void Entities_TwoComponentsExist_ShouldBeTwo()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0));
        sparseSet.AddComponent(new EntityId(5, 1, 0));

        var entities = sparseSet.Entities;
        entities.Length.Should().Be(2);

        entities[0].Index.Should().Be(3);
        entities[1].Index.Should().Be(5);
    }

    [Fact]
    public void Entities_EmptySet_ShouldBeEmpty()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void AddComponent_AddTwiceTheSameEntity_ShouldReturnExisting()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0));
        sparseSet.Count.Should().Be(1);

        sparseSet.AddComponent(new EntityId(3, 1, 0));
        sparseSet.Count.Should().Be(1);
    }

    [Fact]
    public void DeleteComponent_ComponentDoesNotExist_ShouldBeOk()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.DeleteComponent(new EntityId(3, 1, 0));
    }

    [Fact]
    public void DeleteComponent_ComponentExists_ShouldBeOk()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0));
        sparseSet.DeleteComponent(new EntityId(3, 1, 0));

        sparseSet.HasComponent(new EntityId(3, 1, 0)).Should().BeFalse();
    }

    [Fact]
    public void DeleteComponent_TwoComponentExistsDeleteFirst_ShouldBeOk()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        sparseSet.AddComponent(first);
        sparseSet.AddComponent(second);

        sparseSet.DeleteComponent(first);

        sparseSet.HasComponent(first).Should().BeFalse();
        sparseSet.HasComponent(second).Should().BeTrue();
    }

    [Fact]
    public void DeleteComponent_TwoComponentExistsDeleteSecond_ShouldBeOk()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        sparseSet.AddComponent(first);
        sparseSet.AddComponent(second);

        sparseSet.DeleteComponent(second);

        sparseSet.HasComponent(first).Should().BeTrue();
        sparseSet.HasComponent(second).Should().BeFalse();
    }

    [Fact]
    public void DeleteComponent_ThreeComponentExistsDeleteMiddle_ShouldBeOk()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 3, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);
        var third = new EntityId(7, 1, 0);

        sparseSet.AddComponent(first);
        sparseSet.AddComponent(second);
        sparseSet.AddComponent(third);

        sparseSet.DeleteComponent(second);

        sparseSet.HasComponent(first).Should().BeTrue();
        sparseSet.HasComponent(second).Should().BeFalse();
        sparseSet.HasComponent(third).Should().BeTrue();
    }

    [Fact]
    public void Components_TwoComponentsExistDeleteFirst_ShouldBeOne()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0));
        sparseSet.AddComponent(new EntityId(5, 1, 0));

        sparseSet.DeleteComponent(new EntityId(3, 1, 0));

        sparseSet.Count.Should().Be(1);
        sparseSet.Entities[0].Index.Should().Be(5);
    }

    [Fact]
    public void Components_TwoComponentsExistDeleteLast_ShouldBeOne()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0));
        sparseSet.AddComponent(new EntityId(5, 1, 0));

        sparseSet.DeleteComponent(new EntityId(5, 1, 0));

        sparseSet.Count.Should().Be(1);
        sparseSet.Entities[0].Index.Should().Be(3);
    }

    [Fact]
    public void Components_TwoComponentsExistDeleteOneAddOne_ShouldBeTwo()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0));
        sparseSet.AddComponent(new EntityId(5, 1, 0));

        sparseSet.DeleteComponent(new EntityId(3, 1, 0));

        sparseSet.AddComponent(new EntityId(9, 1, 0));

        sparseSet.Count.Should().Be(2);
        sparseSet.Entities[0].Index.Should().Be(5);
        sparseSet.Entities[1].Index.Should().Be(9);
    }

    [Fact]
    public void Components_ThreeComponentsExistDeleteOne_ShouldBeTwo()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 3, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0));
        sparseSet.AddComponent(new EntityId(5, 1, 0));
        sparseSet.AddComponent(new EntityId(9, 1, 0));

        sparseSet.DeleteComponent(new EntityId(5, 1, 0));

        sparseSet.Count.Should().Be(2);
        sparseSet.Entities[0].Index.Should().Be(3);
        sparseSet.Entities[1].Index.Should().Be(9);
    }

    [Fact]
    public void Clear()
    {
        var sparseSet = new CompactTagSparseSet<EmptyComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var entityId1 = new EntityId(3, 1, 0);
        var entityId2 = new EntityId(5, 1, 0);
        sparseSet.AddComponent(entityId1);
        sparseSet.AddComponent(entityId2);

        sparseSet.Clear();

        sparseSet.Count.Should().Be(0);

        sparseSet.HasComponent(entityId1).Should().BeFalse();
        sparseSet.HasComponent(entityId2).Should().BeFalse();
    }
}