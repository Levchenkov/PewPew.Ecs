using System.Diagnostics;
using System.Runtime.CompilerServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Hybrid.Internals;

internal sealed partial class StaticArchetypeInstance<TMask, T1, T2/*{GenericParameters}*/> : IStaticArchetypeInstance<TMask>
    where TMask : struct, IBitMask<TMask>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    /*{GenericConstrains}*/
{
    internal readonly HybridWorld<TMask> World;
    private readonly IResizeStrategy _resizeStrategy;

    private readonly ArchetypeDenseIndexPair[] _sparseToDensePairs;

    internal EntityId[] Entities;
    internal T1[] Components1;
    internal T2[] Components2;
    /*{ComponentFields}*/
    internal readonly RefBox<int> CountBox; // shares between all archetypes with the same components

    private readonly TMask _mask;
    private readonly int _id;

    public StaticArchetypeInstance(
        int id,
        TMask mask,
        ArchetypeDenseIndexPair[] sparseToDensePairs,
        int maxComponentsPerType,
        RefBox<int> countBox,
        HybridWorld<TMask> world,
        IResizeStrategy resizeStrategy)
    {
        _id = id;
        _mask = mask;
        CountBox = countBox;
        World = world;
        _resizeStrategy = resizeStrategy;
        _sparseToDensePairs = sparseToDensePairs;
        Entities = new EntityId[maxComponentsPerType];
        Components1 = new T1[maxComponentsPerType];
        Components2 = new T2[maxComponentsPerType];
        /*{ComponentArrayAllocations}*/
    }

    public StaticArchetypeInstance(
        int id,
        TMask mask,
        ArchetypeDenseIndexPair[] sparseToDensePairs,
        EntityId[] entities,
        T1[] components1,
        T2[] components2,
        /*{ComponentArrayCtorParams}*/
        RefBox<int> countBox,
        HybridWorld<TMask> world,
        IResizeStrategy resizeStrategy)
    {
        _id = id;
        _mask = mask;
        CountBox = countBox;
        World = world;
        _resizeStrategy = resizeStrategy;
        _sparseToDensePairs = sparseToDensePairs;
        Entities = entities;
        Components1 = components1;
        Components2 = components2;
        /*{ComponentArrayInits}*/
    }

    public int Id => _id;

    public TMask Mask => _mask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent<T>(EntityId entityId)
        where T : struct, IComponent
    {
        DebugValidateEntityId(entityId);

        var index = GetRequiredDenseIndex(entityId);

        if (ComponentMetadata<T>.GlobalIndex == ComponentMetadata<T1>.GlobalIndex)
        {
            return ref Unsafe.As<T1, T>(ref Components1[index]);
        }

        if (ComponentMetadata<T>.GlobalIndex == ComponentMetadata<T2>.GlobalIndex)
        {
            return ref Unsafe.As<T2, T>(ref Components2[index]);
        }

        /*{GetComponentGenericCode}*/

        ThrowHelper.ThrowComponentNotFoundException<T>(entityId);

        // never will happen
        return ref Unsafe.As<T1, T>(ref Components1[0]);
    }

    public bool TryGetComponent<T>(EntityId entityId, out ComponentRef<T> componentRef) where T : struct, IComponent
    {
        DebugValidateEntityId(entityId);

        componentRef = default;

        var pair = _sparseToDensePairs[entityId.Index];

        if (pair.ArchetypeId != _id)
            return false;

        var denseIndex = pair.DenseIndex;

        if (ComponentMetadata<T>.GlobalIndex == ComponentMetadata<T1>.GlobalIndex)
        {
            ref var component = ref Unsafe.As<T1, T>(ref Components1[denseIndex]);

            componentRef = new ComponentRef<T>(ref component);

            return true;
        }

        if (ComponentMetadata<T>.GlobalIndex == ComponentMetadata<T2>.GlobalIndex)
        {
            ref var component = ref Unsafe.As<T2, T>(ref Components2[denseIndex]);

            componentRef = new ComponentRef<T>(ref component);

            return true;
        }

        /*{TryGetComponentGenericCode}*/

        ThrowHelper.ThrowComponentNotFoundException<T>(entityId);

        return false;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => CountBox.Ref;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        CountBox.Ref = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T1 GetComponent1(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        return ref Components1[GetRequiredDenseIndex(entityId)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T2 GetComponent2(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        return ref Components2[GetRequiredDenseIndex(entityId)];
    }

    /*{GetComponentMethod}*/

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntity(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var pair = _sparseToDensePairs[entityId.Index];

        return pair.ArchetypeId == _id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponents(EntityId entityId, out ComponentRef<T1, T2> componentRef)
    {
        DebugValidateEntityId(entityId);

        var pair = _sparseToDensePairs[entityId.Index];

        if (pair.ArchetypeId != _id)
        {
            componentRef = default;

            return false;
        }

        componentRef = GetByDenseIndex(pair.DenseIndex);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentRef<T1, T2> GetComponents(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        var pair = _sparseToDensePairs[entityId.Index];

        DebugValidatePair(pair, entityId);

        var componentRef = GetByDenseIndex(pair.DenseIndex);

        return componentRef;
    }

    public void CreateEntity(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var pair = ref _sparseToDensePairs[entityId.Index];

        if (pair.ArchetypeId == _id)
        {
            return;
        }

        World.ValidateNoArchetypeAndNoComponents(entityId, this);

        ref var count = ref CountBox.Ref;

        if (count == Components1.Length)
            Resize();

        var denseIndex = count++;
        Entities[denseIndex] = entityId;
        pair = new ArchetypeDenseIndexPair(_id, denseIndex);

        World.OnComponentAddedToArchetype(entityId, _mask, this);
    }

    public ComponentRef<T1, T2> AddEntity(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var pair = ref _sparseToDensePairs[entityId.Index];

        int denseIndex;
        if (pair.ArchetypeId == _id)
        {
            denseIndex = pair.DenseIndex;
            return new ComponentRef<T1, T2>(ref Components1[denseIndex], ref Components2[denseIndex] /*{ComponentRefCtorParams}*/ );
        }

        World.ValidateNoArchetypeAndNoComponents(entityId, this);

        ref var count = ref CountBox.Ref;
        if (count == Components1.Length)
            Resize();

        denseIndex = count++;
        Entities[denseIndex] = entityId;
        pair = new ArchetypeDenseIndexPair(_id, denseIndex);

        World.OnComponentAddedToArchetype(entityId, _mask, this);

        return new ComponentRef<T1, T2>(ref Components1[denseIndex], ref Components2[denseIndex] /*{ComponentRefCtorParams}*/ );
    }

    public void AddEntity(EntityId entityId, T1 component1, T2 component2 /*{AddEntityParams}*/)
    {
        DebugValidateEntityId(entityId);

        World.ValidateNoArchetypeAndNoComponents(entityId, this);

        ref var pair = ref _sparseToDensePairs[entityId.Index];

        int denseIndex;
        if (pair.ArchetypeId == _id)
        {
            denseIndex = pair.DenseIndex;
            Components1[denseIndex] = component1;
            Components2[denseIndex] = component2;
            /*{AddEntityCopyCode}*/

            return;
        }

        ref var count = ref CountBox.Ref;
        if (count == Components1.Length)
            Resize();

        denseIndex = count++;
        Entities[denseIndex] = entityId;

        Components1[denseIndex] = component1;
        Components2[denseIndex] = component2;
        /*{AddEntityCopyCode}*/
        pair = new ArchetypeDenseIndexPair(_id, denseIndex);

        World.OnComponentAddedToArchetype(entityId, _mask, this);
    }

    public void DeleteEntity(EntityId entityId)
    {
        DebugValidateEntityId(entityId);

        ref var pair = ref _sparseToDensePairs[entityId.Index];

        if (pair.ArchetypeId != _id)
            return;

        var denseIndex = pair.DenseIndex;

        ref var count = ref CountBox.Ref;

#if DEBUG
        if (denseIndex < 0)
            throw new IndexOutOfRangeException("Should be non negative.");

        if (denseIndex >= count)
            throw new IndexOutOfRangeException("Should be less than Count");
#endif

        count--;

        if (denseIndex == count)
        {
            Components1[count] = default;
            Components2[count] = default;
            /*{DeleteEntityDefaultCode}*/

            pair = ArchetypeDenseIndexPair.Invalid;

            World.OnComponentDeletedFromArchetype(entityId, _mask);

            return;
        }

        Components1[denseIndex] = Components1[count];
        Components2[denseIndex] = Components2[count];
        /*{DeleteEntityReplaceCode}*/

        Components1[count] = default;
        Components2[count] = default;
        /*{DeleteEntityDefaultCode}*/

        var replacedEntityId = Entities[denseIndex] = Entities[count];
        _sparseToDensePairs[replacedEntityId.Index] = pair;

        pair = ArchetypeDenseIndexPair.Invalid;

        World.OnComponentDeletedFromArchetype(entityId, _mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<EntityId> GetEntities() => new(Entities, 0, Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetComponents<T>() where T : struct, IComponent
    {
        if (ComponentMetadata<T>.GlobalIndex == ComponentMetadata<T1>.GlobalIndex)
        {
            ref T[] components = ref Unsafe.As<T1[], T[]>(ref Components1);

            return new Span<T>(components, 0, Count);
        }

        if (ComponentMetadata<T>.GlobalIndex == ComponentMetadata<T2>.GlobalIndex)
        {
            ref T[] components = ref Unsafe.As<T2[], T[]>(ref Components2);

            return new Span<T>(components, 0, Count);
        }

        /*{GetComponentsGenericCode}*/

        ThrowHelper.ThrowComponentNotFoundException<T>();

        // never will happen
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetRequiredDenseIndex(EntityId entityId)
    {
        var pair = _sparseToDensePairs[entityId.Index];

#if DEBUG
        if(pair.ArchetypeId != _id)
            throw new NotSupportedException($"{GetType().FullName}  doesn't have entity {entityId}");
#endif

        return pair.DenseIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ComponentRef<T1, T2> GetByDenseIndex(int denseIndex)
    {
#if DEBUG
        if (denseIndex < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException(denseIndex, "Should be non negative.");

        if (denseIndex >= CountBox.Ref)
            ThrowHelper.ThrowArgumentOutOfRangeException(denseIndex, "Should be less than Count");
#endif

        return new ComponentRef<T1, T2>(ref Components1[denseIndex], ref Components2[denseIndex] /*{ComponentRefCtorParams}*/ );
    }

    private void Resize()
    {
        var newSize = _resizeStrategy.GetNewSize(Count);
        EnsureCapacity(newSize);
    }

    private void EnsureCapacity(int capacity)
    {
        if (capacity <= Components1.Length)
            return;

        Array.Resize(ref Components1, capacity);
        Array.Resize(ref Components2, capacity);
        /*{EnsureCapacityCode}*/
        Array.Resize(ref Entities, capacity);
    }

    [Conditional("DEBUG")]
    private void DebugValidatePair(ArchetypeDenseIndexPair pair, EntityId entityId)
    {
        if (pair.ArchetypeId != _id)
            ThrowHelper.ThrowComponentNotFoundException(GetType(), entityId);
    }

    [Conditional("DEBUG")]
    private void DebugValidateEntityId(EntityId entityId)
    {
        if(!World.IsAlive(entityId))
            ThrowHelper.ThrowNotSupportedException($"{GetType().FullName} . Entity {entityId} is not alive.");
    }

    public void MoveEntityTo(EntityId entityId, StaticArchetypeInstance<BitMask64, T1, T2/*{GenericParameters}*/> staticArchetype)
    {
        ComponentRef<T1, T2> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1; // copied
        var component2 = componentRef.Component2; // copied
        /*{MoveEntityToCode}*/

        DeleteEntity(entityId);
        var newTuple = staticArchetype.AddEntity(entityId);
        newTuple.Component1 = component1;
        newTuple.Component2 = component2;
        /*{MoveEntityToCode2}*/
    }

    public void MoveEntityTo<TT1, TT2/*{AltGenericParameters}*/>(EntityId entityId, StaticArchetypeInstance<BitMask64, TT1, TT2/*{AltGenericParameters}*/> staticArchetype)
        where TT1 : struct, IComponent
        where TT2 : struct, IComponent
        /*{AltGenericConstrains}*/
    {
        ComponentRef<T1, T2> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1; // copied
        var component2 = componentRef.Component2; // copied
        /*{MoveEntityToCode}*/

        DeleteEntity(entityId);
        var newTuple = staticArchetype.AddEntity(entityId);

        var t1Index = ComponentMetadata<T1>.GlobalIndex;
        if (t1Index == ComponentMetadata<TT1>.GlobalIndex)
        {
            newTuple.Component1 = Unsafe.As<T1, TT1>(ref component1);
        }
        if (t1Index == ComponentMetadata<TT2>.GlobalIndex)
        {
            newTuple.Component2 = Unsafe.As<T1, TT2>(ref component1);
        }
        /*{CustomMoveEntityToCodeT1}*/

        var t2Index = ComponentMetadata<T2>.GlobalIndex;
        if (t2Index == ComponentMetadata<TT1>.GlobalIndex)
        {
            newTuple.Component1 = Unsafe.As<T2, TT1>(ref component2);
        }
        if (t2Index == ComponentMetadata<TT2>.GlobalIndex)
        {
            newTuple.Component2 = Unsafe.As<T2, TT2>(ref component2);
        }
        /*{CustomMoveEntityToCodeT2}*/

        /*{CustomMoveEntityToCodeTN}*/
    }

    public void MoveEntityToWorld(EntityId entityId)
    {
        ComponentRef<T1, T2> componentRef = GetComponents(entityId);
        var component1 = componentRef.Component1; // copied
        var component2 = componentRef.Component2; // copied
        /*{MoveEntityToCode}*/

        DeleteEntity(entityId);

        World.AddComponent<T1>(entityId) = component1;
        World.AddComponent<T2>(entityId) = component2;
        /*{MoveEntityToWorldCode}*/
    }
}