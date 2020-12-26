using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Client 
{
    sealed class PhysicTranslationSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<PhysicBody, PhysicTranslation, RealTransform> _filter = null;
        
        void IEcsRunSystem.Run ()
        {
            foreach (var i in _filter)
            {
                float3 direction = _filter.Get2(i).Value;
                float3 position = _filter.Get3(i).Value.position;
                Rigidbody body = _filter.Get1(i).Value;
                body.MovePosition(position + direction);
            }
        }
    }
}