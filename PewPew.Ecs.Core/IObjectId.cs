namespace PewPew.Ecs.Core;

/// <summary>
/// Common contract for versioned storage handles (<see cref="StringId"/>, <see cref="ObjectId"/>).
/// </summary>
public interface IObjectId
{
    int Index { get; }
    ushort Generation { get; }
    ushort StorageId { get; }
}
