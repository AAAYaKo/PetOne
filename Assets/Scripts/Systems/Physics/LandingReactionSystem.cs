using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class LandingReactionSystem : IEcsRunSystem
    {
        private const float TIME_STOP = 0.03f;

        // auto-injected fields.
        private readonly EcsFilter<NGravityAttractor, InAir> _filter = null;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            float toFoot = _injectData.ToFootDistance;
            foreach (var i in _filter)
            {
                ref InAir inAir = ref _filter.Get2(i);
                bool timeOut = inAir.Time >= TIME_STOP;
                if (timeOut)
                {
                    float toGround = _filter.Get1(i).DistanceToGround;
                    if (!(toGround > toFoot))
                    {
                        EcsEntity entity = _filter.GetEntity(i);
                        entity.Del<InAir>();
                        entity.Get<LandedTag>();
                    }
                }
                else
                    inAir.Time += Time.fixedDeltaTime;
            }
        }
    }
}