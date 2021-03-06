using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    // TODO: It can be rewrited to DOTween
    /// <summary>
    /// By tag rotates attractor to new source (Animation)
    /// </summary>
    internal sealed class NGravityRotateToNewSource : IEcsRunSystem
    {
        private const float APPROXIMATION = 0.99f;

        // auto-injected fields.
        private readonly EcsFilter<NGravityAttractor, RealTransform, NGravityRotateToTag> _filter = null;
        private readonly InjectData _injectData = null;


        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime * _injectData.SlerpToGravitySourceSpeed;

            foreach (var i in _filter)
            {
                ref var attractor = ref _filter.Get1(i);
                var transform = _filter.Get2(i).Value;
                // Rotation animation
                var up = transform.up;
                var upTarget = attractor.NormalToGround;
                var angle = Calculate.FromToRotation(up, upTarget);
                transform.rotation = math.slerp(transform.rotation, angle * transform.rotation, delta);
                // End rotation (Animation)
                if (math.dot(up, upTarget) > APPROXIMATION)
                    _filter.GetEntity(i).Del<NGravityRotateToTag>();
            }
        }
    }
}