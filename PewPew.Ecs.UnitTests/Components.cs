using System.Numerics;
using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public struct EmptyComponent : IComponent, ITagComponent
{
}

public struct HealthComponent : IComponent, ISingletonComponent
{
    public int Current;
    public int Max;
}

public struct SingletonComponent : ISingletonComponent
{
    public int SomeValue;
}

public struct Damage : IStaticBufferComponent
{
    public int Value;
    public EntityId TargetId;
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

public struct Component5 : IComponent
{
    public int Value;
}

public struct Tag1 : ITagComponent {}
public struct Tag2 : ITagComponent {}
public struct Tag3 : ITagComponent {}
public struct Tag4 : ITagComponent {}