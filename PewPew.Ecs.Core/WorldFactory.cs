namespace PewPew.Ecs.Core;

public class WorldFactory
{
    private int _id;

    public static WorldFactory Shared { get; private set; } = new();

    public byte GetNextWorldId() => (byte)Interlocked.Increment(ref _id);

    public World CreateWorld(int entitiesCount = 100) => CreateWorld(x =>
    {
        x.MaxEntitiesCount = entitiesCount;
        x.MaxComponentsPerSet = entitiesCount;
    });

    public World CreateWorld(Action<WorldSettings> settingsAction)
    {
        var worldSettings = new WorldSettings();
        settingsAction(worldSettings);

        var worldId = GetNextWorldId();
        var world = new World(worldId, worldSettings);

        return world;
    }
}