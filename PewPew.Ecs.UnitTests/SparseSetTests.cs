using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Exceptions;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class SparseSetTests
{
    private readonly DoubleSizeStrategy _resizeStrategy = new(10);
    private readonly StubEntityManager _stubEntityManager = new();

    [Fact]
    public void HasComponent_EmptySet_ShouldBeFalse()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var result = sparseSet.HasComponent(new EntityId(1, 1, 0));

        result.Should().BeFalse();
    }

    [Fact]
    public void HasComponent_ComponentDoesntExist_ShouldBeFalse()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(1, 1, 0));

        var result = sparseSet.HasComponent(new EntityId(2, 1, 0));

        result.Should().BeFalse();
    }

    [Fact]
    public void HasComponent_ComponentExists_ShouldBeTrue()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(1, 1, 0));

        var result = sparseSet.HasComponent(new EntityId(1, 1, 0));

        result.Should().BeTrue();
    }

    [Fact]
    public void HasComponent_EntityIdIsBiggerThanMaxEntitiesCount_ShouldThrow()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var action = () => sparseSet.HasComponent(new EntityId(10, 1, 0));

        action.Should().ThrowExactly<IndexOutOfRangeException>();
    }

    [Fact]
    public void AddComponent_AddMoreThanMaxComponentPerType_ShouldResize()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(0, 1, 0));
        sparseSet.AddComponent(new EntityId(1, 1, 0));
        var action = () => sparseSet.AddComponent(new EntityId(2, 1, 0));

        action.Should().NotThrow();

        sparseSet.Count.Should().Be(3);
    }

    [Fact]
    public void GetComponent_OneComponentExists_ShouldGet()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0)).Max = 42;

        ref var c = ref sparseSet.GetComponent(new EntityId(3, 1, 0));
        c.Max.Should().Be(42);
    }

    [Fact]
    public void GetComponent_TwoComponentsExist_ShouldGet()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0)).Max = 42;
        sparseSet.AddComponent(new EntityId(5, 1, 0)).Max = 69;

        ref var c = ref sparseSet.GetComponent(new EntityId(3, 1, 0));
        c.Max.Should().Be(42);

        ref var c2 = ref sparseSet.GetComponent(new EntityId(5, 1, 0));
        c2.Max.Should().Be(69);
    }

    [Fact]
    public void GetComponent_ComponentDoesntExist_ShouldThrow()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(0, 1, 0)).Max = 42;

        var action = () => sparseSet.GetComponent(new EntityId(1, 1, 0));

#if DEBUG
        action.Should().ThrowExactly<ComponentNotFoundException>();
#else
        action.Should().ThrowExactly<IndexOutOfRangeException>();
