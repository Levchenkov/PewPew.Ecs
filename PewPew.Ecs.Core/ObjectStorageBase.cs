using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

/// <summary>
/// Generic base class for versioned slot-based storages.
/// <para>
/// Manages the full lifecycle: slot allocation, generation tracking, free-list recycling,
/// and capacity growth. Subclasses only need to implement <see cref="CreateId"/>.
/// </para>
/// </summary>
public abstract class ObjectStorageBase<TId, TObject>
    where TId : struct, IObjectId
    where TObject : class
{
    private const ushort InitialGeneration = 1;

    private TObject?[] _objects;
    private ushort[] _generations;
    private int[] _freeList;
    private int _freeCount;
    private int _nextNewIndex;
    private int _count;

    /// <summary>The unique ID of this storage, encoded in every handle it issues.</summary>
    public ushort Id { get; }

    /// <summary>Number of live objects currently stored.</summary>
    public int Count => _count;

    protected ObjectStorageBase(ushort id, int initialCapacity)
    {
        Id = id;
        _objects = new TObject?[initialCapacity];
        _generations = new ushort[initialCapacity];
        _freeList = new int[initialCapacity];
    }

    /// <summary>Creates a handle for the given slot coordinates. Implemented by each subclass.</summary>
    protected abstract TId CreateId(int index, ushort generation, ushort storageId);

    /// <summary>Stores <paramref name="value"/> and returns its versioned handle. O(1).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TId Add(TObject value)
    {
        int index;
        ushort generation;

        if (_freeCount > 0)
        {
            index = _freeList[--_freeCount];
            generation = _generations[index];
        }
        else
        {
            index = _nextNewIndex;
            if (index == _objects.Length)
                Grow();

            _generations[index] = InitialGeneration;
            generation = InitialGeneration;
            ++_nextNewIndex;
        }

        _objects[index] = value;
        ++_count;

        return CreateId(index, generation, Id);
    }

    /// <summary>
    /// Returns the object for a live handle. O(1).
    /// Throws in DEBUG if the handle is stale, out of range, or belongs to a different storage.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TObject Get(TId id)
    {
        DebugValidate(id);
        return _objects[id.Index]!;
    }

    /// <summary>
    /// Replaces the stored object without changing the handle. O(1).
    /// Throws in DEBUG if the handle is invalid.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(TId id, TObject value)
    {
        DebugValidate(id);
        _objects[id.Index] = value;
    }

    /// <summary>
    /// Returns <c>true</c> and the object when <paramref name="id"/> is live. O(1).
    /// Returns <c>false</c> for stale, out-of-range, or wrong-storage handles.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGet(TId id, out TObject? value)
    {
        if (!IsValid(id))
        {
            value = null;
            return false;
        }

        value = _objects[id.Index];
        return true;
    }

    /// <summary>
    /// Frees the slot and increments its generation so that the existing handle becomes stale. O(1).
    /// Throws in DEBUG if the handle is invalid.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Delete(TId id)
    {
        DebugValidate(id);

        var index = id.Index;
        _objects[index] = null;
        _generations[index] = NextGeneration(_generations[index]);

        if (_freeCount == _freeList.Length)
            Array.Resize(ref _freeList, Math.Max(_freeList.Length * 2, 4));

        _freeList[_freeCount++] = index;
        --_count;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="id"/> refers to a live object in this storage. O(1).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid(TId id)
    {
        if (id.StorageId != Id)
            return false;

        var index = id.Index;
        if ((uint)index >= (uint)_nextNewIndex)
            return false;

        return _generations[index] == id.Generation;
    }

    [Conditional("DEBUG")]
    private void DebugValidate(TId id)
    {
        if (id.StorageId != Id)
            ThrowHelper.ThrowNotSupportedException(
                $"{typeof(TId).Name} belongs to storage {id.StorageId}, not {Id}.");

        var index = id.Index;
        if ((uint)index >= (uint)_nextNewIndex)
            ThrowHelper.ThrowNotSupportedException(
                $"{typeof(TId).Name} index {index} is out of range [0, {_nextNewIndex}).");

        if (_generations[index] != id.Generation)
            ThrowHelper.ThrowNotSupportedException(
                $"{typeof(TId).Name} is stale: slot generation is {_generations[index]}, id generation is {id.Generation}.");
    }

    private static ushort NextGeneration(ushort gen) =>
        gen == ushort.MaxValue ? InitialGeneration : (ushort)(gen + 1);

    private void Grow()
    {
        var newSize = _objects.Length * 2;
        Array.Resize(ref _objects, newSize);
        Array.Resize(ref _generations, newSize);
    }
}
