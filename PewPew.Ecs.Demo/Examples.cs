using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Features;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid;
using PewPew.Ecs.Hybrid.Internals;

namespace PewPew.Ecs.Demo;

public static class Examples
{
    public static void Example0()
    {
// init world
        World world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<Position>();
        world.InitComponent<Speed>();

// build entities
        EntityId playerId = world.CreateEntityId();

        ref Position position = ref world.AddComponent<Position>(playerId);
        position.Vector = new Vector3(1, 2, 3);

        ref Speed speed = ref world.AddComponent<Speed>(playerId);
        speed.Vector = new Vector3(1, 1, 1);

// update components
        world.ExecuteQuery((EntityId entityId, ref Position position, ref Speed speed) => position.Vector += speed.Vector);
    }

    public static void Example1()
    {
        World settingsWorld = WorldFactory.Shared.CreateWorld();
        settingsWorld.InitSingleton<HealthRegenSettings>();

        World actorsWorld = WorldFactory.Shared.CreateWorld();
        actorsWorld.InitComponent<Health>();
        actorsWorld.InitComponent<KilledBy>();
        actorsWorld.InitTag<HealthRegenActive>();
        actorsWorld.InitStaticBuffer<Damage>();

        CustomState state = new(settingsWorld, actorsWorld);
    }

    public static void Example2()
    {
        // see HealthRegenSystem
        // see DamageSystem
    }
    
    public static void Example3()
    {
        // as mask you can use BitMask64, BitMask128 or BitMask256 structures
        IndexedWorld<BitMask64> world = WorldFactory.Shared.CreateIndexedWorld<BitMask64>();

        FilterDefinition filterDefinition = new FilterDefinition().With<Position>().With<Speed>().WithTag<Alive>();

        Filter<BitMask64> filter = world.GetFilter(filterDefinition);

        foreach (EntityId entityId in filter.Entities)
        {
            ref Position position = ref world.GetComponent<Position>(entityId);
            ref Speed speed = ref world.GetComponent<Speed>(entityId);

            position.Vector += speed.Vector;
        }
    }

    public static void Example4()
    {
        HybridWorld world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetypeWithTag<Position, Speed, Health, Alive>();
        world.InitStaticArchetypeWithTag<Position, Speed, Health, Dead>();

        var alivePlayers = world.GetStaticArchetypeWithTag<Position, Speed, Health, Alive>();
        var deadPlayers = world.GetStaticArchetypeWithTag<Position, Speed, Health, Dead>();

        for (var index = alivePlayers.Entities.Length - 1; index >= 0; index--)
        {
            var entityId = alivePlayers.Entities[index];
            Health health = alivePlayers.GetComponent3(entityId);
            if (health.Current <= 0)
            {
                alivePlayers.MoveEntityTo(entityId, deadPlayers);
            }
        }

        var alivePlayerDefinition = new FilterDefinition().With<Position>().With<Speed>().WithTag<Alive>();

        world.ExecuteQueryWithoutId<MovementQueryWithoutId, Position, Speed>(alivePlayerDefinition, default);
    }

    public static void Example5()
    {
        HybridWorld world = WorldFactory.Shared.CreateHybridWorld();
        world.InitStaticArchetypeWithTag<Position, Speed, Alive>();

        var alivePlayerDefinition = new FilterDefinition().With<Position>().With<Speed>().WithTag<Alive>();

        world.ExecuteBatchQuery<MovementBatchQuery, Position, Speed>(alivePlayerDefinition, default);
    }

    public static void AllFilterPosibilities()
    {
        IndexedWorld world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Position>();
        world.InitComponent<Speed>();

// filter examples, supports reverse order of components
        var filterDefinition = new FilterDefinition().With<Position>().With<Speed>();
        world.WarmUpFilter(filterDefinition); // optional

        world.ExecuteQuery(filterDefinition, (EntityId entityId, ref Position position, ref Speed speed) =>
        {
            // direct order of components with entity id
            // ...
        });

        world.ExecuteQuery(filterDefinition, (EntityId entityId, ref Speed speed, ref Position position) =>
        {
            // reverse order of components with entity id
            // ...
        });

        world.ExecuteQuery<MovementQuery, Position, Speed>(filterDefinition, default);

        world.ExecuteQuery<ReverseMovementQuery, Speed, Position>(filterDefinition, default);

        foreach (var entityId in world.GetEntities(filterDefinition))
        {
            ref var position = ref world.GetComponent<Position>(entityId);
            ref var speed = ref world.GetComponent<Speed>(entityId);
            // ...
        }

// non filter examples

        world.ExecuteQuery((EntityId entityId, ref Position position, ref Speed speed) =>
        {
            // order of components is important, table scanning will be produced by the last component
            // ...
        });

        world.ExecuteQuery((EntityId entityId, ref Speed speed, ref Position position) =>
        {
            // order of components is important, table scanning will be produced by the last component
            // ...
        });

        world.ExecuteQuery<MovementQuery, Position, Speed>(default);

        world.ExecuteQuery<ReverseMovementQuery, Speed, Position>(default);

// heavy filter examples

        var heavyFilterDefinition = new HeavyFilterDefinition<Position, Speed>();

        world.ExecuteQuery<MovementQuery, Position, Speed>(heavyFilterDefinition, default);

        world.ExecuteQuery(heavyFilterDefinition, (EntityId entityId, ref Position position, ref Speed speed) =>
        {
            // direct order of components with entity id
            // ...
        });

        foreach (var entityId in world.GetEntities(heavyFilterDefinition))
        {
            ref var position = ref world.GetComponent<Position>(entityId);
            ref var speed = ref world.GetComponent<Speed>(entityId);
            // ...
        }

// reverse order of components is not supported
// world.ExecuteQuery<ReverseMovementQuery, Speed, Position>(heavyFilterDefinition, default);

// reverse order of components is not supported
// world.ExecuteQuery(heavyFilterDefinition, (EntityId entityId, ref Speed speed, ref Position position) =>
// {
//     // reverse order of components
//     // ...
// });
    }

