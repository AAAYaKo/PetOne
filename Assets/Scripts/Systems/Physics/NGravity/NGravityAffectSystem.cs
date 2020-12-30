using Leopotam.Ecs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class NGravityAffectSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<RealTransform, NGravityAttractor>.Exclude<NGravityRotateToTag>.Exclude<WannaSleep> _filter = null;
        private readonly LayerMask _gravityLayer = default;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            int count = _filter.GetEntitiesCount();
            if (count != 0)
            {
                //Job
                var hits = new NativeArray<RaycastHit>(count, Allocator.Persistent);
                var commands = new NativeArray<SpherecastCommand>(count, Allocator.Persistent);

                //fill NativeArrays
                foreach (var i in _filter)
                {
                    Transform transform = _filter.Get1(i).Value;
                    float3 position = transform.position;
                    float3 direction = -transform.up;
                    commands[i] = new SpherecastCommand(position, _injectData.RadiusOfGroundScan, direction, layerMask: _gravityLayer);
                }
                SpherecastCommand.ScheduleBatch(commands, hits, 1).Complete();

                //Get|Add Entity Data
                foreach (var i in _filter)
                {
                    ref NGravityAttractor attractor = ref _filter.Get2(i);
                    attractor.NormalToGround = hits[i].normal;
                    attractor.DistanceToGround = hits[i].distance;
                }

                //Dispose NativeArrays
                hits.Dispose();
                commands.Dispose();
            }
        }

    }
}