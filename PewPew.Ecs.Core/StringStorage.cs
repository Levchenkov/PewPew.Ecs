namespace PewPew.Ecs.Core;

/// <summary>
/// Slot-based storage for strings with versioned <see cref="StringId"/> handles.
/// <para>
/// All operations are O(1). Create instances via <see cref="WorldFactory.CreateStringStorage"/>.
/// </para>
/// </summary>
public sealed class StringStorage : ObjectStorageBase<StringId, string>
{
    public StringStorage(ushort id, int initialCapacity = 16) : base(id, initialCapacity) { }

    protected override StringId CreateId(int index, ushort generation, ushort storageId) =>
        new StringId(index, generation, storageId);
}


