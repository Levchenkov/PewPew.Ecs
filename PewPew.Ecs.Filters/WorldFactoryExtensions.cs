using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters;

public static class WorldFactoryExtensions
{
    public static IndexedWorld CreateIndexedWorld(this WorldFactory factory, int entitiesCount = 100)
    {
        return CreateIndexedWorld(factory, x =>
        {
            x.MaxEntitiesCount = entitiesCount;
            x.MaxComponentsPerSet = entitiesCount;
            x.MaxEntitiesPerFilter = entitiesCount;
        });
    }

    public static IndexedWorld<TMask> CreateIndexedWorld<TMask>(this WorldFactory factory, int entitiesCount = 100)
        where TMask : struct, IBitMask<TMask>
    {
        return CreateIndexedWorld<TMask>(factory, x =>
        {
            x.MaxEntitiesCount = entitiesCount;
            x.MaxComponentsPerSet = entitiesCount;
            x.MaxEntitiesPerFilter = entitiesCount;
        });
    }

    public static IndexedWorld<TMask> CreateIndexedWorld<TMask>(
        this WorldFactory factory,
        Action<IndexedWorldSettings> settingsAction)
        where TMask : struct, IBitMask<TMask>
    {
        var worldSettings = new IndexedWorldSettings();

        worldSettings.MaxAllowedUniqueComponentsCount = default(TMask).Capacity;

        settingsAction(worldSettings);

        var worldId = factory.GetNextWorldId();
        var world = new IndexedWorld<TMask>(worldId, worldSettings);

        return world;
    }

    public static IndexedWorld CreateIndexedWorld(
        this WorldFactory factory,
        Action<IndexedWorldSettings> settingsAction)
    {
        var worldSettings = new IndexedWorldSettings();

        worldSettings.MaxAllowedUniqueComponentsCount = default(BitMask64).Capacity;

        settingsAction(worldSettings);

        var worldId = factory.GetNextWorldId();
        var world = new IndexedWorld(worldId, worldSettings);

        return world;
    }
}