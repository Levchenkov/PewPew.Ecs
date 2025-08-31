using PewPew.Ecs.Core;

namespace PewPew.Ecs.Demo;

public sealed class DamageSystem
{
    public void Update(CustomState state)
    {
        ComponentCollection<Health> healths = state.ActorsWorld.GetComponents<Health>();
        StaticBufferCollection<Damage> damages = state.ActorsWorld.GetStaticBuffers<Damage>();

        for (int i = 0; i < damages.BufferCount; i++)
        {
            EntityId damagedEntityId = damages.Entities[i];
            StaticBuffer<Damage> damageBuffer = damages.GetBuffer(damagedEntityId);

            ref Health health = ref healths.GetComponent(damagedEntityId);

            for (int j = 0; j < damageBuffer.Count; j++)
            {
                Damage damage = damageBuffer.Components[j];
                health.Current -= damage.Value;

                if (health.Current <= 0)
                {
                    state.ActorsWorld.AddComponent<KilledBy>(damagedEntityId).KilledByActorId = damage.DamageDealerId;
                    break;
                }
            }

            damageBuffer.Clear();
        }
    }
}