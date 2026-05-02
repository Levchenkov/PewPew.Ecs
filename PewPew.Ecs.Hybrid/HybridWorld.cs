using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Internals;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.Hybrid;

public sealed class HybridWorld
#if BIT_MASK_64_DEFAULT
    : HybridWorld<BitMask64>
#elif BIT_MASK_128_DEFAULT
    : HybridWorld<BitMask128>
#elif BIT_MASK_256_DEFAULT
    : HybridWorld<BitMask256>
#endif
{
    internal HybridWorld(ushort id = 0) : base(id)
    {
    }

    internal HybridWorld(ushort id, HybridWorldSettings settings) : base(id, settings)
    {
    }
}

public partial class HybridWorld<TMask> :
    IHybridComponentsProvider<TMask>,
    IComponentsInitializer,
    IEntityManager,
    ICommandBufferApplier
    where TMask : struct, IBitMask<TMask>
{
    private const int MaxAllowedUniqueComponentsCount = 64; // max performance with this limit

    private readonly HybridWorldSettings _settings;

    private readonly World _world;
    private readonly FilterModule<TMask, HybridFilterInstance<TMask>>  _module;
    internal readonly Dictionary<Type, IStaticArchetypeInstance<TMask>> _typeToArchetypeMap = new();
    private readonly Dictionary<int, Dictionary<Type, object>> _universalKeyToPartsMap = new();
    private readonly TMask[] _entityToMaskMap;
    private readonly IStaticArchetypeInstance<TMask>?[] _entityToArchetypeMap;
    private readonly ArchetypeDenseIndexPair[] _entityIndexToArchetypeDenseIndexMap;

    private HybridFilterInstance<TMask>[] _filters;
    private bool _isAnyFilterCreated;

    public HybridWorld(ushort id = 0) : this(id, new HybridWorldSettings { MaxEntitiesPerArchetype = MaxAllowedUniqueComponentsCount })
    {
    }

    public HybridWorld(ushort id, HybridWorldSettings settings)
    {
        _settings = settings;

        _world = new World(id, settings);
        _module = new FilterModule<TMask, HybridFilterInstance<TMask>>(settings.MaxAllowedUniqueComponentsCount);
        _entityToMaskMap = new TMask[settings.MaxEntitiesCount];
        _entityToArchetypeMap = new IStaticArchetypeInstance<TMask>[settings.MaxEntitiesCount];
        _entityIndexToArchetypeDenseIndexMap = new ArchetypeDenseIndexPair[settings.MaxEntitiesCount];

        _filters = new HybridFilterInstance<TMask>[10];


        new Span<ArchetypeDenseIndexPair>(_entityIndexToArchetypeDenseIndexMap).Fill(ArchetypeDenseIndexPair.Invalid);
    }

    public ushort Id => _world.Id;

    public int EntityCapacity => _world.EntityCapacity;

    public Span<EntityId> Entities => _world.Entities;

    public void InitStaticArchetype<T1, T2>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        => InitStaticArchetype<T1, T2>(_settings.MaxEntitiesPerArchetype);

    public void InitStaticArchetype<T1, T2>(int maxComponentsPerArchetype)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        if (!_world.IsComponentInitialized<T1>())
        {
            InitComponent<T1>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T2>())
        {
            InitComponent<T2>(_settings.MaxComponentsPerSet);
        }

        InitStaticArchetype<T1, T2>(
            maxComponentsPerArchetype,
            typeof(StaticArchetypeInstance<TMask, T1, T2>),
            new()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
            });
    }

    public void InitStaticArchetype<T1, T2, T3>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        => InitStaticArchetype<T1, T2, T3>(_settings.MaxEntitiesPerArchetype);

    public void InitStaticArchetype<T1, T2, T3>(int maxComponentsPerArchetype)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        if (!_world.IsComponentInitialized<T1>())
        {
            InitComponent<T1>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T2>())
        {
            InitComponent<T2>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T3>())
        {
            InitComponent<T3>(_settings.MaxComponentsPerSet);
        }

        InitStaticArchetype<T1, T2, T3>(
            maxComponentsPerArchetype,
            typeof(StaticArchetypeInstance<TMask, T1, T2, T3>),
            new()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
            });
    }

    public void InitStaticArchetype<T1, T2, T3, T4>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        => InitStaticArchetype<T1, T2, T3, T4>(_settings.MaxEntitiesPerArchetype);

    public void InitStaticArchetype<T1, T2, T3, T4>(int maxComponentsPerArchetype)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
    {
        if (!_world.IsComponentInitialized<T1>())
        {
            InitComponent<T1>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T2>())
        {
            InitComponent<T2>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T3>())
        {
            InitComponent<T3>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T4>())
        {
            InitComponent<T4>(_settings.MaxComponentsPerSet);
        }

        InitStaticArchetype<T1, T2, T3, T4>(
            maxComponentsPerArchetype,
            typeof(StaticArchetypeInstance<TMask, T1, T2, T3, T4>),
            new()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
            });
    }

    public void InitStaticArchetype<T1, T2, T3, T4, T5>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        => InitStaticArchetype<T1, T2, T3, T4, T5>(_settings.MaxEntitiesPerArchetype);

    public void InitStaticArchetype<T1, T2, T3, T4, T5>(int maxComponentsPerArchetype)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
    {
        if (!_world.IsComponentInitialized<T1>())
        {
            InitComponent<T1>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T2>())
        {
            InitComponent<T2>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T3>())
        {
            InitComponent<T3>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T4>())
        {
            InitComponent<T4>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T5>())
        {
            InitComponent<T5>(_settings.MaxComponentsPerSet);
        }

        InitStaticArchetype<T1, T2, T3, T4, T5>(
            maxComponentsPerArchetype,
            typeof(StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5>),
            new()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
                ComponentMetadata<T5>.GlobalIndex,
            });
    }

    public void InitStaticArchetype<T1, T2, T3, T4, T5, T6>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
        => InitStaticArchetype<T1, T2, T3, T4, T5, T6>(_settings.MaxEntitiesPerArchetype);

    public void InitStaticArchetype<T1, T2, T3, T4, T5, T6>(int maxComponentsPerArchetype)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
    {
        if (!_world.IsComponentInitialized<T1>())
        {
            InitComponent<T1>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T2>())
        {
            InitComponent<T2>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T3>())
        {
            InitComponent<T3>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T4>())
        {
            InitComponent<T4>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T5>())
        {
            InitComponent<T5>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T6>())
        {
            InitComponent<T6>(_settings.MaxComponentsPerSet);
        }

        InitStaticArchetype<T1, T2, T3, T4, T5, T6>(
            maxComponentsPerArchetype,
            typeof(StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>),
            new()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
                ComponentMetadata<T5>.GlobalIndex,
                ComponentMetadata<T6>.GlobalIndex,
            });
    }

    public void InitStaticArchetypeWithTag<T1, T2, T3>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, ITagComponent
    {
        InitStaticArchetypeWithTag<T1, T2, T3>(_settings.MaxEntitiesPerArchetype);
    }

    public void InitStaticArchetypeWithTag<T1, T2, T3>(int maxComponentsPerArchetype)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, ITagComponent
    {
        if (!_world.IsComponentInitialized<T1>())
        {
            InitComponent<T1>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T2>())
        {
            InitComponent<T2>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T3>())
        {
            InitTag<T3>(_settings.MaxComponentsPerSet);
        }

        InitStaticArchetype<T1, T2>(
            maxComponentsPerArchetype,
            typeof(StaticArchetypeInstanceWithTag<TMask, T1, T2, T3>),
            new()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
            });
    }

    public void InitStaticArchetypeWithTag<T1, T2, T3, T4>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, ITagComponent
    {
        InitStaticArchetypeWithTag<T1, T2, T3, T4>(_settings.MaxEntitiesPerArchetype);
    }

    public void InitStaticArchetypeWithTag<T1, T2, T3, T4>(int maxComponentsPerArchetype)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, ITagComponent
    {
        if (!_world.IsComponentInitialized<T1>())
        {
            InitComponent<T1>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T2>())
        {
            InitComponent<T2>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T3>())
        {
            InitComponent<T3>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T4>())
        {
            InitTag<T4>(_settings.MaxComponentsPerSet);
        }

        InitStaticArchetype<T1, T2, T3>(
            maxComponentsPerArchetype,
            typeof(StaticArchetypeInstanceWithTag<TMask, T1, T2, T3, T4>),
            new()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
            });
    }

    public void InitStaticArchetypeWithTag<T1, T2, T3, T4, T5>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, ITagComponent
    {
        InitStaticArchetypeWithTag<T1, T2, T3, T4, T5>(_settings.MaxEntitiesPerArchetype);
    }

    public void InitStaticArchetypeWithTag<T1, T2, T3, T4, T5>(int maxComponentsPerArchetype)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, ITagComponent
    {
        if (!_world.IsComponentInitialized<T1>())
        {
            InitComponent<T1>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T2>())
        {
            InitComponent<T2>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T3>())
        {
            InitComponent<T3>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T4>())
        {
            InitComponent<T4>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T5>())
        {
            InitTag<T5>(_settings.MaxComponentsPerSet);
        }

        InitStaticArchetype<T1, T2, T3, T4>(
            maxComponentsPerArchetype,
            typeof(StaticArchetypeInstanceWithTag<TMask, T1, T2, T3, T4, T5>),
            new()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
                ComponentMetadata<T5>.GlobalIndex,
            });
    }

    public void InitStaticArchetypeWithTag<T1, T2, T3, T4, T5, T6>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, ITagComponent
    {
        InitStaticArchetypeWithTag<T1, T2, T3, T4, T5, T6>(_settings.MaxEntitiesPerArchetype);
    }

    public void InitStaticArchetypeWithTag<T1, T2, T3, T4, T5, T6>(int maxComponentsPerArchetype)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, ITagComponent
    {
        if (!_world.IsComponentInitialized<T1>())
        {
            InitComponent<T1>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T2>())
        {
            InitComponent<T2>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T3>())
        {
            InitComponent<T3>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T4>())
        {
            InitComponent<T4>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T5>())
        {
            InitComponent<T5>(_settings.MaxComponentsPerSet);
        }

        if (!_world.IsComponentInitialized<T6>())
        {
            InitTag<T6>(_settings.MaxComponentsPerSet);
        }

        InitStaticArchetype<T1, T2, T3, T4, T5>(
            maxComponentsPerArchetype,
            typeof(StaticArchetypeInstanceWithTag<TMask, T1, T2, T3, T4, T5, T6>),
            new()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
                ComponentMetadata<T5>.GlobalIndex,
                ComponentMetadata<T6>.GlobalIndex,
            });
    }

    public StaticArchetype<TMask, T1, T2> GetStaticArchetype<T1, T2>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var key = typeof(StaticArchetypeInstance<TMask, T1, T2>);

        return GetStaticArchetype<T1, T2>(
            key,
            () => new List<int>()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
            });
    }

    public StaticArchetype<TMask, T1, T2, T3> GetStaticArchetype<T1, T2, T3>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        var key = typeof(StaticArchetypeInstance<TMask, T1, T2, T3>);

        return GetStaticArchetype<T1, T2, T3>(
            key,
            () => new List<int>()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
            });
    }

    public StaticArchetype<TMask, T1, T2, T3, T4> GetStaticArchetype<T1, T2, T3, T4>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
    {
        var key = typeof(StaticArchetypeInstance<TMask, T1, T2, T3, T4>);

        return GetStaticArchetype<T1, T2, T3, T4>(
            key,
            () => new List<int>()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
            });
    }

    public StaticArchetype<TMask, T1, T2, T3, T4, T5> GetStaticArchetype<T1, T2, T3, T4, T5>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
    {
        var key = typeof(StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5>);

        return GetStaticArchetype<T1, T2, T3, T4, T5>(
            key,
            () => new List<int>()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
                ComponentMetadata<T5>.GlobalIndex,
            });
    }

    public StaticArchetype<TMask, T1, T2, T3, T4, T5, T6> GetStaticArchetype<T1, T2, T3, T4, T5, T6>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
    {
        var key = typeof(StaticArchetypeInstance<TMask, T1, T2, T3, T4, T5, T6>);

        return GetStaticArchetype<T1, T2, T3, T4, T5, T6>(
            key,
            () => new List<int>()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
                ComponentMetadata<T5>.GlobalIndex,
                ComponentMetadata<T6>.GlobalIndex,
            });
    }

    public StaticArchetype<TMask, T1, T2> GetStaticArchetypeWithTag<T1, T2, T3>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, ITagComponent
    {
        var key = typeof(StaticArchetypeInstanceWithTag<TMask, T1, T2, T3>);

        return GetStaticArchetype<T1, T2>(
            key,
            () => new List<int>()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
            });
    }

    public StaticArchetype<TMask, T1, T2, T3> GetStaticArchetypeWithTag<T1, T2, T3, T4>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, ITagComponent
    {
        var key = typeof(StaticArchetypeInstanceWithTag<TMask, T1, T2, T3, T4>);

        return GetStaticArchetype<T1, T2, T3>(
            key,
            () => new List<int>()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
            });
    }

    public StaticArchetype<TMask, T1, T2, T3, T4> GetStaticArchetypeWithTag<T1, T2, T3, T4, T5>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, ITagComponent
    {
        var key = typeof(StaticArchetypeInstanceWithTag<TMask, T1, T2, T3, T4, T5>);

        return GetStaticArchetype<T1, T2, T3, T4>(
            key,
            () => new List<int>()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
                ComponentMetadata<T5>.GlobalIndex,
            });
    }

    public StaticArchetype<TMask, T1, T2, T3, T4, T5> GetStaticArchetypeWithTag<T1, T2, T3, T4, T5, T6>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, ITagComponent
    {
        var key = typeof(StaticArchetypeInstanceWithTag<TMask, T1, T2, T3, T4, T5, T6>);

        return GetStaticArchetype<T1, T2, T3, T4, T5>(
            key,
            () => new List<int>()
            {
                ComponentMetadata<T1>.GlobalIndex,
                ComponentMetadata<T2>.GlobalIndex,
                ComponentMetadata<T3>.GlobalIndex,
                ComponentMetadata<T4>.GlobalIndex,
                ComponentMetadata<T5>.GlobalIndex,
                ComponentMetadata<T6>.GlobalIndex,
            });
    }

    public ComponentRef<T1, T2> AddStaticArchetype<T1, T2>(EntityId entityId)
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        var staticArchetype = GetStaticArchetype<T1, T2>();
        return staticArchetype.Add(entityId);
    }

    public bool HasFilterComponent<T>(HybridFilter<TMask> filter)
        where T : struct
    {
#if DEBUG
        if (filter.World.Id != Id)
            throw new NotSupportedException($"Filter {filter.Id} belongs to World '{filter.World.Id}. This World is '{Id}'");
#endif

        var localIndex = _module.GetLocalIndex(ComponentMetadata<T>.GlobalIndex);
        return filter.Mask.GetBit(localIndex);
    }

    public HybridFilter<TMask> GetFilter(FilterDefinitionBase filterDefinition)
    {
        var filterId = filterDefinition.FilterId;
        if (filterId == FilterDefinitionBase.InvalidId)
        {
            var filter = FindOrCreateFilter(this, filterDefinition);

            return new HybridFilter<TMask>(filter);
        }

#if DEBUG
        if(filterDefinition.WorldId != Id)
            throw new NotSupportedException($"{nameof(FilterDefinition)}.WorldId != World.Id: {filterDefinition.WorldId} != {_world.Id}");
#endif

        var filterById = _filters[filterId];

        return new HybridFilter<TMask>(filterById);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static HybridFilterInstance<TMask> FindOrCreateFilter(HybridWorld<TMask> world, FilterDefinitionBase filterDefinition)
    {
        world._isAnyFilterCreated = true;
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

    private static HybridFilterInstance<TMask> CreateFilter(int filterId, TMask mask, object worldObject)
    {
        var world = (HybridWorld<TMask>)worldObject;
        var settings = world._settings;

        List<IStaticArchetypeInstance<TMask>> fullArchetypes = new ();
        foreach (var (_, staticArchetype) in world._typeToArchetypeMap)
        {
            // archetype is [A, B, C], filter is [A, B] or archetype is [A, B] and filter is [A, B]
            if (staticArchetype.Mask.Has(mask))
            {
                fullArchetypes.Add(staticArchetype);
            }
        }

        var filter = new HybridFilterInstance<TMask>(
            filterId,
            settings.MaxEntitiesCount,
            settings.MaxEntitiesPerFilter,
            mask,
            world,
            fullArchetypes.ToArray());

        return filter;
    }

    private static void ScanEntities(HybridFilterInstance<TMask> filter, int lastAddedGlobalComponentIndex)
    {
        var filterMask = filter.Mask;

        var world = filter.World;
        var lastAddedComponents = world.GetComponentCollectionByGlobalIndex(lastAddedGlobalComponentIndex);
        var entities = lastAddedComponents.Entities;
        var entityMasks = world._entityToMaskMap;
        var worldEntityToArchetypeMap = world._entityToArchetypeMap;
        for (int i = 0; i < entities.Length; i++)
        {
            var entityId = entities[i];
            var archetype = worldEntityToArchetypeMap[entityId.Index];
            if(archetype != null)
                continue;

            if (entityMasks[entityId.Index].Has(filterMask))
            {
                filter.AddSparseEntity(entityId);
            }
        }
    }

    public void InitComponent<T>() where T : struct, IComponent => InitComponent<T>(_settings.MaxComponentsPerSet);

    public void InitComponent<T>(int maxComponentsPerSet) where T : struct, IComponent
    {
        var sparseSet = new CompactSparseSet<T>(_settings.MaxEntitiesCount, maxComponentsPerSet, _world.ResizeStrategy, _world);
        _world.InitComponent<T>(sparseSet);
        _module.InitComponent<T>();
    }

    public void InitTag<T>() where T : struct, ITagComponent
    {
        InitTag<T>(_settings.MaxComponentsPerSet);
    }

    public void InitTag<T>(int maxComponentsPerSet) where T : struct, ITagComponent
    {
        var tagSparseSet = new CompactTagSparseSet<T>(_settings.MaxEntitiesCount, maxComponentsPerSet, _world.ResizeStrategy, _world);
        _world.InitTag<T>(tagSparseSet);
        _module.InitTag<T>();
    }

    public void InitSingleton<T>() where T : struct, ISingletonComponent
    {
        _world.InitSingleton<T>();
    }

    public void InitStaticBuffer<T>() where T : struct, IStaticBufferComponent
        => InitStaticBuffer<T>(_settings.MaxComponentsPerSet / _settings.MaxElementsCountPerSet, _settings.MaxElementsCountPerSet);

    public void InitStaticBuffer<T>(int maxComponentsPerSet, int maxElementsCount) where T : struct, IStaticBufferComponent
    {
        var staticBufferSet = new CompactStaticBufferSet<T>(_settings.MaxEntitiesCount, maxComponentsPerSet, maxElementsCount, _world.ResizeStrategy, _world);
        _world.InitStaticBuffer<T>(staticBufferSet);
    }

    public void InitDynamicBuffer<T>() where T : struct, IDynamicBufferComponent
        => InitDynamicBuffer<T>(_settings.MaxComponentsPerSet, _settings.InitialDynamicBufferCapacity);

    public void InitDynamicBuffer<T>(int maxComponentsPerSet, int initialCapacity) where T : struct, IDynamicBufferComponent
    {
        var dynamicBufferSet = new CompactDynamicBufferSet<T>(_settings.MaxEntitiesCount, maxComponentsPerSet, initialCapacity, _world.ResizeStrategy, _world);
        _world.InitDynamicBuffer<T>(dynamicBufferSet);
    }

    public EntityId CreateEntityId()
    {
        return _world.CreateEntityId();
    }

    public void DeleteEntityId(EntityId entityId)
    {
        foreach (var filter in _filters)
        {
            if (filter == null)
                break;

            filter.DeleteEntity(entityId);
        }

        foreach (var (_, staticArchetype) in _typeToArchetypeMap)
        {
            staticArchetype.DeleteEntity(entityId);
        }

        _world.DeleteEntityId(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAlive(EntityId entityId)
    {
        return _world.IsAlive(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasMask(EntityId entityId, TMask mask)
    {
        var entityMask = _entityToMaskMap[entityId.Index];
        return entityMask.Has(mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CompactSparseSet<T> GetSparseSet<T>(int globalIndex) where T : struct, IComponent
        => (CompactSparseSet<T>)_world.GetCollectionByGlobalIndex<T>(globalIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CompactTagSparseSet<T> GetTagSparseSet<T>(int index) where T : struct, ITagComponent
        =>  (CompactTagSparseSet<T>)_world.GetCollectionByGlobalIndex<T>(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CompactSparseSet<T> GetSparseSet<T>() where T : struct, IComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = GetSparseSet<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CompactTagSparseSet<T> GetTagSparseSet<T>()
        where T : struct, ITagComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = GetTagSparseSet<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CompactStaticBufferSet<T> GetStaticBufferSet<T>()
        where T : struct, IStaticBufferComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = (CompactStaticBufferSet<T>)_world.GetCollectionByGlobalIndex<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CompactDynamicBufferSet<T> GetDynamicBufferSet<T>()
        where T : struct, IDynamicBufferComponent
    {
        var index = ComponentMetadata<T>.GlobalIndex;
        var set = (CompactDynamicBufferSet<T>)_world.GetCollectionByGlobalIndex<T>(index);

        return set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IComponentCollection GetComponentCollectionByGlobalIndex(int index)
        => (IComponentCollection)_world.GetCollectionByGlobalIndex(index);

    public bool HasComponent<T>(EntityId entityId) where T : struct, IComponent
    {
        var entityMask = _entityToMaskMap[entityId.Index];
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        var localIndex = _module.GetLocalIndex(globalIndex);

        return entityMask.GetBit(localIndex);
    }

    public ref T GetComponent<T>(EntityId entityId) where T : struct, IComponent
    {
        var archetype = _entityToArchetypeMap[entityId.Index];
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        var localIndex = _module.GetLocalIndex(globalIndex);

        if (archetype == null || !archetype.Mask.GetBit(localIndex))
        {
            return ref GetSparseSet<T>(globalIndex).GetComponent(entityId);
        }

        return ref archetype.GetComponent<T>(entityId);
    }

    public bool TryGetComponent<T>(EntityId entityId, out ComponentRef<T> componentRef) where T : struct, IComponent
    {
        var archetype = _entityToArchetypeMap[entityId.Index];
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        var localIndex = _module.GetLocalIndex(globalIndex);

        if (archetype == null || !archetype.Mask.GetBit(localIndex))
        {
            return GetSparseSet<T>(globalIndex).TryGetComponent(entityId, out componentRef);
        }

        return archetype.TryGetComponent<T>(entityId, out componentRef);
    }

    public ref T AddComponent<T>(EntityId entityId) where T : struct, IComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;

        ValidateNoStaticArchetype(entityId);

        ref var component = ref GetSparseSet<T>().AddComponent(entityId);

        var filters = _module.GetFilters(globalIndex);
        var localIndex = _module.GetLocalIndex(globalIndex);

        OnComponentAdded(entityId, localIndex, filters);

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

        foreach (var entityId in denseSet.Entities)
        {
            ValidateNoStaticArchetype(entityId);

            OnComponentAdded(entityId, localIndex, filters);
        }
    }

    public void DeleteComponent<T>(EntityId entityId) where T : struct, IComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;

        ValidateNoStaticArchetype(entityId);

        GetSparseSet<T>().DeleteComponent(entityId);

        var filters = _module.GetFilters(globalIndex);
        var localIndex = _module.GetLocalIndex(globalIndex);

        OnComponentDeleted(entityId, localIndex, filters);
    }

    private void DeleteComponents<T>(List<EntityId> deletedEntities)
        where T : struct, IComponent
    {
        var globalIndex = ComponentMetadata<T>.GlobalIndex;
        var localIndex = _module.GetLocalIndex(globalIndex);
        var filters = _module.GetFilters(globalIndex);

        var sparseSet = GetSparseSet<T>(globalIndex);

        foreach (var deletedEntity in deletedEntities)
        {
            ValidateNoStaticArchetype(deletedEntity);

            sparseSet.SwapAndPopComponent(deletedEntity);

            OnComponentDeleted(deletedEntity, localIndex, filters);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasTag<T>(EntityId entityId) where T : struct, ITagComponent
        => GetTagSparseSet<T>().HasComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddTag<T>(EntityId entityId) where T : struct, ITagComponent
        => GetTagSparseSet<T>().AddComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteTag<T>(EntityId entityId) where T : struct, ITagComponent
        => GetTagSparseSet<T>().DeleteComponent(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasSingleton<T>() where T : struct, ISingletonComponent => _world.HasSingleton<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetSingleton<T>() where T : struct, ISingletonComponent => ref _world.GetSingleton<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSingleton<T>(out ComponentRef<T> componentRef) where T : struct, ISingletonComponent
        => _world.TryGetSingleton(out componentRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSingleton<T>(ref T component) where T : struct, ISingletonComponent
        => _world.TryGetSingleton(ref component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddSingleton<T>() where T : struct, ISingletonComponent => ref _world.AddSingleton<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteSingleton<T>() where T : struct, ISingletonComponent => _world.DeleteSingleton<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent
        => GetStaticBufferSet<T>().HasBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticBuffer<T> GetStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent
        => GetStaticBufferSet<T>().GetBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetStaticBuffer<T>(EntityId entityId, out StaticBuffer<T> staticBuffer)  where T : struct, IStaticBufferComponent
        => GetStaticBufferSet<T>().TryGetComponent(entityId, out staticBuffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticBuffer<T> AddStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent
        => GetStaticBufferSet<T>().AddBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent
        => GetStaticBufferSet<T>().DeleteBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent
        => GetDynamicBufferSet<T>().HasBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DynamicBuffer<T> GetDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent
        => GetDynamicBufferSet<T>().GetBuffer(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetDynamicBuffer<T>(EntityId entityId, out DynamicBuffer<T> buffer) where T : struct, IDynamicBufferComponent
        => GetDynamicBufferSet<T>().TryGetBuffer(entityId, out buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DynamicBuffer<T> AddDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent
    {
        ValidateNoStaticArchetype(entityId);
        return GetDynamicBufferSet<T>().AddBuffer(entityId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent
    {
        ValidateNoStaticArchetype(entityId);
        GetDynamicBufferSet<T>().DeleteBuffer(entityId);
    }

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
    private bool TryGetArchetype(EntityId entityId, out IStaticArchetypeInstance<TMask> archetype)
    {
        archetype = _entityToArchetypeMap[entityId.Index];

        if (archetype == null)
            return false;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool HasArchetype(EntityId entityId)
    {
        return _entityToArchetypeMap[entityId.Index] != null;
    }

    internal void OnComponentAdded(EntityId entityId, int localIndex, List<HybridFilterInstance<TMask>> filters)
    {
        ref var entityMask = ref _entityToMaskMap[entityId.Index];
        entityMask.SetBit(localIndex);

        for (var index = 0; index < filters.Count; index++)
        {
            var filter = filters[index];
            var filterMask = filter.Mask;
            if (entityMask.Has(filterMask))
            {
                filter.AddSparseEntity(entityId);
            }
        }
    }

    internal void OnComponentAddedToArchetype(EntityId entityId, TMask mask, IStaticArchetypeInstance<TMask> archetype)
    {
        var index = entityId.Index;
        ref var entityMask = ref _entityToMaskMap[index];
        entityMask.SetBits(mask);
        _entityToArchetypeMap[index] = archetype;
    }

    internal void OnComponentDeleted(EntityId entityId, int localIndex, List<HybridFilterInstance<TMask>> filters)
    {
        ref var entityMask = ref _entityToMaskMap[entityId.Index];
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

    internal void OnComponentDeletedFromArchetype(EntityId entityId, TMask mask)
    {
        var index = entityId.Index;
        ref var entityMask = ref _entityToMaskMap[index];
        entityMask.ClearBits(mask);
        _entityToArchetypeMap[index] = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetLocalIndex(int componentGlobalIndex) => _module.GetLocalIndex(componentGlobalIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ValidateNoArchetypeAndNoComponents(EntityId entityId, IStaticArchetypeInstance<TMask> staticArchetype)
    {
        var index = entityId.Index;
        var entityArchetype = _entityToArchetypeMap[index];
        if (entityArchetype != null)
        {
            if (entityArchetype == staticArchetype)
                return;

            ThrowHelper.ThrowNotSupportedException(
                $"Static archetype {entityArchetype.GetType()} has this entity {entityId}. " +
                $"To move this entity to another static archetype, the entity must be removed from this one.");
        }

        var entityMask = _entityToMaskMap[index];
        if (!entityMask.Equals(default))
        {
            ThrowHelper.ThrowNotSupportedException($"Entity {entityId} has components. Only empty entity can be added to archetype.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ValidateNoStaticArchetype(EntityId entityId)
    {
        var index = entityId.Index;
        var entityArchetype = _entityToArchetypeMap[index];
        if (entityArchetype != null)
        {
            ThrowHelper.ThrowNotSupportedException(
                $"Static archetype {entityArchetype.GetType()} has this entity {entityId}. To add a new component delete entity from static archetype explicitly.");
        }
    }
}