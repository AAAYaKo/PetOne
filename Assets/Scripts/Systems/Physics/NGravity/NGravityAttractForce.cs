using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Attracts custom gravity Attractors to gravity sources
    /// </summary>
    internal sealed class NGravityAttractForce : IEcsRunSystem
    {
        private const float SLERP_SPEED = 5;

        // auto-injected fields.
        private readonly EcsFilter<NGravityAttractor, PhysicBody, RealTransform>.Exclude<NGravityRotateToTag, WannaSleepTag> _filter = null;


        void IEcsRunSystem.Run()
        {
            float delta = Time.fixedDeltaTime * SLERP_SPEED;
            foreach (var i in _filter)
            {
                var attractor = _filter.Get1(i);
                if (!Equals(attractor.NormalToGround, float3.zero))
                {
                    var body = _filter.Get2(i).Value;
                    var transform = _filter.Get3(i).Value;
                    // New rotation
                    var angle = Calculate.FromToRotation(transform.up, attractor.NormalToGround);
                    var slerp = math.slerp(transform.rotation, (angle * transform.rotation), delta);
                    body.MoveRotation(slerp);
                    // Add foprce
                    body.AddForce(-attractor.NormalToGround * attractor.GravityFactor, ForceMode.Acceleration);
                }
            }
        }
    }
}