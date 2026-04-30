using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Features;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters.Internals;
using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.Filters;

public sealed class IndexedWorld
#if BIT_MASK_64_DEFAULT
    : IndexedWorld<BitMask64>
#elif BIT_MASK_128_DEFAULT
    : IndexedWorld<BitMask128>
#elif BIT_MASK_256_DEFAULT
    : IndexedWorld<BitMask256>
#endif
{
    internal IndexedWorld(ushort id = 0) : base(id)
    {
    }

    internal IndexedWorld(ushort id, IndexedWorldSettings settings) : base(id, settings)
    {
    }
}

public partial class IndexedWorld<TMask> :
    IIndexedComponentsProvider<TMask>,
    IComponentsInitializer,
    IEntityManager,
    ICommandBufferApplier
    where TMask : struct, IBitMask<TMask>
{
    private const int MaxAllowedUniqueComponentsCount = 64; // max performance with this limit

    private readonly IndexedWorldSettings _settings;
    private readonly World _world;

    private readonly FilterModule<TMask, FilterInstance<TMask>> _module;
    internal readonly TMask[] _entityMasks;

    private FilterInstance<TMask>[] _filters;

    public ushort Id => _world.Id;

    public int MaxUniqueComponentsCount => _world.MaxUniqueComponentsCount;

    public int EntityCapacity => _world.EntityCapacity;

    public Span<EntityId> Entities => _world.Entities;

    public IndexedWorld(ushort id = 0) : this(id, new IndexedWorldSettings { MaxAllowedUniqueComponentsCount = MaxAllowedUniqueComponentsCount })
    {
    }

    public IndexedWorld(ushort id, IndexedWorldSettings settings)
    {
        _settings = settings;

        if (!typeof(TMask).IsValueType)
            throw new NotSupportedException();

        if(default(TMask).Capacity < settings.MaxAllowedUniqueComponentsCount)
            throw new NotSupportedException();

        _world = new World(id, settings);
        _module = new FilterModule<TMask, FilterInstance<TMask>>(settings.MaxAllowedUniqueComponentsCount);

        _entityMasks = new TMask[settings.MaxEntitiesCount];

        _filters = new FilterInstance<TMask>[10];

#if HEAVY_FILTERS_ENABLED
        HeavyFilterCtor();
#endif
    }

    public void InitComponent<T>()
        where T : struct, IComponent
        => InitComponent<T>(_settings.MaxComponentsPerSet);

    public void InitComponent<T>(int maxComponentsPerType)
        where T : struct, IComponent
    {
        _world.InitComponent<T>(maxComponentsPerType);

        _module.InitComponent<T>();
#if HEAVY_FILTERS_ENABLED
        _heavyFilterModule.InitComponent<T>();
#endif
    }

    public void InitTag<T>() where T : struct, ITagComponent
        => InitTag<T>(_settings.MaxAllowedUniqueComponentsCount);

    public void InitTag<T>(int maxComponentsPerSet) where T : struct, ITagComponent
    {
        _world.InitTag<T>(maxComponentsPerSet);

        _module.InitTag<T>();
#if HEAVY_FILTERS_ENABLED
        _heavyFilterModule.InitTag<T>();
#endif
    }

    public void InitSingleton<T>() where T : struct, ISingletonComponent
        => _world.InitSingleton<T>();

    public void InitStaticBuffer<T>() where T : struct, IStaticBufferComponent
        => InitStaticBuffer<T>(_settings.MaxComponentsPerSet / _settings.MaxElementsCountPerSet, _settings.MaxElementsCountPerSet);

    public void InitStaticBuffer<T>(int maxComponentsPerSet, int maxElementsCount) where T : struct, IStaticBufferComponent
        => _world.InitStaticBuffer<T>(maxComponentsPerSet, maxElementsCount);

    public void InitDynamicBuffer<T>() where T : struct, IDynamicBufferComponent
        => _world.InitDynamicBuffer<T>();

    public void InitDynamicBuffer<T>(int maxComponentsPerSet, int initialCapacity) where T : struct, IDynamicBufferComponent
        => _world.InitDynamicBuffer<T>(maxComponentsPerSet, initialCapacity);

    public bool IsComponentInitialized<TComponent>()
        where TComponent : struct
        => _world.IsComponentInitialized<TComponent>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EntityId CreateEntityId() => _world.CreateEntityId();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteEntityId(EntityId entityId)
    {
        foreach (var filter in _filters)
        {
            if (filter == null)
                break;

            filter.DeleteEntity(entityId);
        }

#if HEAVY_FILTERS_ENABLED
        foreach (var heavyFilter in _heavyFilters)
        {
            if (heavyFilter == null)
                break;

            heavyFilter.DeleteEntity(entityId);
        }
#endif

        _world.DeleteEntityId(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAlive(EntityId entityId) => _world.IsAlive(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal SparseSet<T> GetSparseSet<T>(int globalIndex) where T : struct, IComponent
        => (SparseSet<T>)_world.GetCollectionByGlobalIndex<T>(globalIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TagSparseSet<T> GetTagSparseSet<T>(int index) where T : struct, ITagComponent
        =>  (TagSparseSet<T>)_world.GetCollectionByGlobalIndex<T>(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal IComponentCollection GetComponentCollectionByGlobalIndex(int index)
        => (IComponentCollection)_world.GetCollectionByGlobalIndex(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal SparseSet<T> GetSparseSet<T>()
        where T : struct, IComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = GetSparseSet<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TagSparseSet<T> GetTagSparseSet<T>()
        where T : struct, ITagComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = GetTagSparseSet<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal SingletonStorage<T> GetSingletonStorageInternal<T>()
        where T : struct, ISingletonComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = (SingletonStorage<T>)_world.GetCollectionByGlobalIndex<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal StaticBufferSet<T> GetStaticBufferSet<T>()
        where T : struct, IStaticBufferComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = (StaticBufferSet<T>)_world.GetCollectionByGlobalIndex<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IndexedComponentCollection<T, TMask> GetComponents<T>()
        where T : struct, IComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        var set = GetSparseSet<T>(globalIndex);

        var localIndex = _module.GetLocalIndex(globalIndex);
        var filters = _module.GetFilters(globalIndex);

#if HEAVY_FILTERS_ENABLED
        var heavyFilters = _heavyFilterModule.GetHeavyFilters(globalIndex);
        return new IndexedComponentCollection<T, TMask>(set, this, globalIndex, localIndex, filters, heavyFilters);
#else
        return new IndexedComponentCollection<T, TMask>(set, this, globalIndex, localIndex, filters);
#endif
    }

    public IndexedTagCollection<T, TMask> GetTags<T>() where T : struct, ITagComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        var set = GetTagSparseSet<T>(globalIndex);

        var localIndex = _module.GetLocalIndex(globalIndex);
        var filters = _module.GetFilters(globalIndex);

#if HEAVY_FILTERS_ENABLED
        var heavyFilters = _heavyFilterModule.GetHeavyFilters(globalIndex);
        return new IndexedTagCollection<T, TMask>(set, this, globalIndex, localIndex, filters, heavyFilters);
#else
        return new IndexedTagCollection<T, TMask>(set, this, globalIndex, localIndex, filters);
#endif
    }

    public StaticBufferCollection<T> GetStaticBuffers<T>() where T : struct, IStaticBufferComponent
    {
        var staticBufferSet = GetStaticBufferSet<T>();

        return new  StaticBufferCollection<T>(staticBufferSet);
    }

    public void WarmUpFilter(FilterDefinitionBase filterDefinition)
    {
        GetFilter(filterDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Filter<TMask> GetFilter(FilterDefinitionBase filterDefinition)
    {
        var filterId = filterDefinition.FilterId;
        if (filterId == FilterDefinitionBase.InvalidId)
        {
            var filter = FindOrCreateFilter(this, filterDefinition);

            return new Filter<TMask>(filter);
        }

#if DEBUG
        if(filterDefinition.WorldId != Id)
            throw new NotSupportedException($"{nameof(FilterDefinition)}.WorldId != World.Id: {filterDefinition.WorldId} != {_world.Id}");
#endif

        var filterById = _filters[filterId];

        return new Filter<TMask>(filterById);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static FilterInstance<TMask> FindOrCreateFilter(IndexedWorld<TMask> world, FilterDefinitionBase filterDefinition)
    {
        ref var filters = ref world._filters;
        var filter = world._module.FetchOrCreateFilter(
            world,
            world.Id,
            filterDefinition,
            filters,
            world._settings.MaxAllowedUniqueComponentsCount,
            (filterId, mask, w) => CreateFilter(filterId, mask, w),
            (filter, lastAddedComponentIndex) => ScanEntities(filter, lastAddedComponentIndex));

        if(filter.Id != filterDefinition.FilterId)
            ThrowHelper.ThrowNotSupportedException($"Filter id {filter.Id} does not match filter definition id {filterDefinition.FilterId}");

        if (filterDefinition.FilterId >= filters.Length)
        {
            Array.Resize(ref filters, filters.Length * 2);
        }

        filters[filterDefinition.FilterId] = filter;

        return filter;
    }

    private static FilterInstance<TMask> CreateFilter(int filterId, TMask mask, object worldObject)
    {
        var world = (IndexedWorld<TMask>)worldObject;
        var settings = world._settings;

        var filter = new FilterInstance<TMask>(
            filterId,
            settings.MaxEntitiesCount,
            settings.MaxEntitiesPerFilter,
            mask,
            world);

        return filter;
    }

    private static void ScanEntities(FilterInstance<TMask> filter, int lastAddedGlobalComponentIndex)
    {
        var filterMask = filter.Mask;

        var world = filter.World;
        var lastAddedComponents = world.GetComponentCollectionByGlobalIndex(lastAddedGlobalComponentIndex);
        var entities = lastAddedComponents.Entities;
        var entityMasks = world._entityMasks;
        for (int i = 0; i < entities.Length; i++)
        {
            var entityId = entities[i];
            if (entityMasks[entityId.Index].Has(filterMask))
            {
                filter.AddEntity(entityId);
            }
        }
    }

    public bool HasFilterComponent<T>(Filter<TMask> filter)
        where T : struct
    {
        #if DEBUG
        if (filter.World.Id != Id)
            throw new NotSupportedException($"Filter {filter.Id} belongs to World '{filter.World.Id}. This World is '{Id}'");
        #endif

        var localIndex = _module.GetLocalIndex(ComponentMetadata<T>.GlobalIndex);
        return filter.Mask.GetBit(localIndex);
    }

    public void InitNameFeature() => _world.InitNameFeature();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NameFeature GetNameFeature() => _world.GetNameFeature();

    public bool HasComponent<T>(EntityId entityId) where T : struct, IComponent => _world.HasComponent<T>(entityId);

    public ref T GetComponent<T>(EntityId entityId) where T : struct, IComponent => ref _world.GetComponent<T>(entityId);

    public bool TryGetComponent<T>(EntityId entityId, out ComponentRef<T> componentRef)
        where T : struct, IComponent
        => _world.TryGetComponent<T>(entityId, out componentRef);

    public ref T AddComponent<T>(EntityId entityId)
        where T : struct, IComponent
    {
        var componentGlobalIndex = ComponentMetadata<T>.GlobalIndex;
        ref var component = ref GetSparseSet<T>(componentGlobalIndex).AddComponent(entityId);

        var localIndex = _module.GetLocalIndex(componentGlobalIndex);
        var filters = _module.GetFilters(componentGlobalIndex);

#if HEAVY_FILTERS_ENABLED

        var heavyFilters = _heavyFilterModule.GetHeavyFilters(componentGlobalIndex);
        OnComponentAdded(entityId, localIndex, filters, heavyFilters);

#else
        OnComponentAdded(entityId, localIndex, filters);
#endif

        return ref component;
    }

    private void AddComponents<T>(DenseSet<T> denseSet)
        where T : struct, IComponent
    {
        var componentGlobalIndex = ComponentMetadata<T>.GlobalIndex;
        var localIndex = _module.GetLocalIndex(componentGlobalIndex);
        var filters = _module.GetFilters(componentGlobalIndex);

        var sparseSet = GetSparseSet<T>(componentGlobalIndex);
        sparseSet.AddComponents(denseSet);

#if HEAVY_FILTERS_ENABLED
        var heavyFilters = _heavyFilterModule.GetHeavyFilters(componentGlobalIndex);
#endif

        foreach (var entityId in denseSet.Entities)
        {
#if HEAVY_FILTERS_ENABLED
            OnComponentAdded(entityId, localIndex, filters, heavyFilters);
#else
            OnComponentAdded(entityId, localIndex, filters);
#endif
        }
    }

    public void DeleteComponent<T>(EntityId entityId)
        where T : struct, IComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        var replacedEntityId = GetSparseSet<T>(globalIndex).SwapAndPopComponent(entityId);

        var localIndex = _module.GetLocalIndex(globalIndex);
        var filters = _module.GetFilters(globalIndex);

#if HEAVY_FILTERS_ENABLED

        var heavyFilters = _heavyFilterModule.GetHeavyFilters(globalIndex);
        OnComponentDeleted(entityId, replacedEntityId, globalIndex, localIndex, filters, heavyFilters);

#else
        OnComponentDeleted(entityId, localIndex, filters);
#endif
    }

    private void DeleteComponents<T>(List<EntityId> deletedEntities)
        where T : struct, IComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        var localIndex = _module.GetLocalIndex(globalIndex);
        var filters = _module.GetFilters(globalIndex);

        var sparseSet = GetSparseSet<T>(globalIndex);

#if HEAVY_FILTERS_ENABLED
        var heavyFilters = _heavyFilterModule.GetHeavyFilters(globalIndex);
#endif

        foreach (var deletedEntity in deletedEntities)
        {
            var replacedEntityId = sparseSet.SwapAndPopComponent(deletedEntity);

#if HEAVY_FILTERS_ENABLED
            OnComponentDeleted(deletedEntity, replacedEntityId, globalIndex, localIndex, filters, heavyFilters);
#else
            OnComponentDeleted(deletedEntity, localIndex, filters);
#endif
        }
    }

    internal void OnComponentAdded(EntityId entityId, int localIndex, List<FilterInstance<TMask>> filters)
    {
        ref var entityMask = ref _entityMasks[entityId.Index];
        entityMask.SetBit(localIndex);

        for (var index = 0; index < filters.Count; index++)
        {
            var filter = filters[index];
            if (entityMask.Has(filter.Mask))
            {
                filter.AddEntity(entityId);
            }
        }
    }

    internal void OnComponentDeleted(EntityId entityId, int localIndex, List<FilterInstance<TMask>> filters)
    {
        ref var entityMask = ref _entityMasks[entityId.Index];
        var oldEntityMask = entityMask;
        entityMask.ClearBit(localIndex);

        for (var index = 0; index < filters.Count; index++)
        {
            var filter = filters[index];
            if (oldEntityMask.Has(filter.Mask))
            {
                filter.DeleteEntity(entityId);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetLocalIndex(int componentGlobalIndex) => _module.GetLocalIndex(componentGlobalIndex);

    public void InitCommandBufferCache<T>(int capacity = 10) where T : struct => _world.InitCommandBufferCache<T>(capacity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CommandBuffer<T> GetCommandBufferFor<T>(int capacity = 10) where T : struct, IComponent
        => _world.GetCommandBufferFor<T>(capacity, this);

    void ICommandBufferApplier.Apply<T>(CommandBuffer<T> buffer)
    {
        if(!buffer.IsInitialized)
            throw new NotSupportedException("Buffer is not initialized correctly!");

        DeleteComponents<T>(buffer.DeletedEntities);
        AddComponents(buffer.AddedComponents);

        buffer.DeletedEntities.Clear();
        buffer.AddedEntities.Clear();
        buffer.AddedComponents.Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasSingleton<T>() where T : struct, ISingletonComponent => _world.HasSingleton<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetSingleton<T>() where T : struct, ISingletonComponent => ref _world.GetSingleton<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSingleton<T>(out ComponentRef<T> componentRef) where T : struct, ISingletonComponent =>
        _world.TryGetSingleton(out componentRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSingleton<T>(ref T component) where T : struct, ISingletonComponent =>
        _world.TryGetSingleton(ref component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddSingleton<T>() where T : struct, ISingletonComponent => ref _world.AddSingleton<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteSingleton<T>() where T : struct, ISingletonComponent => _world.DeleteSingleton<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent =>
        _world.HasStaticBuffer<T>(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticBuffer<T> GetStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent =>
        _world.GetStaticBuffer<T>(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticBuffer<T> AddStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent =>
        _world.AddStaticBuffer<T>(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent =>
        _world.DeleteStaticBuffer<T>(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DynamicBufferCollection<T> GetDynamicBuffers<T>() where T : struct, IDynamicBufferComponent =>
        _world.GetDynamicBuffers<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent =>
        _world.HasDynamicBuffer<T>(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DynamicBuffer<T> GetDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent =>
        _world.GetDynamicBuffer<T>(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetDynamicBuffer<T>(EntityId entityId, out DynamicBuffer<T> buffer) where T : struct, IDynamicBufferComponent =>
        _world.TryGetDynamicBuffer<T>(entityId, out buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DynamicBuffer<T> AddDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent =>
        _world.AddDynamicBuffer<T>(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent =>
        _world.DeleteDynamicBuffer<T>(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasTag<T>(EntityId entityId) where T : struct, ITagComponent => _world.HasTag<T>(entityId);

    public void AddTag<T>(EntityId entityId) where T : struct, ITagComponent
    {
        var componentGlobalIndex = ComponentMetadata<T>.GlobalIndex;
        GetTagSparseSet<T>(componentGlobalIndex).AddComponent(entityId);

        var localIndex = _module.GetLocalIndex(componentGlobalIndex);
        var filters = _module.GetFilters(componentGlobalIndex);

#if HEAVY_FILTERS_ENABLED

        var heavyFilters = _heavyFilterModule.GetHeavyFilters(componentGlobalIndex);
        OnComponentAdded(entityId, localIndex, filters, heavyFilters);

#else
        OnComponentAdded(entityId, localIndex, filters);
#endif
    }

    public void DeleteTag<T>(EntityId entityId) where T : struct, ITagComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        var replacedEntityId = GetTagSparseSet<T>(globalIndex).SwapAndPopComponent(entityId);

        var localIndex = _module.GetLocalIndex(globalIndex);
        var filters = _module.GetFilters(globalIndex);

#if HEAVY_FILTERS_ENABLED

        var heavyFilters = _heavyFilterModule.GetHeavyFilters(globalIndex);
        OnComponentDeleted(entityId, replacedEntityId, globalIndex, localIndex, filters, heavyFilters);

#else
        OnComponentDeleted(entityId, localIndex, filters);
#endif
    }
}