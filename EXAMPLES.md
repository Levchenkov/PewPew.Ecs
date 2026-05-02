# PewPew.Ecs Example Guide

This guide explains the runnable examples from `PewPew.Ecs.Demo/Examples.cs` and related demo files.

All snippets are based on the current demo code, so they match the public API used in this repository.

## Before you start

Most examples assume these namespaces:

```csharp
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid;
```

Important runtime rule: initialize every component, tag, singleton, static buffer or dynamic buffer before using it.

---

## Example 0 — Basic world and regular components

Use a plain `World` when you want sparse-set component storage without indexed filters.

```csharp
World world = WorldFactory.Shared.CreateWorld();
world.InitComponent<Position>();
world.InitComponent<Speed>();

EntityId playerId = world.CreateEntityId();

ref Position position = ref world.AddComponent<Position>(playerId);
position.Vector = new Vector3(1, 2, 3);

ref Speed speed = ref world.AddComponent<Speed>(playerId);
speed.Vector = new Vector3(1, 1, 1);

world.ExecuteQuery((EntityId entityId, ref Position position, ref Speed speed) =>
    position.Vector += speed.Vector);
```

Why this is useful:

- lowest-friction setup
- fast iteration over sparse-set storage
- good default when you do not need cached filters yet

---

## Example 1 — Split state across multiple worlds

A common pattern is to keep different parts of the game state in different worlds.

```csharp
World settingsWorld = WorldFactory.Shared.CreateWorld();
settingsWorld.InitSingleton<HealthRegenSettings>();

World actorsWorld = WorldFactory.Shared.CreateWorld();
actorsWorld.InitComponent<Health>();
actorsWorld.InitComponent<KilledBy>();
actorsWorld.InitTag<HealthRegenActive>();
actorsWorld.InitStaticBuffer<Damage>();

CustomState state = new(settingsWorld, actorsWorld);

public record struct CustomState(World SettingsWorld, World ActorsWorld);
```

Component kinds used here:

```csharp
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

When to use this pattern:

- configuration in one world, runtime actors in another
- check changes and send by network only for selected worlds
- isolated simulation layers

---

## Example 2 — Build systems around collections, tags and buffers

Systems can be plain classes. They do not need a custom framework lifecycle.

### Health regeneration system

```csharp
public sealed class HealthRegenSystem
{
    public void Update(CustomState state)
    {
        HealthRegenSettings healthRegenSettings = state.SettingsWorld.GetSingleton<HealthRegenSettings>();

        ComponentCollection<Health> healths = state.ActorsWorld.GetComponents<Health>();
        Span<EntityId> entitiesToRegen = state.ActorsWorld.GetTags<HealthRegenActive>().Entities;
        for (int i = 0; i < entitiesToRegen.Length; i++)
        {
            EntityId entity = entitiesToRegen[i];
            ref Health health = ref healths.GetComponent(entity);
            health.Current = Math.Min(health.Current + healthRegenSettings.RegenPerTick, health.Max);
        }
    }
}
```

### Damage processing system

```csharp
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

Notes:

- `GetTags<T>().Entities` gives a dense span of matching entities for a tag.
- `StaticBuffer<T>` is useful for per-entity append-only events such as incoming damage.
- Clearing a processed static buffer keeps ownership local to the entity.

---

## Example 3 — Indexed filters with cached membership

Use `IndexedWorld<TMask>` when you want filter membership to be updated incrementally.

```csharp
IndexedWorld<BitMask64> world = WorldFactory.Shared.CreateIndexedWorld<BitMask64>();

FilterDefinition filterDefinition = new FilterDefinition()
    .With<Position>()
    .With<Speed>()
    .WithTag<Alive>();

Filter<BitMask64> filter = world.GetFilter(filterDefinition);

foreach (EntityId entityId in filter.Entities)
{
    ref Position position = ref world.GetComponent<Position>(entityId);
    ref Speed speed = ref world.GetComponent<Speed>(entityId);

    position.Vector += speed.Vector;
}
```

Use indexed filters when:

- the same filter is executed many times
- component/tag membership changes incrementally over time
- you want predictable cached entity sets

Mask options:

- `BitMask64`
- `BitMask128`
- `BitMask256`

