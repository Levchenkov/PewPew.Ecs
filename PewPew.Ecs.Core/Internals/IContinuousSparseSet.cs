namespace PewPew.Ecs.Core.Internals;

internal interface IContinuousSparseSet : IComponentCollection
{
    int[] InternalIndexes { get; }
}