    public readonly struct MovementQuery : IQuery<Position, Speed>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(EntityId entityId, ref Position position, ref Speed speed) { }
    }

    public readonly record struct ReverseMovementQuery : IQuery<Speed, Position>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(EntityId entityId, ref Speed speed, ref Position position)
        {
            // ...
        }
    }

    private static CustomState InitState()
    {
        World settingsWorld = WorldFactory.Shared.CreateWorld();
        settingsWorld.InitSingleton<HealthRegenSettings>();

        World actorsWorld = WorldFactory.Shared.CreateWorld();
        actorsWorld.InitComponent<Health>();
        actorsWorld.InitTag<HealthRegenActive>();

        IndexedWorld indexedWorld = WorldFactory.Shared.CreateIndexedWorld();
        indexedWorld.InitComponent<Health>();
        indexedWorld.InitTag<HealthRegenActive>();

        var customState = new CustomState(settingsWorld, actorsWorld);

        return customState;
    }

    public readonly record struct HealthRegenQuery(HealthRegenSettings Settings) : IQueryWithTag<Health, HealthRegenActive>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(EntityId entityId, ref Health health) => health.Current = Math.Min(health.Current + Settings.RegenPerTick, health.Max);
    }

    public static void Example6()
    {
        World world = WorldFactory.Shared.CreateWorld();

        ComponentCollection<Position> positions = world.GetComponents<Position>();

// call Clear() when you want to remove all components, the fastest way btw
        positions.Clear();

// or

// components can only be removed in reverse order while iterating over them
        for (int i = positions.Count - 1; i >= 0; i--)
        {
            if (i % 2 == 0)
                continue;

            EntityId entityId = positions.Entities[i];
            positions.DeleteComponent(entityId);
        }

// or

// just use command buffer
        using CommandBuffer<Position> commandBuffer = world.GetCommandBufferFor<Position>();
        for (int i = 0; i < positions.Count; i++)
        {
            if (i % 2 == 0)
                continue;

            commandBuffer.QueueDeleteComponent(positions.Entities[i]);
        }
    }

    public static void Example7()
    {
        World world = WorldFactory.Shared.CreateWorld();
        world.InitNameFeature();

        NameFeature nameFeature = world.GetNameFeature();
        nameFeature.WorldName = "Main World";
        EntityId firstEntity = nameFeature.CreateEntityId("entity #1");

        EntityId secondEntity = world.CreateEntityId();
        nameFeature.SetName(secondEntity, "entity #2");

        EntityId getByName = nameFeature.GetEntityId("entity #1");

        // firstEntity == getByName // true
    }

    public static void SimpleSparseSet()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<Health>();
        world.InitComponent<KilledBy>();

        world.InitSingleton<HealthRegenSettings>();
        world.InitTag<HealthRegenActive>();
        world.InitStaticBuffer<Damage>();

        var systemCollection = new SystemCollection();

        systemCollection.Add(new HealthRegenSystem());

        var state = new State
        {
            World1 = world,
        };
        systemCollection.Update(state);
    }

    public static void SharedWorldFactory()
    {
        var world = WorldFactory.Shared.CreateWorld();

        world.InitComponent<Health>();

        var systemCollection = new SystemCollection();

        systemCollection.Add(new HealthRegenSystem());

        var state = new State
        {
            World1 = world,
        };
        systemCollection.Update(state);
    }

    public static void SimpleNonCachedFilter()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<Position>();
        world.InitComponent<Speed>();

        var entityId = world.CreateEntityId();
        world.GetComponents<Position>().AddComponent(entityId);
        world.GetComponents<Speed>().AddComponent(entityId);

        var systemCollection = new SystemCollection();

        systemCollection.Add(new PositionSystem());

        var state = new State
        {
            World1 = world,
        };
        systemCollection.Update(state);
    }

    public static void SimpleCachedFilter()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Position>();
        world.InitComponent<Speed>();

        var entityId = world.CreateEntityId();
        world.GetComponents<Position>().AddComponent(entityId);
        world.GetComponents<Speed>().AddComponent(entityId);

        var systemCollection = new SystemCollection();

        systemCollection.Add(new CachedPositionSystem());

        var state = new State
        {
            World2 = world,
        };
        systemCollection.Update(state);
    }

    public static void SimpleCommandBuffer()
    {
        var world = WorldFactory.Shared.CreateIndexedWorld();
        world.InitComponent<Position>();
        world.InitComponent<Speed>();
        world.InitComponent<AliveComponent>();

        var entityId = world.CreateEntityId();
        world.GetComponents<Position>().AddComponent(entityId);
        world.GetComponents<Speed>().AddComponent(entityId);
        world.GetComponents<AliveComponent>().AddComponent(entityId);

        var systemCollection = new SystemCollection();

        systemCollection.Add(new CommandBufferPositionSystem());

        var state = new State
        {
            World2 = world,
        };
        systemCollection.Update(state);
    }

    public static void SimpleStaticArchetype()
    {
        var world = WorldFactory.Shared.CreateHybridWorld();

        world.InitStaticArchetype<Position, Speed>();
        world.InitStaticArchetype<Health, Speed>();

        StaticArchetype<BitMask64, Position, Speed> archetype = world.GetStaticArchetype<Position, Speed>();
        var entityId = world.CreateEntityId();
        ComponentRef<Position, Speed> componentRef = archetype.Add(entityId);

        archetype.ExecuteQueryWithoutId<BitMask64, MovementQueryWithoutId, Position, Speed>(default);
    }

    public readonly struct MovementQueryWithoutId : IQueryWithoutId<Position, Speed>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(ref Position position, ref Speed speed)
        {
            position.Vector += speed.Vector;
        }
    }

    public readonly struct MovementBatchQuery : IBatchQuery<Position, Speed>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BatchUpdate(Span<Position> positions, Span<Speed> speeds)
        {
            Span<float> floatPositions = MemoryMarshal.Cast<Position, float>(positions);
            Span<float> floatSpeeds = MemoryMarshal.Cast<Speed, float>(speeds);

            int length = floatPositions.Length - floatPositions.Length % 8;

            Span<Vector256<float>> ints = MemoryMarshal.Cast<float, Vector256<float>>(floatPositions.Slice(0, length));
            Span<Vector256<float>> a = MemoryMarshal.Cast<float, Vector256<float>>(floatSpeeds.Slice(0, length));

            for (int i = 0; i < ints.Length; i++)
                ints[i] += a[i];

            for (int i = length; i < floatPositions.Length; i++)
                floatPositions[i] += floatSpeeds[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SparseUpdate(ref Position position, ref Speed speed)
        {
            position.Vector += speed.Vector;
        }
    }

    public static void CustomArchetype()
    {
        HybridWorld world = WorldFactory.Shared.CreateHybridWorld();
        world.InitPlayerArchetype();

        EntityId entityId = world.CreateEntityId();

        PlayerArchetype playerArchetype = world.GetPlayerArchetype();
        Player player = playerArchetype.Add(entityId);
        player.Position.Vector = new Vector3(42);
    }
}

