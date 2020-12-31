using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class LandingReactionSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<NGravityAttractor, JumpData> _filter = null;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            float toFoot = _injectData.ToFootDistance;
            foreach (var i in _filter)
            {
                ref JumpData jump = ref _filter.Get2(i);
                float toGround = _filter.Get1(i).DistanceToGround;
                if (!(toGround > toFoot))
                {
                    if (jump.IsInAir)
                    {
                        EcsEntity entity = _filter.GetEntity(i);
                        entity.Del<JumpData>();
                        entity.Get<LandedTag>();
                    }
                    else
                        jump.IsInAir = true;
                }
            }
        }
    }
}