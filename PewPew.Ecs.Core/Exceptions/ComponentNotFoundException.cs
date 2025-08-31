using System.Runtime.Serialization;

namespace PewPew.Ecs.Core.Exceptions;

public class ComponentNotFoundException : Exception
{
    public ComponentNotFoundException(Type type) : base(type.Name)
    {
    }

    public ComponentNotFoundException(Type type, EntityId entityId) : base($"[{type.Name}] {entityId}")
    {
    }

    public ComponentNotFoundException(Type type, int id) : base($"[{type.Name}] {id}")
    {
    }

    protected ComponentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}