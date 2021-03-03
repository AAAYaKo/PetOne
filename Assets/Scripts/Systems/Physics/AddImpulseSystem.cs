using Leopotam.Ecs;
using PetOne.Components;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Add impulse to body
    /// </summary>
    internal sealed class AddImpulseSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<ForceImpulse, PhysicBody> _filter;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                var body = _filter.Get2(i).Value;
                var force = _filter.Get1(i).Value * body.mass;
                body.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}