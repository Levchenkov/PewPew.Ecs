namespace PewPew.Ecs.SourceGenerators;

[AttributeUsage(AttributeTargets.Class)]
public class StaticArchetypeInstanceAttribute : Attribute
{
    public StaticArchetypeInstanceAttribute(params string[] genericParameters)
    {
    }
}