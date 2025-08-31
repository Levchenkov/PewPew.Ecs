namespace PewPew.Ecs.Core.Internals;

internal interface IPoolableId
{
    int GetIndex();
    ushort GetGeneration();
    ushort GetWorldId();
}