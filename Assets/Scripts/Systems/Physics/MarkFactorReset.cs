using Leopotam.Ecs;
using PetOne.Components;
using Unity.Mathematics;

namespace PetOne.Systems
{
    internal sealed class MarkFactorReset : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<JumpData, PhysicBody, RealTransform> _filter = null;


        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                PhysicBody body = _filter.Get2(i);
                float3 up = _filter.Get3(i).Value.up;
                bool isInAir = _filter.Get1(i).IsInAir;
                bool isntRising = math.dot(body.Value.velocity, up) <= 0;
                if (isntRising && isInAir)
                {
                    EcsEntity entity = _filter.GetEntity(i);
                    entity.Get<FactorResetTag>();
                }
            }
        }
    }
}