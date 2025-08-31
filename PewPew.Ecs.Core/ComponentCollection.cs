using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public readonly ref struct ComponentCollection<T>
    where T : struct, IComponent
{
    private readonly SparseSet<T> _sparseSet; // SparseSet should be the first. Order of fields is important for performance!

    internal int[] InternalIndexes => _sparseSet.InternalIndexes;

    public Span<T> Components
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _sparseSet.Components;
    }

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _sparseSet.Entities;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _sparseSet.Count;
    }

    internal ComponentCollection(SparseSet<T> sparseSet)
    {
        _sparseSet = sparseSet;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset() => _sparseSet.Reset();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Restore(EntityId[] entities, T[] components, int[] indexes, int count)
        => _sparseSet.Restore(entities, components, indexes, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Backup(out EntityId[] entities, out T[] components, out int[] indexes, out int count)
        => _sparseSet.Backup(out entities, out components, out indexes, out count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _sparseSet.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent(EntityId entityId) => _sparseSet.HasComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent(EntityId entityId) => ref _sparseSet.GetComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, out ComponentRef<T> componentRef) =>
        _sparseSet.TryGetComponent(entityId, out componentRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent(EntityId entityId, ref T component) =>
        _sparseSet.TryGetComponent(entityId, ref component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddComponent(EntityId entityId) => ref _sparseSet.AddComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteComponent(EntityId entityId) => _sparseSet.SwapAndPopComponent(entityId);
}