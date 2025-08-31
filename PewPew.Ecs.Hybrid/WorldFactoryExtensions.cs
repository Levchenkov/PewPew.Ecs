using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid;

public static class WorldFactoryExtensions
{
    public static HybridWorld CreateHybridWorld(this WorldFactory factory, int entitiesCount = 100)
    {
        return CreateHybridWorld(factory, x =>
        {
            x.MaxEntitiesCount = entitiesCount;
            x.MaxComponentsPerSet = entitiesCount;
            x.MaxEntitiesPerArchetype = entitiesCount;
        });
    }

    public static HybridWorld<TMask> CreateHybridWorld<TMask>(this WorldFactory factory, int entitiesCount = 100)
        where TMask : struct, IBitMask<TMask>
    {
        return CreateHybridWorld<TMask>(factory, x =>
        {
            x.MaxEntitiesCount = entitiesCount;
            x.MaxComponentsPerSet = entitiesCount;
            x.MaxEntitiesPerArchetype = entitiesCount;
        });
    }

    public static HybridWorld CreateHybridWorld(
        this WorldFactory factory,
        Action<HybridWorldSettings> settingsAction)
    {
        var worldSettings = new HybridWorldSettings();

        worldSettings.MaxAllowedUniqueComponentsCount = default(BitMask64).Capacity;

        settingsAction(worldSettings);

        var worldId = factory.GetNextWorldId();
        var world = new HybridWorld(worldId, worldSettings);

        return world;
    }

    public static HybridWorld<TMask> CreateHybridWorld<TMask>(
        this WorldFactory factory,
        Action<HybridWorldSettings> settingsAction)
        where TMask : struct, IBitMask<TMask>
    {
        var worldSettings = new HybridWorldSettings();

        worldSettings.MaxAllowedUniqueComponentsCount = default(TMask).Capacity;

        settingsAction(worldSettings);

        var worldId = factory.GetNextWorldId();
        var world = new HybridWorld<TMask>(worldId, worldSettings);

        return world;
    }
}