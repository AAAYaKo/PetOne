using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Client
{
    sealed class TestNGravityAttractorsInit : IEcsInitSystem
    {
        // auto-injected fields.
        private readonly EcsWorld _world = null;
        private readonly int _attractorsCount = 0;
        private readonly GameObject _attractor;

        public void Init()
        {
            for (uint i = 0; i < _attractorsCount; i++)
            {
                Random random = Random.CreateFromIndex(i);
                float3 position = random.NextFloat3() * 10;
                GameObject gameObject = Object.Instantiate(_attractor);
                Transform transform = gameObject.transform;
                Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
                transform.position = position;

                EcsEntity entity = _world.NewEntity();
                ref RealTransform realTransform = ref entity.Get<RealTransform>();
                realTransform.Value = transform;
                ref PhysicBody body = ref entity.Get<PhysicBody>();
                body.Value = rigidbody;
                ref NGravityAttractor attractor = ref entity.Get<NGravityAttractor>();
                attractor.GravityFactor = random.NextFloat(0, 10);
                entity.Get<ChangeSourceTag>();
            }
        }
    }
}