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

    /// <summary>
    /// Creates a <see cref="StringStorage"/> with a unique storage ID issued by this factory.
    /// </summary>
    public StringStorage CreateStringStorage(int initialCapacity = 16)
    {
        var id = GetNextWorldId();
        return new StringStorage(id, initialCapacity);
    }

    /// <summary>
    /// Creates an <see cref="ObjectStorage{T}"/> with a unique storage ID issued by this factory.
    /// </summary>
    public ObjectStorage<T> CreateObjectStorage<T>(int initialCapacity = 16)
        where T : class
    {
        var id = GetNextWorldId();
        return new ObjectStorage<T>(id, initialCapacity);
    }
}