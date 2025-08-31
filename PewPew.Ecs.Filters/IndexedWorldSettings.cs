using PewPew.Ecs.Core;

namespace PewPew.Ecs.Filters;

public class IndexedWorldSettings : WorldSettings
{
    public int MaxEntitiesPerFilter = ushort.MaxValue;
}