using PewPew.Ecs.Core;

namespace PewPew.Ecs.Demo;

public sealed class HealthRegenSystem
{
    public void Update(CustomState state)
    {
        HealthRegenSettings healthRegenSettings = state.SettingsWorld.GetSingleton<HealthRegenSettings>();

        ComponentCollection<Health> healths = state.ActorsWorld.GetComponents<Health>();
        Span<EntityId> entitiesToRegen = state.ActorsWorld.GetTags<HealthRegenActive>().Entities;
        for (int i = 0; i < entitiesToRegen.Length; i++)
        {
            EntityId entity = entitiesToRegen[i];
            ref Health health = ref healths.GetComponent(entity);
            health.Current = Math.Min(health.Current + healthRegenSettings.RegenPerTick, health.Max);
        }
    }
}