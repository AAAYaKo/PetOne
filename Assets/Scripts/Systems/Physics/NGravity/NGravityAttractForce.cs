using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class NGravityAttractForce : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<NGravityAttractor, PhysicBody>.Exclude<NGravityRotateToTag> _filter = null;

        void IEcsRunSystem.Run()
        {
            foreach(var i in _filter)
            {
                var attractor = _filter.Get1(i);
                var body = _filter.Get2(i);
                body.Value.AddForce(-attractor.NormalToGround * attractor.GravityFactor, ForceMode.Acceleration);
            }
        }
    }
}