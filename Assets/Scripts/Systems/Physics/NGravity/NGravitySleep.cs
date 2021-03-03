using Leopotam.Ecs;
using PetOne.Components;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Putts attractors to sleep if they don't move
    /// </summary>
    internal sealed class NGravitySleep : IEcsRunSystem
    {
        private const float SLEEP_THRESHOLD = 0.0001f;
        private const float SLEEP_DELLAY = 1;

        // auto-injected fields.
        private readonly EcsFilter<PhysicBody, NGravityAttractor> _filter = null;

        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime;
            foreach (var i in _filter)
            {
                var body = _filter.Get1(i);
                ref var attractor = ref _filter.Get2(i);
                var entity = _filter.GetEntity(i);
                // If entity is moving
                if (entity.Has<PhysicTranslation>())
                {
                    SetNotSleepping(ref attractor, entity);
                    continue;
                }
                bool wannaSleep = (body.Value.velocity.sqrMagnitude <= SLEEP_THRESHOLD);
                // Not want sleep
                if (!wannaSleep)
                {
                    SetNotSleepping(ref attractor, entity);
                }
                // Put to sleep
                else if (attractor.TimeToSleep >= SLEEP_DELLAY)
                {
                    entity.Get<WannaSleepTag>();
                }
                // Tick
                else
                {
                    attractor.TimeToSleep += delta;
                }
            }
        }

        private void SetNotSleepping(ref NGravityAttractor attractor, EcsEntity entity)
        {
            attractor.TimeToSleep = 0;
            entity.Del<WannaSleepTag>();
        }
    }
}