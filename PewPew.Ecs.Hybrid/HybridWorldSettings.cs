using PewPew.Ecs.Core;

namespace PewPew.Ecs.Hybrid;

public class HybridWorldSettings : WorldSettings
{
    public int MaxEntitiesPerArchetype = ushort.MaxValue;
    public int MaxEntitiesPerFilter = ushort.MaxValue;
}