public readonly ref struct PlayerArchetype
{
    private readonly StaticArchetype<BitMask64, Position, Speed> _archetype;

    public PlayerArchetype(StaticArchetype<BitMask64, Position, Speed> archetype)
    {
        _archetype = archetype;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Player Get(EntityId entityId)
    {
        var index = _archetype.GetRequiredIndexUnsafe(entityId);

        return new Player(
            ref _archetype.GetComponent1Unsafe(index),
            ref _archetype.GetComponent2Unsafe(index));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref Position GetPosition(EntityId entityId) => ref _archetype.GetComponent1(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref Speed GetSpeed(EntityId entityId) => ref _archetype.GetComponent2(entityId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(EntityId entityId, Player player)
    {
        _archetype.Add(entityId, player.Position, player.Speed);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Player Add(EntityId entityId)
    {
        var player = _archetype.Add(entityId);

        return new Player(ref player.Component1, ref player.Component2);
    }
}

public ref struct Player
{
    public Player(ref Position position, ref Speed speed)
    {
        Position = ref position;
        Speed = ref speed;
    }

    public ref Position Position;
    public ref Speed Speed;
}

public static class ExampleHybridWorldExtensions
{
    public static void InitPlayerArchetype(this HybridWorld world)
    {
        world.InitStaticArchetype<Position, Speed>();
    }

    public static PlayerArchetype GetPlayerArchetype(this HybridWorld world)
    {
        var archetypeRef = world.GetStaticArchetype<Position, Speed>();

        return new PlayerArchetype(archetypeRef);
    }
}