using Leopotam.Ecs;
using PetOne.Components;
using UnityEngine;

namespace PetOne.Systems
{
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
                ref NGravityAttractor attractor = ref _filter.Get2(i);
                EcsEntity entity = _filter.GetEntity(i);

                if (entity.Has<PhysicTranslation>())
                {
                    attractor.TimeToSleep = 0;
                    entity.Del<WannaSleepTag>();
                    continue;
                }

                bool wannaSleep = body.Value.velocity.sqrMagnitude <= SLEEP_THRESHOLD;

                if (wannaSleep && (attractor.TimeToSleep >= SLEEP_DELLAY))
                    entity.Get<WannaSleepTag>();

                else if (wannaSleep)
                    attractor.TimeToSleep += delta;

                else
                {
                    attractor.TimeToSleep = 0;
                    entity.Del<WannaSleepTag>();
                }
            }
        }
    }
}