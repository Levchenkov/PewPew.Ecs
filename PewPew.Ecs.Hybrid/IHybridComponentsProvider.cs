using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid;

public interface IHybridComponentsProvider<TMask> :
    IComponentProvider,
    ITagProvider,
    ISingletonProvider,
    IStaticBufferProvider,
    IDynamicBufferProvider
    where TMask : struct, IBitMask<TMask>
{
}