# PewPew.Ecs

### What is PewPew.Ecs?

ECS framework-agnostic library implemented on Sparse Set data structure and compile-time static archetype.
It has different component types and allows split state on a few worlds.

### Quickstart

```csharp
// Example#0
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
```

### Where can I get it?

``` dotnet add package PewPew.Ecs ```

### Features
- data locality
- non alloc
- multi worlds per state.
- Component types:
  - Regular
  - Singleton
  - Tag
  - Static Buffer
- Non-indexed filters
- Indexed filters
- Static Archetypes
  - SIMD
- Command buffer

### Examples

```csharp
// Example#1 - Use custom multi-world state with different component types!
World settingsWorld = WorldFactory.Shared.CreateWorld();
settingsWorld.InitSingleton<HealthRegenSettings>();

World actorsWorld = WorldFactory.Shared.CreateWorld();
actorsWorld.InitComponent<Health>();
actorsWorld.InitComponent<KilledBy>();
actorsWorld.InitTag<HealthRegenActive>();
actorsWorld.InitStaticBuffer<Damage>();

CustomState state = new(settingsWorld, actorsWorld);

public record struct CustomState(World SettingsWorld, World ActorsWorld);

public struct Health : IComponent
{
    public int Current;
    public int Max;
}

public struct HealthRegenActive : ITagComponent
{
}

public struct HealthRegenSettings : ISingletonComponent
{
    public int RegenPerTick;
}

public struct Damage : IStaticBufferComponent
{
    public int Value;
    public EntityId DamageDealerId;
}

public struct KilledBy : IComponent
{
    public EntityId KilledByActorId;
}
```
```csharp
// Example#2 - Define systems as you want!
public sealed class HealthRegenSystem
{
    public void Update(CustomState state)
    {
        HealthRegenSettings healthRegenSettings = state.SettingsWorld.GetSingleton<HealthRegenSettings>();

        ComponentSetRef<Health> healths = state.ActorsWorld.GetComponents<Health>();
        Span<EntityId> entitiesToRegen = state.ActorsWorld.GetTags<HealthRegenTag>().Entities;
        for (int i = 0; i < entitiesToRegen.Length; i++)
        {
            EntityId entity = entitiesToRegen[i];
            ref Health health = ref healths.GetComponent(entity);
            health.Current = Math.Min(health.Current + healthRegenSettings.RegenPerTick, health.Max);
        }
    }
}

public sealed class DamageSystem
{
    public void Update(CustomState state)
    {
        ComponentCollection<Health> healths = state.ActorsWorld.GetComponents<Health>();
        StaticBufferCollection<Damage> damages = state.ActorsWorld.GetStaticBuffers<Damage>();

        for (int i = 0; i < damages.BufferCount; i++)
        {
            EntityId damagedEntityId = damages.Entities[i];
            StaticBuffer<Damage> damageBuffer = damages.GetBuffer(damagedEntityId);

            ref Health health = ref healths.GetComponent(damagedEntityId);

            for (int j = 0; j < damageBuffer.Count; j++)
            {
                Damage damage = damageBuffer.Components[j];
                health.Current -= damage.Value;

                if (health.Current <= 0)
                {
                    state.ActorsWorld.AddComponent<KilledBy>(damagedEntityId).KilledByActorId = damage.DamageDealerId;
                    break;
                }
            }

            damageBuffer.Clear();
        }
    }
}
```
```csharp
// Example#3 - Use filters with BitMask64, BitMask128 or BitMask256 masks!
IndexedWorld<BitMask64> world = WorldFactory.Shared.CreateIndexedWorld<BitMask64>();

FilterDefinition filterDefinition = new FilterDefinition().With<Position>().With<Speed>().WithTag<Alive>();

Filter<BitMask64> filter = world.GetFilter(filterDefinition);

foreach (EntityId entityId in filter.Entities)
{
    ref Position position = ref world.GetComponent<Position>(entityId);
    ref Speed speed = ref world.GetComponent<Speed>(entityId);

    position.Vector += speed.Vector;
}
```
```csharp
// Example#4 - Use compile-time static archetypes!
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

public readonly struct MovementQueryWithoutId : IQueryWithoutId<Position, Speed>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(ref Position position, ref Speed speed)
    {
        position.Vector += speed.Vector;
    }
}
```
```csharp
// Example#5 - Use SIMD!
HybridWorld world = WorldFactory.Shared.CreateHybridWorld();
world.InitStaticArchetypeWithTag<Position, Speed, Alive>();

var alivePlayerDefinition = new FilterDefinition().With<Position>().With<Speed>().WithTag<Alive>();

world.ExecuteBatchQuery<MovementBatchQuery, Position, Speed>(alivePlayerDefinition, default);

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
```
```csharp
// Example#6 Use custom static archetypes!
HybridWorld world = WorldFactory.Shared.CreateHybridWorld();
world.InitPlayerArchetype();

EntityId entityId = world.CreateEntityId();

PlayerArchetype playerArchetype = world.GetPlayerArchetype();
Player player = playerArchetype.Add(entityId);
player.Position.Vector = new Vector3(42);

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
```
```csharp
// Example#7 - Use lambda delegates for filtering.
IndexedWorld world = WorldFactory.Shared.CreateIndexedWorld();
world.InitComponent<Position>();
world.InitComponent<Speed>();

var filterDefinition = new FilterDefinition().With<Position>().With<Speed>();

world.ExecuteQuery(filterDefinition, (EntityId entityId, ref Position position, ref Speed speed) =>
{
    position.Vector += speed.Vector;
});

// or struct for filtering

world.ExecuteQuery<MovementQuery, Position, Speed>(filterDefinition, default);

public readonly struct MovementQuery : IQuery<Position, Speed>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(EntityId entityId, ref Position position, ref Speed speed)
    {
        position.Vector += speed.Vector;
    }
}
```
```csharp
// Example#8 Use deffered command buffers!
World world = WorldFactory.Shared.CreateWorld();

ComponentCollection<Position> positions = world.GetComponents<Position>();

using CommandBuffer<Position> commandBuffer = world.GetCommandBufferFor<Position>();
for (int i = 0; i < positions.Count; i++)
{
    if (i % 2 == 0)
        continue;

    commandBuffer.QueueDeleteComponent(positions.Entities[i]);
}
```