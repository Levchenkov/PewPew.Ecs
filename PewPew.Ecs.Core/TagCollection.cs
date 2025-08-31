using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public readonly ref struct TagCollection<T>
    where T : struct, ITagComponent
{
    private readonly TagSparseSet<T> _tagSparseSet; // TagSparseSet should be the first. Order of fields is important for performance!

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tagSparseSet.Entities;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tagSparseSet.Count;
    }

    internal TagCollection(TagSparseSet<T> tagSparseSet)
    {
        _tagSparseSet = tagSparseSet;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _tagSparseSet.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent(EntityId entityId) => _tagSparseSet.HasComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent(EntityId entityId) => _tagSparseSet.AddComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteComponent(EntityId entityId) => _tagSparseSet.DeleteComponent(entityId);
}