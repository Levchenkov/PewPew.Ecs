using System.Runtime.InteropServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Filters;

public abstract class HeavyFilterDefinition
{
    internal const int InvalidId = -1;

    internal int HeavyFilterId = InvalidId;
    internal ushort WorldId;
    internal readonly List<int> GlobalComponentIndexes = new();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not HeavyFilterDefinition heavyFilterDefinition) return false;
        return Equals(heavyFilterDefinition);
    }

    private bool Equals(HeavyFilterDefinition other) => GlobalComponentIndexes.SequenceEqual(other.GlobalComponentIndexes);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
#if NETSTANDARD2_1
        var componentIndexes = GlobalComponentIndexes;
        for (var i = 0; i < componentIndexes.Count; i++)
        {
            var index = componentIndexes[i];
            hashCode.Add(index);
        }
#else
        var intSpan = CollectionsMarshal.AsSpan(GlobalComponentIndexes);
        var byteSpan = MemoryMarshal.Cast<int, byte>(intSpan);
        hashCode.AddBytes(byteSpan);
#endif

        return hashCode.ToHashCode();
    }

    protected void AddComponent<T>()
        where T : struct
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        if (index == ComponentMetadata.InvalidIndex)
        {
            ComponentMetadata<T>.InitComponentMetadata();
            index = ComponentMetadata<T>.GlobalIndex;
        }

        if(GlobalComponentIndexes.Contains(index))
            throw new NotSupportedException($"Component type {typeof(T).Name} is already added to filter definition.");

        GlobalComponentIndexes.Add(index);
    }
}

public sealed class HeavyFilterDefinition<T1, T2> : HeavyFilterDefinition
    where T1 : struct
    where T2 : struct
{
    public HeavyFilterDefinition()
    {
        AddComponent<T1>();
        AddComponent<T2>();
        GlobalComponentIndexes.Sort();
    }
}

public sealed class HeavyFilterDefinition<T1, T2, T3> : HeavyFilterDefinition
    where T1 : struct
    where T2 : struct
    where T3 : struct
{
    public HeavyFilterDefinition()
    {
        AddComponent<T1>();
        AddComponent<T2>();
        AddComponent<T3>();
        GlobalComponentIndexes.Sort();
    }
}

internal struct None : IComponent { }