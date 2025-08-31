namespace PewPew.Ecs.Core;

public class WorldSettings
{
    public int MaxEntitiesCount = ushort.MaxValue;
    public int MaxComponentsPerSet = ushort.MaxValue;
    public int MaxAllowedUniqueComponentsCount = 64; // limited by default, can be not limited
    public int MaxElementsCountPerSet = 100;
    public int MaxEntitiesPerGroup = ushort.MaxValue; // for group feature
}