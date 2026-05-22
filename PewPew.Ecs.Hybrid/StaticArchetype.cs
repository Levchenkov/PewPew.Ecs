using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.Hybrid;

public readonly ref partial struct StaticArchetype<TMask, T1, T2/*{GenericParameters}*/>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    /*{GenericConstrains}*/
{
    private readonly StaticArchetypeInstance<TMask, T1, T2/*{GenericParameters}*/> _instance;

    private readonly Ref<T1> _component1;
    private readonly Ref<T2> _component2;
    /*{ComponentFields}*/

    internal StaticArchetype(StaticArchetypeInstance<TMask, T1, T2/*{GenericParameters}*/> instance)
    {
        _instance = instance;
        _component1 = new Ref<T1>(ref instance.Components1[0]);
        _component2 = new Ref<T2>(ref instance.Components2[0]);
        /*{ComponentFieldsInit}*/
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _instance.Count;
    }

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_instance.Entities, 0, Count);
    }

    public Span<T1> Components1
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_instance.Components1, 0, Count);
    }

    public Span<T2> Components2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_instance.Components2, 0, Count);
    }

    /*{ComponentProperties}*/

    public int Id => _instance.Id;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T1 GetComponent1(EntityId entityId) => ref _instance.GetComponent1(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T2 GetComponent2(EntityId entityId) => ref _instance.GetComponent2(entityId);

    /*{GetComponentMethods}*/

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetRequiredIndexUnsafe(EntityId entityId)
    {
        return _instance.GetRequiredDenseIndex(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T1 GetComponent1Unsafe(int index)
    {
        DebugValidateIndex(index);

        return ref Unsafe.Add(ref _component1.Value, index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T2 GetComponent2Unsafe(int index)
    {
        DebugValidateIndex(index);

        return ref Unsafe.Add(ref _component2.Value, index);
    }

    /*{GetComponentUnsafeMethods}*/

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent<T>(EntityId entityId)
        where T : struct, IComponent
        => ref _instance.GetComponent<T>(entityId);

    public bool TryGetComponent<T>(EntityId entityId, out ComponentRef<T> componentRef) where T : struct, IComponent
        => _instance.TryGetComponent(entityId, out componentRef);

    [Conditional("DEBUG")]
    private void DebugValidateIndex(int index)
    {
        if(index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index is out of range.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has(EntityId entityId) => _instance.HasEntity(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponents(EntityId entityId, out ComponentRef<T1, T2> componentRef)
        => _instance.TryGetComponents(entityId, out componentRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef<T1, T2> Get(EntityId entityId) => _instance.GetComponents(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Create(EntityId entityId) => _instance.CreateEntity(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef<T1, T2> Add(EntityId entityId) => _instance.AddEntity(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(EntityId entityId, T1 component1, T2 component2/*{AddMethodParams}*/)
        => _instance.AddEntity(entityId, component1, component2/*{CallParams}*/);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Delete(EntityId entityId) => _instance.DeleteEntity(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo(EntityId entityId, StaticArchetype<BitMask64, T1, T2/*{GenericParameters}*/> staticArchetype)
        =>  _instance.MoveEntityTo(entityId, staticArchetype._instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityTo<TT1, TT2/*{AltGenericParameters}*/>(EntityId entityId, StaticArchetype<BitMask64, TT1, TT2/*{AltGenericParameters}*/> staticArchetype)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        /*{AltGenericConstrains}*/
        => _instance.MoveEntityTo(entityId, staticArchetype._instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveEntityToWorld(EntityId entityId) => _instance.MoveEntityToWorld(entityId);

    internal StaticArchetypeInstance<TMask, T1, T2/*{GenericParameters}*/> Instance
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _instance;
    }
}