#endif

    }

    [Fact]
    public void Components_TwoComponentsExist_ShouldBeTwo()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(0, 1, 0)).Max = 42;
        sparseSet.AddComponent(new EntityId(1, 1, 0)).Max = 69;

        var components = sparseSet.Components;
        components.Length.Should().Be(2);

        components[0].Max.Should().Be(42);
        components[1].Max.Should().Be(69);
    }

    [Fact]
    public void Entities_TwoComponentsExist_ShouldBeTwo()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0));
        sparseSet.AddComponent(new EntityId(5, 1, 0));

        var entities = sparseSet.Entities;
        entities.Length.Should().Be(2);

        entities[0].Index.Should().Be(3);
        entities[1].Index.Should().Be(5);
    }

    [Fact]
    public void Components_EmptySet_ShouldBeEmpty()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.Components.Length.Should().Be(0);
    }

    [Fact]
    public void Entities_EmptySet_ShouldBeEmpty()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void AddComponent_AddTwiceTheSameEntity_ShouldReturnExisting()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0)).Max = 42;
        sparseSet.AddComponent(new EntityId(3, 1, 0)).Max.Should().Be(42);
    }

    [Fact]
    public void DeleteComponent_ComponentDoesNotExist_ShouldBeOk()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.SwapAndPopComponent(new EntityId(3, 1, 0));
    }

    [Fact]
    public void DeleteComponent_ComponentExists_ShouldBeOk()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var entityId = new EntityId(3, 1, 0);
        sparseSet.AddComponent(entityId).Max = 42;
        sparseSet.SwapAndPopComponent(entityId);

        sparseSet.HasComponent(entityId).Should().BeFalse();
    }

    [Fact]
    public void DeleteComponent_TwoComponentExistsDeleteFirst_ShouldBeOk()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        sparseSet.AddComponent(first).Max = 42;
        sparseSet.AddComponent(second).Max = 69;

        sparseSet.SwapAndPopComponent(first);

        sparseSet.HasComponent(first).Should().BeFalse();
        sparseSet.HasComponent(second).Should().BeTrue();
        sparseSet.GetComponent(second).Max.Should().Be(69);
    }

    [Fact]
    public void DeleteComponent_TwoComponentExistsDeleteSecond_ShouldBeOk()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);

        sparseSet.AddComponent(first).Max = 42;
        sparseSet.AddComponent(second).Max = 69;

        sparseSet.SwapAndPopComponent(second);

        sparseSet.HasComponent(first).Should().BeTrue();
        sparseSet.HasComponent(second).Should().BeFalse();
        sparseSet.GetComponent(first).Max.Should().Be(42);
    }

    [Fact]
    public void DeleteComponent_ThreeComponentExistsDeleteMiddle_ShouldBeOk()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 3, _resizeStrategy, _stubEntityManager);

        var first = new EntityId(3, 1, 0);
        var second = new EntityId(5, 1, 0);
        var third = new EntityId(7, 1, 0);

        sparseSet.AddComponent(first).Max = 42;
        sparseSet.AddComponent(second).Max = 69;
        sparseSet.AddComponent(third).Max = 1024;

        sparseSet.SwapAndPopComponent(second);

        sparseSet.HasComponent(first).Should().BeTrue();
        sparseSet.HasComponent(second).Should().BeFalse();
        sparseSet.HasComponent(third).Should().BeTrue();
        sparseSet.GetComponent(first).Max.Should().Be(42);
        sparseSet.GetComponent(third).Max.Should().Be(1024);
    }


    [Fact]
    public void Components_TwoComponentsExistDeleteFirst_ShouldBeOne()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0)).Max = 42;
        sparseSet.AddComponent(new EntityId(5, 1, 0)).Max = 69;

        sparseSet.SwapAndPopComponent(new EntityId(3, 1, 0));

        sparseSet.Components[0].Max.Should().Be(69);
        sparseSet.Entities[0].Index.Should().Be(5);
    }

    [Fact]
    public void Components_TwoComponentsExistDeleteLast_ShouldBeOne()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0)).Max = 42;
        sparseSet.AddComponent(new EntityId(5, 1, 0)).Max = 69;

        sparseSet.SwapAndPopComponent(new EntityId(5, 1, 0));

        sparseSet.Components[0].Max.Should().Be(42);
        sparseSet.Entities[0].Index.Should().Be(3);
    }

    [Fact]
    public void Components_TwoComponentsExistDeleteOneAddOne_ShouldBeTwo()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0)).Max = 42;
        sparseSet.AddComponent(new EntityId(5, 1, 0)).Max = 69;

        sparseSet.SwapAndPopComponent(new EntityId(3, 1, 0));

        sparseSet.AddComponent(new EntityId(9, 1, 0)).Max = 13;

        sparseSet.Components[0].Max.Should().Be(69);
        sparseSet.Components[1].Max.Should().Be(13);
        sparseSet.Entities[0].Index.Should().Be(5);
        sparseSet.Entities[1].Index.Should().Be(9);
    }

    [Fact]
    public void Components_ThreeComponentsExistDeleteOne_ShouldBeTwo()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 3, _resizeStrategy, _stubEntityManager);

        sparseSet.AddComponent(new EntityId(3, 1, 0)).Max = 42;
        sparseSet.AddComponent(new EntityId(5, 1, 0)).Max = 69;
        sparseSet.AddComponent(new EntityId(9, 1, 0)).Max = 13;

        sparseSet.SwapAndPopComponent(new EntityId(5, 1, 0));

        sparseSet.Components[0].Max.Should().Be(42);
        sparseSet.Components[1].Max.Should().Be(13);
        sparseSet.Entities[0].Index.Should().Be(3);
        sparseSet.Entities[1].Index.Should().Be(9);
    }

    [Fact]
    public void TryGet_Empty_ShouldBeFalse()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 3, _resizeStrategy, _stubEntityManager);

        var hasComponent = sparseSet.TryGetComponent(new EntityId(3, 1, 0), out var wrapper);

        hasComponent.Should().BeFalse();
    }

    [Fact]
    public void TryGet_ComponentExists_ShouldBeTrue()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 3, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        sparseSet.AddComponent(entityId).Max = 42;

        var hasComponent = sparseSet.TryGetComponent(entityId, out var wrapper);

        hasComponent.Should().BeTrue();

        wrapper.Component.Max.Should().Be(42);
    }

    [Fact]
    public void TryGet_ComponentHasBeenDeleted_ShouldBeFalse()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 3, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        sparseSet.AddComponent(entityId).Max = 42;
        sparseSet.SwapAndPopComponent(entityId);

        var hasComponent = sparseSet.TryGetComponent(entityId, out var wrapper);

        hasComponent.Should().BeFalse();
    }

    [Fact]
    public void TryGetAnd_ComponentExistsAndHasBeenChanged_ShouldBeTrue()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 3, _resizeStrategy, _stubEntityManager);
        var entityId = new EntityId(3, 1, 0);

        sparseSet.AddComponent(entityId).Max = 42;

        var hasComponent = sparseSet.TryGetComponent(entityId, out var wrapper);

        hasComponent.Should().BeTrue();

        wrapper.Component.Max.Should().Be(42);

        wrapper.Component.Max = 69;

        sparseSet.GetComponent(entityId).Max.Should().Be(69);
    }

    [Fact]
    public void DeleteComponents()
    {
        var sparseSet = new SparseSet<HealthComponent>(15, 4, _resizeStrategy, _stubEntityManager);

        List<EntityId> entities = new List<EntityId>
        {
            new EntityId(3, 1, 0),
            new EntityId(5, 1, 0),
            new EntityId(9, 1, 0),
            new EntityId(11, 1, 0),
        };
        sparseSet.AddComponent(entities[0]).Max = 42;
        sparseSet.AddComponent(entities[2]).Max = 69;
        sparseSet.AddComponent(entities[3]).Max = 1024;
        sparseSet.AddComponent(new EntityId(13, 1, 0)).Max = 13;

        sparseSet.Count.Should().Be(4);

        sparseSet.DeleteComponents(entities);

        sparseSet.Count.Should().Be(1);
        sparseSet.Components[0].Max.Should().Be(13);
    }

    [Fact]
    public void AddComponents()
    {
        var sparseSet = new SparseSet<HealthComponent>(15, 1, _resizeStrategy, _stubEntityManager);
        sparseSet.AddComponent(new EntityId(7, 1, 0)).Max = 7;

        var denseSet = new DenseSet<HealthComponent>();

        denseSet.Add(new EntityId(3, 1, 0)).Max = 42;
        denseSet.Add(new EntityId(5, 1, 0)).Max = 69;
        denseSet.Add(new EntityId(7, 1, 0)).Max = 1024;
        denseSet.Add(new EntityId(9, 1, 0)).Max = 9;
        denseSet.Add(new EntityId(11, 1, 0)).Max = 11;

        sparseSet.AddComponents(denseSet);
        sparseSet.Count.Should().Be(5);

        sparseSet.Entities[0].Index.Should().Be(7);
        sparseSet.Entities[1].Index.Should().Be(3);
        sparseSet.Entities[2].Index.Should().Be(5);
        sparseSet.Entities[3].Index.Should().Be(9);
        sparseSet.Entities[4].Index.Should().Be(11);

        sparseSet.Components[0].Max.Should().Be(1024);
        sparseSet.Components[1].Max.Should().Be(42);
        sparseSet.Components[2].Max.Should().Be(69);
        sparseSet.Components[3].Max.Should().Be(9);
        sparseSet.Components[4].Max.Should().Be(11);
    }

    [Fact]
    public void Clear()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2, _resizeStrategy, _stubEntityManager);

        var entityId1 = new EntityId(3, 1, 0);
        var entityId2 = new EntityId(5, 1, 0);
        sparseSet.AddComponent(entityId1).Max = 42;
        sparseSet.AddComponent(entityId2).Max = 69;

        sparseSet.Clear();

        sparseSet.Count.Should().Be(0);

        sparseSet.HasComponent(entityId1).Should().BeFalse();
        sparseSet.HasComponent(entityId2).Should().BeFalse();

        sparseSet.AddComponent(entityId2).Max.Should().Be(0);
    }
}