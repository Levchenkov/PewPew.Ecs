namespace PewPew.Ecs.SourceGenerators;

[AttributeUsage(AttributeTargets.Struct)]
public class StaticArchetypeAttribute : Attribute
{
    public StaticArchetypeAttribute(params string[] genericParameters)
    {
    }
}