using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class StringStorageTests
{
    private static StringStorage CreateStorage(int capacity = 4) =>
        WorldFactory.Shared.CreateStringStorage(capacity);

    // -- Add / Get --

    [Fact]
    public void Add_ThenGet_ReturnsStoredString()
    {
        var storage = CreateStorage();

        var id = storage.Add("hello");
        storage.Get(id).Should().Be("hello");
    }

    [Fact]
    public void Add_MultipleStrings_EachIdReturnsItsOwnString()
    {
        var storage = CreateStorage();

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
        var storage = CreateStorage();

        var id1 = storage.Add("a");
        var id2 = storage.Add("b");

        id1.Should().NotBe(id2);
    }

    [Fact]
    public void Count_TracksLiveStrings()
    {
        var storage = CreateStorage();
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
        var storage = CreateStorage();

        var id = storage.Add("bye");
        storage.Delete(id);

        storage.IsValid(id).Should().BeFalse();
    }

    [Fact]
    public void IsValid_ForFreshId_ReturnsTrue()
    {
        var storage = CreateStorage();

        var id = storage.Add("live");

        storage.IsValid(id).Should().BeTrue();
    }

    [Fact]
    public void IsValid_ForDefaultId_ReturnsFalse()
    {
        var storage = CreateStorage();

        storage.IsValid(StringId.Invalid).Should().BeFalse();
    }

    [Fact]
    public void IsValid_ForOtherStorageId_ReturnsFalse()
    {
        var storageA = CreateStorage();
        var storageB = CreateStorage();

        var idFromA = storageA.Add("cross");

        storageB.IsValid(idFromA).Should().BeFalse();
    }

    // -- Slot reuse and generation --

    [Fact]
    public void Delete_ThenAdd_ReusesSlotsWithNewGeneration()
    {
        var storage = CreateStorage(capacity: 2);

        var id1 = storage.Add("first");
        storage.Delete(id1);

        var id2 = storage.Add("reused");

        // same physical slot, different generation
        id1.Index.Should().Be(id2.Index);
        id1.Generation.Should().NotBe(id2.Generation);
        storage.IsValid(id1).Should().BeFalse();
        storage.IsValid(id2).Should().BeTrue();
        storage.Get(id2).Should().Be("reused");
    }

    [Fact]
    public void Delete_ThenAdd_OldIdIsStaleNewIdIsLive()
    {
        var storage = CreateStorage();

        var old = storage.Add("old");
        storage.Delete(old);
        var fresh = storage.Add("fresh");

        storage.IsValid(old).Should().BeFalse();
        storage.IsValid(fresh).Should().BeTrue();
        storage.Get(fresh).Should().Be("fresh");
    }

    // -- Update --

    [Fact]
    public void Update_ChangesString_SameIdRemainsValid()
    {
        var storage = CreateStorage();

        var id = storage.Add("original");
        storage.Update(id, "updated");

        storage.IsValid(id).Should().BeTrue();
        storage.Get(id).Should().Be("updated");
    }

    // -- TryGet --

    [Fact]
    public void TryGet_LiveId_ReturnsTrueAndString()
    {
        var storage = CreateStorage();
        var id = storage.Add("try-me");

        var result = storage.TryGet(id, out var value);

        result.Should().BeTrue();
        value.Should().Be("try-me");
    }

    [Fact]
    public void TryGet_StaleId_ReturnsFalse()
    {
        var storage = CreateStorage();
        var id = storage.Add("gone");
        storage.Delete(id);

        var result = storage.TryGet(id, out var value);

        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGet_InvalidId_ReturnsFalse()
    {
        var storage = CreateStorage();

        var result = storage.TryGet(StringId.Invalid, out var value);

        result.Should().BeFalse();
        value.Should().BeNull();
    }

    // -- Growth --

    [Fact]
    public void Add_BeyondInitialCapacity_GrowsAutomatically()
    {
        var storage = CreateStorage(capacity: 2);

        var ids = Enumerable.Range(0, 10)
            .Select(i => storage.Add($"string_{i}"))
            .ToArray();

        for (int i = 0; i < ids.Length; i++)
            storage.Get(ids[i]).Should().Be($"string_{i}");
    }

    // -- StorageId in StringId --

    [Fact]
    public void StringId_StorageId_MatchesStorage()
    {
        var storage = CreateStorage();
        var id = storage.Add("check-storage");

        id.StorageId.Should().Be(storage.Id);
    }

    // -- ECS integration: StringId in component --

    [Fact]
    public void StringIdComponent_StoreAndRetrieve_WorksCorrectly()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<StringIdComponent>();

        var storage = WorldFactory.Shared.CreateStringStorage();

        var entity = world.CreateEntityId();
        ref var component = ref world.AddComponent<StringIdComponent>(entity);
        var longString = "A long description that doesn't fit in BlittableString32 easily.";
        component.DescriptionId = storage.Add(longString);

        var retrieved = world.GetComponent<StringIdComponent>(entity);
        storage.Get(retrieved.DescriptionId).Should().Be(longString);
    }
}