---

## Example 4 — Hybrid world with static archetypes

`HybridWorld` combines sparse storage with static archetypes.

```csharp
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

This is useful when:

- the structure of entities is known ahead of time — components are stored contiguously in memory, which reduces cache misses during iteration
- moving entities between static layouts is part of gameplay state changes
- you still want filter-based iteration on top of archetypes

Important: initialize static archetypes before creating hybrid filters.

---

## Example 5 — SIMD batch queries

Batch queries let you process contiguous archetype data in chunks and fall back to per-entity updates when needed.

```csharp
HybridWorld world = WorldFactory.Shared.CreateHybridWorld();
world.InitStaticArchetypeWithTag<Position, Speed, Alive>();

var alivePlayerDefinition = new FilterDefinition().With<Position>().With<Speed>().WithTag<Alive>();
world.ExecuteBatchQuery<MovementBatchQuery, Position, Speed>(alivePlayerDefinition, default);

public readonly struct MovementBatchQuery : IBatchQuery<Position, Speed>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BatchUpdate(Span<Position> positions, Span<Speed> speeds)
    {
        int vectorCount = Vector<float>.Count; // 8
        int length = positions.Length - positions.Length % vectorCount;

        Span<Vector<float>> positionVectors = MemoryMarshal.Cast<Position, Vector<float>>(positions.Slice(0, length));
        Span<Vector<float>> speedVectors = MemoryMarshal.Cast<Speed, Vector<float>>(speeds.Slice(0, length));

        for (int i = 0; i < positionVectors.Length; i++)
            positionVectors[i] += speedVectors[i];

        for (int i = length; i < positions.Length; i++)
            positions[i].Vector += speeds[i].Vector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SparseUpdate(ref Position position, ref Speed speed)
    {
        position.Vector += speed.Vector;
    }
}
```

Use this pattern when:

- you want to use vectorization for optimizations
- you are optimizing hot movement or math-heavy systems

---

## Example 6 — Source-generated custom archetypes

You can define your own archetype wrapper as a `ref struct` and let the source generator create the boilerplate.

### Runtime usage

```csharp
HybridWorld world = WorldFactory.Shared.CreateHybridWorld();
world.InitPlayerArchetype();

EntityId entityId = world.CreateEntityId();

PlayerArchetype playerArchetype = world.GetPlayerArchetype();
Player player = playerArchetype.Add(entityId);
player.Position.Vector = new Vector3(42);
```

### User-authored type

```csharp
[PewPew.Ecs.Hybrid.Archetype]
public ref struct Player
{
    public ref Position Position;
    public ref Speed Speed;

    public Player(ref Position position, ref Speed speed)
    {
        Position = ref position;
        Speed = ref speed;
    }
}
```

Generated API:

- `PlayerArchetype`
- `HybridWorld.InitPlayerArchetype()`
- `HybridWorld.GetPlayerArchetype()`

### Project setup

To enable source generation, reference the generator as an analyzer, as shown in `PewPew.Ecs.Demo/PewPew.Ecs.Demo.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\PewPew.Ecs.Hybrid.SourceGenerators\PewPew.Ecs.Hybrid.SourceGenerators.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

---

## Example 7 — Query execution styles for filters

The demo shows both lambda-based and struct-based query styles for indexed filters.

```csharp
IndexedWorld world = WorldFactory.Shared.CreateIndexedWorld();
world.InitComponent<Position>();
world.InitComponent<Speed>();

var filterDefinition = new FilterDefinition().With<Position>().With<Speed>();

world.ExecuteQuery(filterDefinition, (EntityId entityId, ref Position position, ref Speed speed) =>
{
    position.Vector += speed.Vector;
});

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

Which style to choose:

- lambda: for quic prototypes
- struct query: allows inline aggressively and most CPU efficient

---

## Example 8 — Deferred component deletion with command buffers

Use a command buffer when you need to modify a collection while iterating over it in forward order.

```csharp
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

Why this matters:

- you can't iterate and delete entities or components, so you can use deferred deletion

---

## Bonus — Entity names

The demo also includes an optional name feature.

