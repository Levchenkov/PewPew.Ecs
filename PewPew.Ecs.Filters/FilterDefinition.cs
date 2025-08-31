using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Filters;

public class FilterDefinition : FilterDefinitionBase
{
    public FilterDefinition With<T>()
        where T : struct, IComponent
    {
        WithInternal<T>();

        return this;
    }

    public FilterDefinition WithTag<T>()
        where T : struct, ITagComponent
    {
        WithInternal<T>();

        return this;
    }
}

public sealed class FilterDefinition<T1, T2> : FilterDefinitionBase
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    public FilterDefinition()
    {
        WithInternal<T1>();
        WithInternal<T2>();
        Compile();
    }
}

public abstract class FilterDefinitionBase
{
    internal const int InvalidId = -1;

    internal int FilterId = InvalidId;

    internal ushort WorldId;

    private bool _isCompiled;

    protected internal readonly List<int> GlobalComponentIndexes = new();

    protected internal int LastAddedGlobalComponentIndex = ComponentMetadata.InvalidIndex;

    internal bool IsCompiled => _isCompiled;

    protected void WithInternal<T>()
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
        LastAddedGlobalComponentIndex = index;

        Invalidate();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Compile()
    {
        if (_isCompiled)
            return;

        if (GlobalComponentIndexes.Count == 0)
            ThrowHelper.ThrowNotSupportedException("No components was added to the filter definition.");

        if (GlobalComponentIndexes.Count == 1)
            ThrowHelper.ThrowNotSupportedException("Filter definition with single component is not supported.");

        GlobalComponentIndexes.Sort();
        _isCompiled = true;
    }

    private void Invalidate()
    {
        FilterId = InvalidId;
        _isCompiled = false;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((FilterDefinition)obj);
    }

    private bool Equals(FilterDefinition other) => GlobalComponentIndexes.SequenceEqual(other.GlobalComponentIndexes);

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

    public override string ToString()
    {
        return $"Hash: {GetHashCode()}, Indexes: {string.Join(", ", GlobalComponentIndexes)}";
    }
}