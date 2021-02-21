using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    sealed class AnimatorWalkingSystem : IEcsRunSystem
    {
        private const string SPEED_FIELD_NAME = "SpeedPercent";
        private const float APPROXIMATION = 0.000001f;

        // auto-injected fields.
        private readonly EcsFilter<ViewComponent> _filter = null;
        private readonly InjectData _injectData = null;


        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime;
            foreach (var i in _filter)
            {
                ref var view = ref _filter.Get1(i);
                LerpSpeedPercent(delta, i, view);
            }
        }

        private void LerpSpeedPercent(float delta, int index, ViewComponent view)
        {
            float to = _filter.Get1(index).TargetSpeedPercent;
            float from = view.Animator.GetFloat(SPEED_FIELD_NAME);
            if (math.abs(to - from) > APPROXIMATION)
            {
                float lerp = math.lerp(from, to, delta * _injectData.LerpAnimatorSpeed);
                view.Animator.SetFloat(SPEED_FIELD_NAME, lerp);
            }
        }
    }
}