```csharp
World world = WorldFactory.Shared.CreateWorld();
world.InitNameFeature();

NameFeature nameFeature = world.GetNameFeature();
nameFeature.WorldName = "Main World";
EntityId firstEntity = nameFeature.CreateEntityId("entity #1");

EntityId secondEntity = world.CreateEntityId();
nameFeature.SetName(secondEntity, "entity #2");

EntityId getByName = nameFeature.GetEntityId("entity #1");
```

This is useful for tooling, debugging, editor integration and readable logs.

---

## Example 9 — Blittable primitive wrappers (`BlittableBool`, `BlittableChar`)

ECS components must be blittable structs. `bool` and `char` are not blittable and cannot appear directly as component fields. Use the provided drop-in wrappers instead.

### Component definitions

```csharp
public struct UnitState : IComponent
{
    public BlittableBool IsAlive;  // wraps bool as byte
    public BlittableChar Grade;    // wraps char as ushort
}
```

### Usage

```csharp
World world = WorldFactory.Shared.CreateWorld();
world.InitComponent<UnitState>();

EntityId unitId = world.CreateEntityId();
ref UnitState state = ref world.AddComponent<UnitState>(unitId);

state.IsAlive = true;   // implicit bool → BlittableBool
state.Grade = 'A';      // implicit char → BlittableChar

bool isAlive = state.IsAlive;  // implicit BlittableBool → bool
char grade = state.Grade;      // implicit BlittableChar → char
```

Both types implicitly convert to and from their underlying primitive, so call sites look identical to raw `bool`/`char`.

---

## Example 10 — Inline UTF-8 strings in components (`BlittableString32`)

`BlittableString32` stores up to 32 UTF-8 bytes directly inside a component struct — no heap allocation, no handles. Use it for short, frequently-accessed labels such as entity tags, locale keys, or asset names.

### Component definition

```csharp
public struct NameComponent : IComponent
{
    public BlittableString32 Name;
}
```

### Setting a value

```csharp
World world = WorldFactory.Shared.CreateWorld();
world.InitComponent<NameComponent>();

EntityId entityId = world.CreateEntityId();
ref NameComponent name = ref world.AddComponent<NameComponent>(entityId);

if (BlittableString32.TryCreate("Player_1", out BlittableString32 value))
    name.Name = value;
```

`TryCreate` returns `false` when the UTF-8 byte count of the input string exceeds `BlittableString32.Capacity` (32). Use `Set(ReadOnlySpan<byte>)` for raw UTF-8 bytes.

### Reading a value

```csharp
// zero-copy span — no allocation
ReadOnlySpan<byte> bytes = name.Name.AsReadOnlySpan();

// allocating string — debug/logging only
string text = name.Name.ToString();
```

Prefer `AsReadOnlySpan()` for comparisons and hot-path iteration. `ToString()` is provided for debugging.

---

## Example 11 — Versioned string storage (`StringStorage`, `StringId`)

When strings are too long for `BlittableString32` or change over an entity's lifetime, keep them outside the component and refer to them by a versioned `StringId`. This keeps components blittable and avoids duplicating large strings per entity.

### Setup

```csharp
StringStorage strings = WorldFactory.Shared.CreateStringStorage();
```

### Adding and reading

```csharp
StringId id = strings.Add("some long description that exceeds 32 bytes...");
string value = strings.Get(id);
```

### Storing the handle in a component

```csharp
public struct DescriptionComponent : IComponent
{
    public StringId DescriptionId;
}

World world = WorldFactory.Shared.CreateWorld();
world.InitComponent<DescriptionComponent>();

EntityId entityId = world.CreateEntityId();
world.AddComponent<DescriptionComponent>(entityId).DescriptionId = strings.Add("hello world");

// later:
string description = strings.Get(world.GetComponent<DescriptionComponent>(entityId).DescriptionId);
```

### Update, delete, and stale handle detection

```csharp
strings.Update(id, "updated text");

strings.Delete(id);

// id is now stale — IsValid returns false, Get throws in DEBUG
bool live = strings.IsValid(id);     // false

// TryGet is a safe alternative to Get
if (strings.TryGet(id, out string? text))
    Console.WriteLine(text);
```

Slot indices are reused after deletion. The generation counter in `StringId` makes old handles stale automatically.

---

