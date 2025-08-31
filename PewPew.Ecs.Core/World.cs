using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PewPew.Ecs.Core.Features;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public sealed class World :
    IComponentsProvider,
    IComponentsInitializer,
    IEntityManager,
    ICommandBufferApplier
{
    private object[] _setMap;
    private object[] _setCollection;
    private object[] _featureMap;
    private object[] _commandBufferCacheMap;
    private readonly EntityIdPool _entityIdPool;
    private IResizeStrategy _resizeStrategy = new DoubleSizeStrategy(minSize: 10);
    private readonly WorldSettings _settings;

    private int _setCount;
    private int _maskableComponentCount;

    public ushort Id { get; }

    public int MaxUniqueComponentsCount => _settings.MaxAllowedUniqueComponentsCount;

    protected internal IReadOnlyList<object> SetMap => _setMap;

    public int EntityCapacity => _entityIdPool.Capacity;

    public Span<EntityId> Entities => _entityIdPool.Values;

    internal IResizeStrategy ResizeStrategy => _resizeStrategy;

    internal World(ushort id = 0) : this(id, new WorldSettings())
    {
    }

    internal World(ushort id, WorldSettings settings)
    {
        Id = id;
        _settings = settings;
        _setMap = new object[settings.MaxAllowedUniqueComponentsCount];
        _setCollection = new object[settings.MaxAllowedUniqueComponentsCount];
        _commandBufferCacheMap = new object[settings.MaxAllowedUniqueComponentsCount];

        _entityIdPool = new EntityIdPool(Id, _settings.MaxEntitiesCount);

        _featureMap = new object[10];
    }

    public void SetResizeStrategy(IResizeStrategy resizeStrategy) => _resizeStrategy = resizeStrategy;

    public void InitComponent<T>()
        where T : struct, IComponent
        => InitComponent<T>(_settings.MaxComponentsPerSet);

    public void InitComponent<T>(int maxComponentsPerSet)
        where T : struct, IComponent
    {
        var sparseSet = new SparseSet<T>(_settings.MaxEntitiesCount, maxComponentsPerSet, _resizeStrategy, _entityIdPool);

        InitComponent<T>(sparseSet);
    }

    internal void InitComponent<T>(object sparseSet)
        where T : struct, IComponent
    {
        AddMaskableComponentSetFor<T>(sparseSet);
    }

    public void InitSingleton<T>()
        where T : struct, ISingletonComponent
    {
        var singletonStorage = new SingletonStorage<T>();

        InitSingleton<T>(singletonStorage);
    }

    internal void InitSingleton<T>(object singletonStorage)
        where T : struct
    {
        AddComponentSetFor<T>(singletonStorage);
    }

    public void InitTag<T>()
        where T : struct, ITagComponent
        => InitTag<T>(_settings.MaxComponentsPerSet);

    public void InitTag<T>(int maxComponentsPerSet)
        where T : struct, ITagComponent
    {
        var tagSparseSet = new TagSparseSet<T>(_settings.MaxEntitiesCount, maxComponentsPerSet, _resizeStrategy, _entityIdPool);

        InitTag<T>(tagSparseSet);
    }

    internal void InitTag<T>(object tagSparseSet)
        where T : struct, ITagComponent
    {
        AddMaskableComponentSetFor<T>(tagSparseSet);
    }

    public void InitStaticBuffer<T>()
        where T : struct, IStaticBufferComponent =>
        InitStaticBuffer<T>(_settings.MaxComponentsPerSet / _settings.MaxElementsCountPerSet, _settings.MaxElementsCountPerSet);

    public void InitStaticBuffer<T>(int maxComponentsPerType, int maxElementsCount)
        where T : struct, IStaticBufferComponent
    {
        var staticBufferSet = new StaticBufferSet<T>(_settings.MaxEntitiesCount, maxComponentsPerType, maxElementsCount, _resizeStrategy, _entityIdPool);

        InitStaticBuffer<T>(staticBufferSet);
    }

    internal void InitStaticBuffer<T>(object staticBufferSet)
        where T : struct, IStaticBufferComponent
    {
        AddComponentSetFor<T>(staticBufferSet);
    }

    private void AddMaskableComponentSetFor<T>(object set)
        where T : struct
    {
        if (IsComponentInitialized<T>())
            throw new NotSupportedException($"Component {typeof(T).Name} has already been initialized in the world #{Id} !");

        if (_maskableComponentCount == _settings.MaxAllowedUniqueComponentsCount)
            throw new Exception($"Component {typeof(T).Name} can't be initialized due max capacity '{_settings.MaxAllowedUniqueComponentsCount}' reached.");

        _maskableComponentCount++;

        ComponentMetadata<T>.InitComponentMetadata();

        AddSet<T>(set);
    }

    private void AddSet<T>(object set)
        where T : struct
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        if(index >= _setMap.Length)
            Array.Resize(ref _setMap, Math.Max(_setMap.Length << 1, ComponentMetadata.TotalCount));

        _setMap[index] = set;

        if(index >= _commandBufferCacheMap.Length)
            Array.Resize(ref _commandBufferCacheMap, Math.Max(_commandBufferCacheMap.Length << 1, ComponentMetadata.TotalCount));

        if(_setCount == _setCollection.Length)
            Array.Resize(ref _setCollection, _setCount << 1);

        _setCollection[_setCount++] = set;
    }

    private void AddComponentSetFor<T>(object set)
        where T : struct
    {
        if (IsComponentInitialized<T>())
            throw new NotSupportedException($"Component {typeof(T).Name} has already been registered in the world #{Id} !");

        ComponentMetadata<T>.InitComponentMetadata();

        AddSet<T>(set);
    }

    public void InitNameFeature()
    {
        InitFeature(new NameFeature.Data());
    }

    private void InitFeature<TFeatureData>(TFeatureData feature)
    {
        if (IsFeatureInitialized<TFeatureData>())
            throw new NotSupportedException($"Feature {typeof(TFeatureData).Name} has already been registered in the world #{Id} !");

        if (FeatureMetadata<TFeatureData>.Index == FeatureMetadata.InvalidIndex)
        {
            FeatureMetadata<TFeatureData>.Index = FeatureMetadata.TotalCount++;
        }

        AddFeature<TFeatureData>(feature);
    }

    private void AddFeature<T>(object feature)
    {
        var index = FeatureMetadata<T>.Index;

        if(index >= _featureMap.Length)
            Array.Resize(ref _featureMap, _featureMap.Length << 1);

        _featureMap[index] = feature;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentCollection<T> GetComponents<T>()
        where T : struct, IComponent
    {
        var set = GetSparseSet<T>();

        return new ComponentCollection<T>(set);
    }

    public TagCollection<T> GetTags<T>() where T : struct, ITagComponent
    {
        var tagSparseSet = GetTagSparseSet<T>();

        return new TagCollection<T>(tagSparseSet);
    }

    public StaticBufferCollection<T> GetStaticBuffers<T>() where T : struct, IStaticBufferComponent
    {
        var staticBufferSet = GetStaticBufferSet<T>();

        return new StaticBufferCollection<T>(staticBufferSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal SparseSet<T> GetSparseSet<T>()
        where T : struct, IComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = (SparseSet<T>)GetCollectionByGlobalIndex<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TagSparseSet<T> GetTagSparseSet<T>()
        where T : struct, ITagComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = (TagSparseSet<T>)GetCollectionByGlobalIndex<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal SingletonStorage<T> GetSingletonStorageInternal<T>()
        where T : struct, ISingletonComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = (SingletonStorage<T>)GetCollectionByGlobalIndex<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal StaticBufferSet<T> GetStaticBufferSet<T>()
        where T : struct, IStaticBufferComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = (StaticBufferSet<T>)GetCollectionByGlobalIndex<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal object GetCollectionByGlobalIndex<T>(int index)
        where T : struct
    {
#if DEBUG
        if (!IsComponentInitialized<T>())
            throw new Exception($"Component {typeof(T).Name} is not initialized.");
#endif

        var collection = _setMap[index];

        return collection;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal object GetCollectionByGlobalIndex(int index)
    {
#if DEBUG
        if (!IsComponentInitialized(index))
            throw new Exception($"Component with index {index} is not initialized.");
#endif

        var collection = _setMap[index];

        return collection;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent<T>(EntityId entityId)
        where T : struct, IComponent
    {
        var result = GetSparseSet<T>().HasComponent(entityId);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent<T>(EntityId entityId)
        where T : struct, IComponent
    {
        ref var result = ref GetSparseSet<T>().GetComponent(entityId);

        return ref result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent<T>(EntityId entityId, out ComponentRef<T> componentRef)
        where T : struct, IComponent
    {
        var result = GetSparseSet<T>().TryGetComponent(entityId, out componentRef);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddComponent<T>(EntityId entityId)
        where T : struct, IComponent
    {
        ref var component = ref GetSparseSet<T>().AddComponent(entityId);

        return ref component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteComponent<T>(EntityId entityId)
        where T : struct, IComponent
    {
        GetSparseSet<T>().SwapAndPopComponent(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasSingleton<T>() where T : struct, ISingletonComponent => GetSingletonStorageInternal<T>().HasComponent();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetSingleton<T>()
        where T : struct, ISingletonComponent
    {
        var singletonStorage = GetSingletonStorageInternal<T>();
        ref var component = ref singletonStorage.GetComponent();

        return ref component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSingleton<T>(out ComponentRef<T> componentRef)
        where T : struct, ISingletonComponent
    {
        var singletonStorage = GetSingletonStorageInternal<T>();

        return singletonStorage.TryGetComponent(out componentRef);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSingleton<T>(ref T component)
        where T : struct, ISingletonComponent
    {
        var singletonStorage = GetSingletonStorageInternal<T>();

        return singletonStorage.TryGetComponent(ref component);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddSingleton<T>()
        where T : struct, ISingletonComponent
    {
        var singletonStorage = GetSingletonStorageInternal<T>();
        ref var component = ref singletonStorage.AddComponent();

        return ref component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteSingleton<T>()
        where T : struct, ISingletonComponent
    {
        var singletonStorage = GetSingletonStorageInternal<T>();
        singletonStorage.DeleteComponent();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasTag<T>(EntityId entityId) where T : struct, ITagComponent => GetTagSparseSet<T>().HasComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddTag<T>(EntityId entityId)
        where T : struct, ITagComponent
    {
        var tagSparseSet = GetTagSparseSet<T>();
        tagSparseSet.AddComponent(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteTag<T>(EntityId entityId)
        where T : struct, ITagComponent
    {
        var tagSparseSet = GetTagSparseSet<T>();
        tagSparseSet.DeleteComponent(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasStaticBuffer<T>(EntityId entityId)
        where T : struct, IStaticBufferComponent
    {
        return GetStaticBufferSet<T>().HasBuffer(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticBuffer<T> GetStaticBuffer<T>(EntityId entityId)
        where T : struct, IStaticBufferComponent
    {
        return GetStaticBufferSet<T>().GetBuffer(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticBuffer<T> AddStaticBuffer<T>(EntityId entityId)
        where T : struct, IStaticBufferComponent
    {
        return GetStaticBufferSet<T>().AddBuffer(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteStaticBuffer<T>(EntityId entityId)
        where T : struct, IStaticBufferComponent
    {
        GetStaticBufferSet<T>().DeleteBuffer(entityId);
    }

    public EntityId CreateEntityId() => _entityIdPool.Get();

    public void DeleteEntityId(EntityId entityId)
    {
        for (int i = 0; i < _setCount; i++)
        {
            var collection = _setCollection[i];
            if (collection is IComponentCollection componentCollection)
            {
                componentCollection.DeleteComponent(entityId);
            }
        }

        _entityIdPool.Return(entityId);
    }

    public bool IsAlive(EntityId entityId) => _entityIdPool.IsAlive(entityId);

    public NameFeature GetNameFeature()
    {
        var data = GetFeature<NameFeature.Data>();

        return new NameFeature(_entityIdPool, data);
    }

    private TFeature GetFeature<TFeature>()
    {
#if DEBUG
        if (!IsFeatureInitialized<TFeature>())
            throw new Exception($"Feature {typeof(TFeature).Name} is not initialized.");
#endif

        return (TFeature)_featureMap[FeatureMetadata<TFeature>.Index];
    }

    public bool IsComponentInitialized<TComponent>()
        where TComponent : struct
    {
        var globalIndex = ComponentMetadata<TComponent>.GlobalIndex;

        return IsComponentInitialized(globalIndex);
    }

    private bool IsComponentInitialized(int globalIndex)
    {
        return globalIndex != ComponentMetadata.InvalidIndex
               && globalIndex < _setMap.Length
               && _setMap[globalIndex] != null!;
    }

    private bool IsFeatureInitialized<TFeature>()
    {
        return FeatureMetadata<TFeature>.Index != FeatureMetadata.InvalidIndex
               && FeatureMetadata<TFeature>.Index < _featureMap.Length
               && _featureMap[FeatureMetadata<TFeature>.Index] != null!;
    }

    public void InitCommandBufferCache<T>(int capacity = 10)
        where T : struct
    {
        if(!IsComponentInitialized<T>())
            throw new NotSupportedException($"Component {typeof(T).Name} is not initialized!");

        ref var cache = ref _commandBufferCacheMap[ComponentMetadata<T>.GlobalIndex];
        if (cache == null!)
        {
            cache = new CommandBufferCache<T>
            {
                AddedComponentsCache = new DenseSet<T>(capacity),
                DeletedEntitiesCache = new List<EntityId>(capacity),
                AddedEntities = new HashSet<EntityId>(capacity)
            };
        }
        else
        {
            var value = (CommandBufferCache<T>)cache;
            value.AddedComponentsCache.EnsureCapacity(capacity);
            value.DeletedEntitiesCache.Capacity = capacity;
            value.AddedEntities.EnsureCapacity(capacity);
        }
    }

    public CommandBuffer<T> GetCommandBufferFor<T>(int capacity = 10)
        where T : struct, IComponent
    {
        var buffer = GetCommandBufferFor<T>(capacity, this);

        return buffer;
    }

    internal CommandBuffer<T> GetCommandBufferFor<T>(int capacity, ICommandBufferApplier commandBufferApplier)
        where T : struct, IComponent
    {
        InitCommandBufferCache<T>(capacity);
        var cache = (CommandBufferCache<T>)_commandBufferCacheMap[ComponentMetadata<T>.GlobalIndex];

        var buffer = new CommandBuffer<T>(commandBufferApplier, _entityIdPool, cache.AddedComponentsCache, cache.AddedEntities, cache.DeletedEntitiesCache);

        return buffer;
    }

    void ICommandBufferApplier.Apply<T>(CommandBuffer<T> buffer)
    {
        if(!buffer.IsInitialized)
            throw new NotSupportedException("Buffer is not initialized correctly!");

        var sparseSet = GetSparseSet<T>();

        sparseSet.DeleteComponents(buffer.DeletedEntities);
        sparseSet.AddComponents(buffer.AddedComponents);

        buffer.DeletedEntities.Clear();
        buffer.AddedEntities.Clear();
        buffer.AddedComponents.Reset();
    }

    private class CommandBufferCache<T> where T : struct
    {
        public DenseSet<T> AddedComponentsCache;
        public HashSet<EntityId> AddedEntities;
        public List<EntityId> DeletedEntitiesCache;
    }
}