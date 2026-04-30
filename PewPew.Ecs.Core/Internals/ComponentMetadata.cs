using System.Diagnostics;

namespace PewPew.Ecs.Core.Internals;

internal static class ComponentMetadata<T>
    where T : struct
{
    public static int GlobalIndex = ComponentMetadata.InvalidIndex;

    public static void InitComponentMetadata()
    {
        if (GlobalIndex != ComponentMetadata.InvalidIndex)
            return;

        if (!BlittableHelper.IsBlittable(typeof(T)))
            ThrowHelper.ThrowNonBlittableComponentException<T>();

        GlobalIndex = ComponentMetadata.TotalCount++;

#if DEBUG
        ComponentMetadata.DebugComponentMap[GlobalIndex] =  typeof(T);
#endif
    }

    [Conditional("DEBUG")]
    public static void DebugSetGlobalIndex(int globalIndex)
    {
        GlobalIndex = globalIndex;
        ComponentMetadata.TotalCount = Math.Max(ComponentMetadata.TotalCount, globalIndex);
        ComponentMetadata.TotalCount++;
    }
}

internal static class ComponentMetadata
{
    public const int InvalidIndex = -1;

    public static int TotalCount = 0;

#if DEBUG
    public static Dictionary<int, Type> DebugComponentMap = new();
#endif
}