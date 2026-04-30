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

public struct DynamicEvent : IDynamicBufferComponent
{
    public int Value;
}

public struct DynamicDamage : IDynamicBufferComponent
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

public struct BlittableBoolComponent : IComponent
{
    public BlittableBool Flag;
    public BlittableBool OtherFlag;
}

public struct BlittableCharComponent : IComponent
{
    public BlittableChar Letter;
    public BlittableChar Symbol;
}

public struct BlittableString32Component : IComponent
{
    public BlittableString32 Name;
}

public struct StringIdComponent : IComponent
{
    public StringId DescriptionId;
}

public struct ObjectIdComponent : IComponent
{
    public ObjectId DataId;
}