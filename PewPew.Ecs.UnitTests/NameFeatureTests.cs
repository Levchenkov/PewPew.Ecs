using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Features;

namespace PewPew.Ecs.UnitTests;

public class NameFeatureTests
{
    [Fact]
    public void CreateEntityId_ProvideName_EntityShouldBeCreated()
    {
        var world1 = new World();
        world1.InitNameFeature();

        var nameFeature = world1.GetNameFeature();
        var entityId1 = nameFeature.CreateEntityId("Entity1");


        var nameFeature2 = world1.GetNameFeature();
        nameFeature2.GetName(entityId1).Should().Be("Entity1");
        nameFeature2.GetEntityId("Entity1").Should().Be(entityId1);
    }

    [Fact]
    public void SetName_SetEntityName_EntityNameShouldBeSet()
    {
        var world1 = new World();
        world1.InitNameFeature();

        var nameFeature = world1.GetNameFeature();

        var entityId2 = world1.CreateEntityId();
        nameFeature.SetName(entityId2, "Entity2");

        var nameFeature2 = world1.GetNameFeature();
        nameFeature2.GetName(entityId2).Should().Be("Entity2");
        nameFeature2.GetEntityId("Entity2").Should().Be(entityId2);
    }

    [Fact]
    public void WorldName_ProvideWorldName_WorldNameShouldBeSet()
    {
        var world1 = new World();
        world1.InitNameFeature();

        var nameFeature = world1.GetNameFeature();
        nameFeature.WorldName = "World1";

        var nameFeature2 = world1.GetNameFeature();
        nameFeature2.WorldName.Should().Be("World1");
    }
}