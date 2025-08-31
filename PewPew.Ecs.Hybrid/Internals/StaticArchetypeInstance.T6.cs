using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.SourceGenerators;

namespace PewPew.Ecs.Hybrid.Internals;

[StaticArchetypeInstance("T3", "T4", "T5", "T6")]
internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
}