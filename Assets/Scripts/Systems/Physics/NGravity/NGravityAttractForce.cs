using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class NGravityAttractForce : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<NGravityAttractor, PhysicBody, RealTransform>.Exclude<NGravityRotateToTag> _filter = null;

        void IEcsRunSystem.Run()
        {
            foreach(var i in _filter)
            {
                var attractor = _filter.Get1(i);
                var body = _filter.Get2(i).Value;
                var transform = _filter.Get3(i).Value;
                quaternion angle = Calculate.FromToRotation(transform.up, attractor.NormalToGround);

                body.AddForce(-attractor.NormalToGround * attractor.GravityFactor, ForceMode.Acceleration);
                quaternion slerp = math.slerp(transform.rotation, angle * transform.rotation, 10 * Time.fixedDeltaTime);
                body.MoveRotation(slerp);
            }
        }
    }
}