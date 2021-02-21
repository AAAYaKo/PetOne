using Leopotam.Ecs;
using PetOne.Components;
using UnityEngine;

namespace PetOne.Systems
{
    internal sealed class PhysicTranslationSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<PhysicBody, PhysicTranslation, RealTransform> _filter = null;
        

        void IEcsRunSystem.Run ()
        {
            foreach (var i in _filter)
            {
                var direction = (Vector3)_filter.Get2(i).Value;
                var position = _filter.Get3(i).Value.position;
                var body = _filter.Get1(i).Value;
                body.MovePosition(position + direction);
            }
        }
    }
}