## Example 12 — Versioned generic object storage (`ObjectStorage<T>`, `ObjectId`)

`ObjectStorage<T>` generalizes `StringStorage` to any reference type — textures, audio clips, AI graphs, or any managed resource. The API is identical; only the handle type changes to `ObjectId`.

### Setup

```csharp
ObjectStorage<Texture> textures = WorldFactory.Shared.CreateObjectStorage<Texture>();
```

### Adding and reading

```csharp
ObjectId textureId = textures.Add(new Texture("hero.png"));
Texture texture = textures.Get(textureId);
```

### Storing the handle in a component

```csharp
public struct RenderComponent : IComponent
{
    public ObjectId TextureId;
}

World world = WorldFactory.Shared.CreateWorld();
world.InitComponent<RenderComponent>();

EntityId entityId = world.CreateEntityId();
world.AddComponent<RenderComponent>(entityId).TextureId = textures.Add(new Texture("hero.png"));
```

### Lifetime management

```csharp
textures.Update(textureId, new Texture("hero_hd.png"));

textures.Delete(textureId);

// Safe read — returns false for stale or wrong-storage handles
if (textures.TryGet(textureId, out Texture? tex))
    Render(tex);
```

`ObjectId` and `StringId` are distinct types; handles from one storage cannot be passed to another even by accident. Each storage issued by `WorldFactory.Shared` has a unique `StorageId` embedded in every handle.

---

## Example 13 — Dynamic buffers (`DynamicBuffer<T>`)

A dynamic buffer stores a variable-length sequence of elements per entity. Unlike `StaticBuffer<T>` (fixed max capacity set at init time), a dynamic buffer grows on demand — each entity gets its own independently-sized list.

### Component definition

```csharp
public struct DamageEvent : IDynamicBufferComponent
{
    public int Value;
    public EntityId DealerId;
}
```

### Initialization

```csharp
World world = WorldFactory.Shared.CreateWorld();

// default capacity and growth strategy from WorldSettings
world.InitDynamicBuffer<DamageEvent>();

// explicit initial capacity per buffer
world.InitDynamicBuffer<DamageEvent>(maxComponentsPerSet: 256, initialCapacity: 8);
```

### Writing to a buffer

```csharp
EntityId entityId = world.CreateEntityId();

DynamicBuffer<DamageEvent> buffer = world.AddDynamicBuffer<DamageEvent>(entityId);
buffer.AddLast(new DamageEvent { Value = 10, DealerId = attackerId });
buffer.AddLast(new DamageEvent { Value = 5,  DealerId = attackerId });
```

`AddBuffer` is idempotent — calling it on an entity that already has a buffer returns the existing one.

### Reading and processing

```csharp
DynamicBuffer<DamageEvent> buffer = world.GetDynamicBuffer<DamageEvent>(entityId);

var components = buffer.Components;
for (int i = 0; i < buffer.Count; i++)
{
    DamageEvent ev = components[i]
    // process ev...
}

buffer.Clear(); // keep buffer allocated, reset count to 0
```

### Iteration over all entities with a buffer

```csharp
DynamicBufferCollection<DamageEvent> collection = world.GetDynamicBuffers<DamageEvent>();

for (int i = 0; i < collection.BufferCount; i++)
{
    EntityId entity = collection.Entities[i];~~~~
    DynamicBuffer<DamageEvent> buf = collection.GetBuffer(entity);
    // process buf...
    buf.Clear();
}
```


> **Stale handle warning:** After `DeleteDynamicBuffer` called the instance of dynamic buffer is invalidated. Any further call on the old handle (e.g. `AddLast`) will throw. Always discard handles after deletion.

### `StaticBuffer<T>` vs `DynamicBuffer<T>`

| | `StaticBuffer<T>` | `DynamicBuffer<T>` |
|---|---|---|
| Capacity | fixed max set at `InitStaticBuffer` | grows on demand per entity |
| Memory layout | all entity slots pre-allocated contiguously | separate heap array per entity |
| Best for | short, bounded event lists (incoming damage, etc.) | unbounded queues, entity inventories, command lists |

Static and dynamic buffers on archetype entities are not allowed — `AddDynamicBuffer` and `DeleteDynamicBuffer` throw for entities that belong to a static archetype.
