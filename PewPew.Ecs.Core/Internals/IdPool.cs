using System.Runtime.CompilerServices;

namespace PewPew.Ecs.Core.Internals;

internal abstract class IdPool<T>
    where T : struct, IEquatable<T>, IPoolableId
{
    // EntityId Index starts from 0, WorldId starts from 0, so Generation should start from 1, bcs valid entity should not be equal EntityId.Invalid
    public const ushort InitialGen = 1;
    private const int InvalidIndex = -1;

    private readonly ushort _worldId;
    private readonly T _invalidValue;

    private int[] _indexes;
    private T[] _values;
    private T[] _map;
    private int _count;
    private int _nextCreateIndex;

    public Span<T> Values => new(_values, 0, _count);

    public int Capacity => _values.Length;

    protected IdPool(ushort worldId, int maxCount, T invalidValue)
    {
        if (maxCount < 0)
            throw new ArgumentException($"{nameof(maxCount)}: {maxCount}");

        _worldId = worldId;
        _invalidValue = invalidValue;
        _indexes = new int[maxCount];
        _values = new T[maxCount];
        _map = new T[maxCount];

        Reset();
    }

    protected abstract T Create(int index, ushort gen, ushort worldId);

    protected abstract ushort GetNextGen(T id);

    internal void Reset()
    {
        _count = 0;

        new Span<int>(_indexes).Fill(InvalidIndex);
        new Span<T>(_values).Fill(_invalidValue);
        new Span<T>(_map).Fill(_invalidValue);
    }

    public T Get()
    {
        if (_count == _values.Length)
            ThrowHelper.ThrowNotSupportedException("Reached maximum entity capacity. Max capacity can be provided when world creates.");

        if (_count == _nextCreateIndex)
        {
            // create a new
            var id = Create(_nextCreateIndex, InitialGen, _worldId);
            _indexes[_nextCreateIndex] = _count;
            _map[_nextCreateIndex] = id;
            _values[_count] = id;
            ++_count;
            ++_nextCreateIndex;

            return id;
        }
        else
        {
            // return a created early
            var entityId = _values[_count];
            var idIndex = entityId.GetIndex();
            _indexes[idIndex] = _count;
            _map[idIndex] = entityId;
            ++_count;

            return entityId;
        }
    }

    public void Return(T id)
    {
        if (id.GetWorldId() != _worldId)
            ThrowHelper.ThrowNotSupportedException($"Wrong worldId. Pool world: {_worldId}, EntityId world: {id.GetWorldId()}");

        var idIndex = id.GetIndex();
        ref var index = ref _indexes[idIndex];
        if (index == InvalidIndex)
            return;

        var invalidatedEntityId = Invalidate(id);
        _map[idIndex] = invalidatedEntityId;

        --_count;
        if (index == _count)
        {
            // delete the last
            _values[_count] = invalidatedEntityId;
            index = InvalidIndex;
        }
        else
        {
            // swap with the last
            var replacedEntityId = _values[index] = _values[_count];
            _values[_count] = invalidatedEntityId;

            var replacedIndex = replacedEntityId.GetIndex();
            _indexes[replacedIndex] = index;
            index = InvalidIndex;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAlive(T id)
    {
        var existingEntityId = _map[id.GetIndex()];
        if (existingEntityId.Equals(id))
        {
            return true;
        }

        if (id.GetWorldId() != _worldId)
            ThrowHelper.ThrowNotSupportedException($"Wrong worldId. Pool world: {_worldId}, EntityId world: {id.GetWorldId()}");

        // Generation is changed or Index is Invalid

        return false;
    }

    private T Invalidate(T id)
    {
        var newGen = GetNextGen(id);
        return Create(id.GetIndex(), newGen, _worldId);
    }
}