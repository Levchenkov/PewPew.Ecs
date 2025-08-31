using System.Reflection;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.Core;

public static class ComponentMetadataInitializer
{
    public static void FromAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            FromAssembly(assembly);
        }
    }

    public static void FromAssembly(Assembly assembly)
    {
        foreach (var valueType in assembly.GetTypes().Where(x => x is { IsValueType: true, IsEnum: false }))
        {
            if (GetSupportedComponentTypes().Any(supportedType => supportedType.IsAssignableFrom(valueType)))
            {
                InitType(valueType);
            }
        }
    }

    private static IEnumerable<Type> GetSupportedComponentTypes()
    {
        yield return typeof(IComponent);
        yield return typeof(ITagComponent);
        yield return typeof(ISingletonComponent);
        yield return typeof(IStaticBufferComponent);
    }

    private static void InitType(Type type)
    {
        if (!type.IsValueType || type.IsEnum)
        {
            throw new ArgumentException("Type must be a struct.", nameof(type));
        }

        Type genericType = typeof(ComponentMetadata<>).MakeGenericType(type);

        var initMethod = genericType.GetMethod(nameof(ComponentMetadata<int>.InitComponentMetadata), BindingFlags.Public | BindingFlags.Static);
        if (initMethod != null)
        {
            initMethod.Invoke(null, null);
        }
    }
}