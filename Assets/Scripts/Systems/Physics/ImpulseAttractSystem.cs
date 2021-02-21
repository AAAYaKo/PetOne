using Leopotam.Ecs;
using PetOne.Components;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    internal sealed class ImpulseAttractSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<ForceImpulse, PhysicBody> _filter;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                Rigidbody body = _filter.Get2(i).Value;
                float3 force = _filter.Get1(i).Value * body.mass;
                body.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}