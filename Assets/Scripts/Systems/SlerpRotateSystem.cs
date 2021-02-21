using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    internal sealed class SlerpRotateSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<TargetRotation, RealTransform> _filter = null;

        private readonly InjectData _injectData = null;


        void IEcsRunSystem.Run()
        {
            float time = Time.deltaTime * _injectData.SlerpRotateViewSpeed;
            foreach (var i in _filter)
            {
                var target = _filter.Get1(i).Value;
                var transform = _filter.Get2(i).Value;
                transform.rotation = math.slerp(transform.rotation, target, time);
            }
        }
    }
}