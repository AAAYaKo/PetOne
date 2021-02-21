using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    internal sealed class NGravityAttractForce : IEcsRunSystem
    {
        private const float SLERP_SPEED = 10;

        // auto-injected fields.
        private readonly EcsFilter<NGravityAttractor, PhysicBody, RealTransform>.Exclude<NGravityRotateToTag, WannaSleepTag> _filter = null;


        void IEcsRunSystem.Run()
        {
            foreach(var i in _filter)
            {
                var attractor = _filter.Get1(i);
                var body = _filter.Get2(i).Value;
                var transform = _filter.Get3(i).Value;

                if(!Equals(attractor.NormalToGround, float3.zero))
                {
                    quaternion angle = Calculate.FromToRotation(transform.up, attractor.NormalToGround);

                    body.AddForce(-attractor.NormalToGround * attractor.GravityFactor, ForceMode.Acceleration);
                    quaternion slerp = math.slerp(transform.rotation, angle * transform.rotation, SLERP_SPEED * Time.fixedDeltaTime);
                    body.MoveRotation(slerp);
                }
            }
        }
    }
}