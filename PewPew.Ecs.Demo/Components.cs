using System.Numerics;
using PewPew.Ecs.Core;

namespace PewPew.Ecs.Demo;

public struct Ammo : IComponent
{
    public int Current;
    public int Max;
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

public struct AliveComponent : IComponent
{
    public int Tick;
}

public struct Alive : ITagComponent
{
}

public struct Dead : ITagComponent
{
}

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

public struct Position : IComponent
{
    public Vector3 Vector;
}

public struct Speed : IComponent
{
    public Vector3 Vector;
}

public struct SomeStaticBuffer : IStaticBufferComponent
{
    public int SomeValue;
}

public struct Component1 : IComponent
{
    public int Value;
}

public struct Component2 : IComponent
{
    public int Value;
}

public struct Component3 : IComponent
{
    public int Value;
}

public struct Component4 : IComponent
{
    public int Value;
}