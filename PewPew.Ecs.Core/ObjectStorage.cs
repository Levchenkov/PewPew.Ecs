namespace PewPew.Ecs.Core;

/// <summary>
/// Slot-based storage for reference-type objects with versioned <see cref="ObjectId"/> handles.
/// <para>
/// All operations are O(1). Create instances via <see cref="WorldFactory.CreateObjectStorage{T}"/>.
/// </para>
/// </summary>
public sealed class ObjectStorage<T> : ObjectStorageBase<ObjectId, T>
    where T : class
{
    public ObjectStorage(ushort id, int initialCapacity = 16) : base(id, initialCapacity) { }

    protected override ObjectId CreateId(int index, ushort generation, ushort storageId) =>
        new ObjectId(index, generation, storageId);
}

