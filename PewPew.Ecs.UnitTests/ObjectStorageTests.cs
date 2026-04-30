using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class ObjectStorageTests
{
    private static ObjectStorage<T> CreateStorage<T>(int capacity = 4) where T : class =>
        WorldFactory.Shared.CreateObjectStorage<T>(capacity);

    // -- Add / Get --

    [Fact]
    public void Add_ThenGet_ReturnsStoredObject()
    {
        var storage = CreateStorage<string>();

        var id = storage.Add("hello");
        storage.Get(id).Should().Be("hello");
    }

    [Fact]
    public void Add_ReferenceType_StoresAndRetrieves()
    {
        var storage = CreateStorage<List<int>>();
        var list = new List<int> { 1, 2, 3 };

        var id = storage.Add(list);

        storage.Get(id).Should().BeSameAs(list);
    }

    [Fact]
    public void Add_MultipleObjects_EachIdReturnsItsOwn()
    {
        var storage = CreateStorage<string>();

        var id1 = storage.Add("first");
        var id2 = storage.Add("second");
        var id3 = storage.Add("third");

        storage.Get(id1).Should().Be("first");
        storage.Get(id2).Should().Be("second");
        storage.Get(id3).Should().Be("third");
    }

    [Fact]
    public void Add_ReturnsUniqueIds()
    {
        var storage = CreateStorage<string>();

        var id1 = storage.Add("a");
        var id2 = storage.Add("b");

        id1.Should().NotBe(id2);
    }

    [Fact]
    public void Count_TracksLiveObjects()
    {
        var storage = CreateStorage<string>();
        storage.Count.Should().Be(0);

        var id1 = storage.Add("x");
        storage.Count.Should().Be(1);

        storage.Add("y");
        storage.Count.Should().Be(2);

        storage.Delete(id1);
        storage.Count.Should().Be(1);
    }

    // -- Delete / IsValid --

    [Fact]
    public void Delete_MakesIdStale()
    {
        var storage = CreateStorage<string>();

        var id = storage.Add("bye");
        storage.Delete(id);

        storage.IsValid(id).Should().BeFalse();
    }

    [Fact]
    public void IsValid_ForFreshId_ReturnsTrue()
    {
        var storage = CreateStorage<string>();

        var id = storage.Add("live");

        storage.IsValid(id).Should().BeTrue();
    }

    [Fact]
    public void IsValid_ForDefaultId_ReturnsFalse()
    {
        var storage = CreateStorage<string>();

        storage.IsValid(ObjectId.Invalid).Should().BeFalse();
    }

    [Fact]
    public void IsValid_ForOtherStorageId_ReturnsFalse()
    {
        var storageA = CreateStorage<string>();
        var storageB = CreateStorage<string>();

        var idFromA = storageA.Add("cross");

        storageB.IsValid(idFromA).Should().BeFalse();
    }

    // -- Slot reuse and generation --

    [Fact]
    public void Delete_ThenAdd_ReusesSlotsWithNewGeneration()
    {
        var storage = CreateStorage<string>(capacity: 2);

        var id1 = storage.Add("first");
        storage.Delete(id1);

        var id2 = storage.Add("reused");

        id1.Index.Should().Be(id2.Index);
        id1.Generation.Should().NotBe(id2.Generation);
        storage.IsValid(id1).Should().BeFalse();
        storage.IsValid(id2).Should().BeTrue();
        storage.Get(id2).Should().Be("reused");
    }

    // -- Update --

    [Fact]
    public void Update_ChangesObject_SameIdRemainsValid()
    {
        var storage = CreateStorage<string>();

        var id = storage.Add("original");
        storage.Update(id, "updated");

        storage.IsValid(id).Should().BeTrue();
        storage.Get(id).Should().Be("updated");
    }

    [Fact]
    public void Update_WithReferenceType_ReplacesReference()
    {
        var storage = CreateStorage<List<int>>();
        var listA = new List<int> { 1 };
        var listB = new List<int> { 2, 3 };

        var id = storage.Add(listA);
        storage.Update(id, listB);

        storage.Get(id).Should().BeSameAs(listB);
    }

    // -- TryGet --

    [Fact]
    public void TryGet_LiveId_ReturnsTrueAndObject()
    {
        var storage = CreateStorage<string>();
        var id = storage.Add("try-me");

        var result = storage.TryGet(id, out var value);

        result.Should().BeTrue();
        value.Should().Be("try-me");
    }

    [Fact]
    public void TryGet_StaleId_ReturnsFalse()
    {
        var storage = CreateStorage<string>();
        var id = storage.Add("gone");
        storage.Delete(id);

        var result = storage.TryGet(id, out var value);

        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGet_InvalidId_ReturnsFalse()
    {
        var storage = CreateStorage<string>();

        var result = storage.TryGet(ObjectId.Invalid, out var value);

        result.Should().BeFalse();
        value.Should().BeNull();
    }

    // -- Growth --

    [Fact]
    public void Add_BeyondInitialCapacity_GrowsAutomatically()
    {
        var storage = CreateStorage<string>(capacity: 2);

        var ids = Enumerable.Range(0, 10)
            .Select(i => storage.Add($"object_{i}"))
            .ToArray();

        for (int i = 0; i < ids.Length; i++)
            storage.Get(ids[i]).Should().Be($"object_{i}");
    }

    // -- StorageId --

    [Fact]
    public void ObjectId_StorageId_MatchesStorage()
    {
        var storage = CreateStorage<string>();
        var id = storage.Add("check");

        id.StorageId.Should().Be(storage.Id);
    }

    // -- ECS integration --

    [Fact]
    public void ObjectIdComponent_StoreAndRetrieve_WorksCorrectly()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<ObjectIdComponent>();

        var storage = WorldFactory.Shared.CreateObjectStorage<string>();

        var entity = world.CreateEntityId();
        ref var component = ref world.AddComponent<ObjectIdComponent>(entity);
        component.DataId = storage.Add("Stored via ObjectStorage<string>.");

        var retrieved = world.GetComponent<ObjectIdComponent>(entity);
        storage.Get(retrieved.DataId).Should().StartWith("Stored via ObjectStorage");
    }
}
