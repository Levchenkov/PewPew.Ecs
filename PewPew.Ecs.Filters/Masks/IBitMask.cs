namespace PewPew.Ecs.Filters.Masks;

public interface IBitMask<T> : IEquatable<T>
    where T : IBitMask<T>
{
    int Capacity { get; }

    bool Has(in T value);

    T SetBit(int index);

    T SetBits(in T value);

    T ClearBit(int index);

    T ClearBits(in T value);

    bool GetBit(int index);

    bool HasIntersection(in T value);

    int GetHashCode();
}