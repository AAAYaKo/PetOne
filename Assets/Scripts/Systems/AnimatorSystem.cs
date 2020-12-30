using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class AnimatorSystem : IEcsRunSystem
    {
        private const string FIELD_NAME = "SpeedPercent";
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
                float to = _filter.Get1(i).Value;
                float from = view.Animator.GetFloat(FIELD_NAME);
                if (math.abs(to - from) > APPROXIMATION)
                {
                    float lerp = math.lerp(from, to, delta * _injectData.LerpAnimatorSpeed);
                    view.Animator.SetFloat(FIELD_NAME, lerp);
                }
                else
                    _filter.GetEntity(i).Del<TargetSpeedPercent>();
            }
        }
    }
}