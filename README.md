# PewPew.Ecs

PewPew.Ecs is an ECS library built around sparse-set storage, allocation-free iteration, and explicit control over world layout.

It supports multiple worlds in one state, indexed filters, hybrid static archetypes, command buffers, and source-generated archetype wrappers.

## Packages and layers

- `PewPew.Ecs.Core` — base ECS runtime (`World`, `EntityId`, sparse sets, command buffer)
- `PewPew.Ecs.Filters` — indexed worlds and cached filters on top of Core
- `PewPew.Ecs.Hybrid` — sparse world + static archetypes + filters
- `PewPew.Ecs.Hybrid.SourceGenerators` — generators for custom archetype wrappers
- `PewPew.Ecs.Demo` — runnable usage examples referenced by the documentation

## Installation

```powershell
dotnet add package PewPew.Ecs~~~~
```

## Quickstart

```csharp
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

## Example-driven documentation

Detailed documentation based on the runnable snippets in `PewPew.Ecs.Demo/Examples.cs` lives in [`EXAMPLES.md`](EXAMPLES.md).

It covers:

- quickstart with regular components
- multi-world state with singleton, tag, and static-buffer components
- system-style updates over collections and tags
- indexed filters with `BitMask64`, `BitMask128`, or `BitMask256`
- hybrid worlds with static archetypes
- SIMD batch queries
- source-generated archetype wrappers
- deferred deletes through command buffers
- optional entity naming support
- blittable primitive wrappers (`BlittableBool`, `BlittableChar`)
- inline UTF-8 strings in components (`BlittableString32`)
- versioned string storage (`StringStorage` / `StringId`)
- versioned generic object storage (`ObjectStorage<T>` / `ObjectId`)
- dynamic buffers (`DynamicBuffer<T>`) — variable-length per-entity sequences

## Important rules

- Initialize every component, tag, singleton, static buffer, and dynamic buffer before using it.
- Component and tag deletion is swap-and-pop, so entity order is not stable.
- Indexed filters require at least two unique component types in a filter definition.
- In hybrid worlds, initialize static archetypes before creating hybrid filters.
- A `DynamicBuffer<T>` handle is invalidated after `DeleteDynamicBuffer` — do not call methods on it after deletion.

## Features

- data locality
- allocation-free query execution
- multiple worlds per state
- component kinds:
  - regular
  - singleton
  - tag
  - static buffer
  - dynamic buffer
- entity naming
- non-indexed queries
- indexed filters
- static archetypes
- command buffers
- source-generated hybrid archetypes
- blittable primitive wrappers: `BlittableBool`, `BlittableChar`
- inline UTF-8 string for components: `BlittableString32`
- versioned string storage: `StringStorage` / `StringId`
- versioned generic object storage: `ObjectStorage<T>` / `ObjectId`

## Run locally

Run the demo project:

```powershell
dotnet run --project .\PewPew.Ecs.Demo\PewPew.Ecs.Demo.csproj
```

Run the full test suite:

```powershell
dotnet test .\PewPew.Ecs.Private.sln -v minimal
```