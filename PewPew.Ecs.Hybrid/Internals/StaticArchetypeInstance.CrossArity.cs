using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid.Internals;

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
    {
        ComponentRef<T1, T2> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
    {
        ComponentRef<T1, T2> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4, TT5> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
    {
        ComponentRef<T1, T2> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);
        if (t1Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T1, TT5>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);
        if (t2Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T2, TT5>(ref component2);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4, TT5, TT6> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        where TT6 : struct, IComponent
    {
        ComponentRef<T1, T2> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);
        if (t1Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T1, TT5>(ref component1);
        if (t1Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T1, TT6>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);
        if (t2Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T2, TT5>(ref component2);
        if (t2Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T2, TT6>(ref component2);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
    {
        ComponentRef<T1, T2, T3> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
    {
        ComponentRef<T1, T2, T3> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);
        if (t3Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T3, TT4>(ref component3);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4, TT5> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
    {
        ComponentRef<T1, T2, T3> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);
        if (t1Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T1, TT5>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);
        if (t2Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T2, TT5>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);
        if (t3Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T3, TT4>(ref component3);
        if (t3Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T3, TT5>(ref component3);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4, TT5, TT6> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        where TT6 : struct, IComponent
    {
        ComponentRef<T1, T2, T3> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);
        if (t1Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T1, TT5>(ref component1);
        if (t1Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T1, TT6>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);
        if (t2Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T2, TT5>(ref component2);
        if (t2Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T2, TT6>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);
        if (t3Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T3, TT4>(ref component3);
        if (t3Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T3, TT5>(ref component3);
        if (t3Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T3, TT6>(ref component3);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);
        if (t4Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T4, TT3>(ref component4);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4, TT5> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);
        if (t1Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T1, TT5>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);
        if (t2Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T2, TT5>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);
        if (t3Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T3, TT4>(ref component3);
        if (t3Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T3, TT5>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);
        if (t4Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T4, TT3>(ref component4);
        if (t4Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T4, TT4>(ref component4);
        if (t4Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T4, TT5>(ref component4);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4, TT5, TT6> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        where TT6 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);
        if (t1Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T1, TT5>(ref component1);
        if (t1Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T1, TT6>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);
        if (t2Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T2, TT5>(ref component2);
        if (t2Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T2, TT6>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);
        if (t3Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T3, TT4>(ref component3);
        if (t3Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T3, TT5>(ref component3);
        if (t3Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T3, TT6>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);
        if (t4Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T4, TT3>(ref component4);
        if (t4Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T4, TT4>(ref component4);
        if (t4Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T4, TT5>(ref component4);
        if (t4Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T4, TT6>(ref component4);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4, T5> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;
        var component5 = componentRef.Component5;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);

        var t5Index = ComponentMetadata<T5>.GlobalIndex;
        if (t5Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T5, TT1>(ref component5);
        if (t5Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T5, TT2>(ref component5);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4, T5> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;
        var component5 = componentRef.Component5;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);
        if (t4Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T4, TT3>(ref component4);

        var t5Index = ComponentMetadata<T5>.GlobalIndex;
        if (t5Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T5, TT1>(ref component5);
        if (t5Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T5, TT2>(ref component5);
        if (t5Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T5, TT3>(ref component5);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4, T5> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;
        var component5 = componentRef.Component5;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);
        if (t3Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T3, TT4>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);
        if (t4Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T4, TT3>(ref component4);
        if (t4Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T4, TT4>(ref component4);

        var t5Index = ComponentMetadata<T5>.GlobalIndex;
        if (t5Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T5, TT1>(ref component5);
        if (t5Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T5, TT2>(ref component5);
        if (t5Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T5, TT3>(ref component5);
        if (t5Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T5, TT4>(ref component5);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5, TT6>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4, TT5, TT6> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
        where TT6 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4, T5> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;
        var component5 = componentRef.Component5;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);
        if (t1Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T1, TT5>(ref component1);
        if (t1Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T1, TT6>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);
        if (t2Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T2, TT5>(ref component2);
        if (t2Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T2, TT6>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);
        if (t3Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T3, TT4>(ref component3);
        if (t3Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T3, TT5>(ref component3);
        if (t3Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T3, TT6>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);
        if (t4Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T4, TT3>(ref component4);
        if (t4Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T4, TT4>(ref component4);
        if (t4Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T4, TT5>(ref component4);
        if (t4Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T4, TT6>(ref component4);

        var t5Index = ComponentMetadata<T5>.GlobalIndex;
        if (t5Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T5, TT1>(ref component5);
        if (t5Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T5, TT2>(ref component5);
        if (t5Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T5, TT3>(ref component5);
        if (t5Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T5, TT4>(ref component5);
        if (t5Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T5, TT5>(ref component5);
        if (t5Index == ComponentMetadata<TT6>.GlobalIndex)
            newTuple.Component6 = Unsafe.As<T5, TT6>(ref component5);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4, T5, T6> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;
        var component5 = componentRef.Component5;
        var component6 = componentRef.Component6;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);

        var t5Index = ComponentMetadata<T5>.GlobalIndex;
        if (t5Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T5, TT1>(ref component5);
        if (t5Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T5, TT2>(ref component5);

        var t6Index = ComponentMetadata<T6>.GlobalIndex;
        if (t6Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T6, TT1>(ref component6);
        if (t6Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T6, TT2>(ref component6);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4, T5, T6> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;
        var component5 = componentRef.Component5;
        var component6 = componentRef.Component6;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);
        if (t4Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T4, TT3>(ref component4);

        var t5Index = ComponentMetadata<T5>.GlobalIndex;
        if (t5Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T5, TT1>(ref component5);
        if (t5Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T5, TT2>(ref component5);
        if (t5Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T5, TT3>(ref component5);

        var t6Index = ComponentMetadata<T6>.GlobalIndex;
        if (t6Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T6, TT1>(ref component6);
        if (t6Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T6, TT2>(ref component6);
        if (t6Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T6, TT3>(ref component6);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4, T5, T6> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;
        var component5 = componentRef.Component5;
        var component6 = componentRef.Component6;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);
        if (t3Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T3, TT4>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);
        if (t4Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T4, TT3>(ref component4);
        if (t4Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T4, TT4>(ref component4);

        var t5Index = ComponentMetadata<T5>.GlobalIndex;
        if (t5Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T5, TT1>(ref component5);
        if (t5Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T5, TT2>(ref component5);
        if (t5Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T5, TT3>(ref component5);
        if (t5Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T5, TT4>(ref component5);

        var t6Index = ComponentMetadata<T6>.GlobalIndex;
        if (t6Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T6, TT1>(ref component6);
        if (t6Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T6, TT2>(ref component6);
        if (t6Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T6, TT3>(ref component6);
        if (t6Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T6, TT4>(ref component6);

    }
}

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
    where T6 : struct, IComponent
{
    public void MoveEntityTo<TT1, TT2, TT3, TT4, TT5>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2, TT3, TT4, TT5> target)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        where TT3 : struct, IComponent
        where TT4 : struct, IComponent
        where TT5 : struct, IComponent
    {
        ComponentRef<T1, T2, T3, T4, T5, T6> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1;
        var component2 = componentRef.Component2;
        var component3 = componentRef.Component3;
        var component4 = componentRef.Component4;
        var component5 = componentRef.Component5;
        var component6 = componentRef.Component6;

        DeleteEntity(entityId);
        var newTuple = target.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        if (t1Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T1, TT3>(ref component1);
        if (t1Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T1, TT4>(ref component1);
        if (t1Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T1, TT5>(ref component1);

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        if (t2Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T2, TT3>(ref component2);
        if (t2Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T2, TT4>(ref component2);
        if (t2Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T2, TT5>(ref component2);

        var t3Index = ComponentMetadata<T3>.GlobalIndex;
        if (t3Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T3, TT1>(ref component3);
        if (t3Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T3, TT2>(ref component3);
        if (t3Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T3, TT3>(ref component3);
        if (t3Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T3, TT4>(ref component3);
        if (t3Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T3, TT5>(ref component3);

        var t4Index = ComponentMetadata<T4>.GlobalIndex;
        if (t4Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T4, TT1>(ref component4);
        if (t4Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T4, TT2>(ref component4);
        if (t4Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T4, TT3>(ref component4);
        if (t4Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T4, TT4>(ref component4);
        if (t4Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T4, TT5>(ref component4);

        var t5Index = ComponentMetadata<T5>.GlobalIndex;
        if (t5Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T5, TT1>(ref component5);
        if (t5Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T5, TT2>(ref component5);
        if (t5Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T5, TT3>(ref component5);
        if (t5Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T5, TT4>(ref component5);
        if (t5Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T5, TT5>(ref component5);

        var t6Index = ComponentMetadata<T6>.GlobalIndex;
        if (t6Index == ComponentMetadata<TT1>.GlobalIndex)
            newTuple.Component1 = Unsafe.As<T6, TT1>(ref component6);
        if (t6Index == ComponentMetadata<TT2>.GlobalIndex)
            newTuple.Component2 = Unsafe.As<T6, TT2>(ref component6);
        if (t6Index == ComponentMetadata<TT3>.GlobalIndex)
            newTuple.Component3 = Unsafe.As<T6, TT3>(ref component6);
        if (t6Index == ComponentMetadata<TT4>.GlobalIndex)
            newTuple.Component4 = Unsafe.As<T6, TT4>(ref component6);
        if (t6Index == ComponentMetadata<TT5>.GlobalIndex)
            newTuple.Component5 = Unsafe.As<T6, TT5>(ref component6);

    }
}

