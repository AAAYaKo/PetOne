using Leopotam.Ecs;
using Unity.Mathematics;

namespace Client
{
    sealed class MarkFactorReset : IEcsRunSystem
    {

        // auto-injected fields.
        private readonly EcsFilter<FactorOverrided, PhysicBody, RealTransform> _filter = null;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                PhysicBody body = _filter.Get2(i);
                float3 up = _filter.Get3(i).Value.up;
                bool isntRising = !(math.dot(body.Value.velocity, up) > 0);
                if (isntRising)
                    _filter.GetEntity(i).Get<FactorReset>();
            }
        }
    }
}