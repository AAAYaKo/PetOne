using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Lerp and update speed percent in animator
    /// </summary>
    internal sealed class AnimatorWalkingSystem : IEcsRunSystem
    {
        // names related to animator
        private const string SPEED_PROPERTY_NAME = "SpeedPercent";

        // auto-injected fields.
        private readonly EcsFilter<ViewComponent> _filter = null;
        private readonly InjectData _injectData = null;


        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime * _injectData.LerpAnimatorSpeed;
            foreach (var i in _filter)
            {
                ref var view = ref _filter.Get1(i);
                LerpSpeedPercent(delta, i, view);
            }
        }

        private void LerpSpeedPercent(float delta, int index, ViewComponent view)
        {
            float to = _filter.Get1(index).TargetSpeedPercent;
            float from = view.Animator.GetFloat(SPEED_PROPERTY_NAME);
            if (!Mathf.Approximately(from, to))
            {
                float lerp = math.lerp(from, to, delta);
                view.Animator.SetFloat(SPEED_PROPERTY_NAME, lerp);
            }
        }
    }
}