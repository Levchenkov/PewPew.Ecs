using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters;

public interface IIndexedComponentsProvider<TMask> :
    IIndexedComponentCollectionProvider<TMask>,
    IIndexedTagCollectionProvider<TMask>,
    ISingletonProvider,
    IStaticBufferCollectionProvider
    where TMask : struct, IBitMask<TMask>
{
}

public interface IIndexedComponentCollectionProvider<TMask> : IComponentProvider
    where TMask : struct, IBitMask<TMask>
{
    IndexedComponentCollection<T, TMask> GetComponents<T>() where T : struct, IComponent;
}

public interface IIndexedTagCollectionProvider<TMask> : ITagProvider
    where TMask : struct, IBitMask<TMask>
{
    IndexedTagCollection<T, TMask> GetTags<T>() where T : struct, ITagComponent;
}