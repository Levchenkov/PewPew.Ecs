using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.SourceGenerators;

namespace PewPew.Ecs.Hybrid;

[StaticArchetype("T3")]
public readonly ref partial struct StaticArchetype<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
}