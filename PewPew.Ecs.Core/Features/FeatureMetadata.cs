namespace PewPew.Ecs.Core.Features;

internal static class FeatureMetadata<T>
{
    public static int Index = FeatureMetadata.InvalidIndex;
}

internal static class FeatureMetadata
{
    public const int InvalidIndex = -1;

    public static int TotalCount = 0;
}