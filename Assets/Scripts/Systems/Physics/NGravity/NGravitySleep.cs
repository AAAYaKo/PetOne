using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class NGravitySleep : IEcsRunSystem
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
                    attractor.Time = 0;
                    entity.Del<WannaSleep>();
                    continue;
                }

                bool wannaSleep = body.Value.velocity.sqrMagnitude <= SLEEP_THRESHOLD;

                if (wannaSleep && (attractor.Time >= SLEEP_DELLAY))
                    entity.Get<WannaSleep>();

                else if (wannaSleep)
                    attractor.Time += delta;

                else
                {
                    attractor.Time = 0;
                    entity.Del<WannaSleep>();
                }
            }
        }
    }
}