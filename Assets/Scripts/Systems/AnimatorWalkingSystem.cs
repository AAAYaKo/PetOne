using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class AnimatorWalkingSystem : IEcsRunSystem
    {
        private const string SPEED_FIELD_NAME = "SpeedPercent";

        private const float APPROXIMATION = 0.000001f;

        // auto-injected fields.
        private readonly EcsFilter<TargetSpeedPercent, ViewComponent> _filter = null;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime;
            foreach (var i in _filter)
            {
                ref ViewComponent view = ref _filter.Get2(i);
                LerpSpeedPercent(delta, i, view);
            }
        }

        private void LerpSpeedPercent(float delta, int index, ViewComponent view)
        {
            EcsEntity entity = _filter.GetEntity(index);
            float to = _filter.Get1(index).Value;
            float from = view.Animator.GetFloat(SPEED_FIELD_NAME);
            if (math.abs(to - from) > APPROXIMATION)
            {
                float lerp = math.lerp(from, to, delta * _injectData.LerpAnimatorSpeed);
                view.Animator.SetFloat(SPEED_FIELD_NAME, lerp);
            }
            else
                entity.Del<TargetSpeedPercent>();
        }
    }
}