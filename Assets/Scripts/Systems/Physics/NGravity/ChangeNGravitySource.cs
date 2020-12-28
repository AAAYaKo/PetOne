using DataStructures.ViliWonka.KDTree;
using Leopotam.Ecs;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class ChangeNGravitySource : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<ChangeSourceTag, RealTransform> _filter = null;
        private readonly LayerMask _gravityLayer = default;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            int count = _filter.GetEntitiesCount();
            if (count != 0)
            {
                NativeArray<RaycastHit> hits = new NativeArray<RaycastHit>(count, Allocator.TempJob);
                NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(count, Allocator.TempJob);

                foreach (var i in _filter)
                {
                    _filter.GetEntity(i).Get<NGravityRotateToTag>();

                    float3 position = _filter.Get2(i).Value.position;
                    var colliders = Physics.OverlapSphere(position, _injectData.NewGravitySourceScanRadiuce, _gravityLayer);
                    var points = colliders.Select(x => x.ClosestPoint(position)).ToArray();

                    KDTree tree = new KDTree(points, 8);
                    KDQuery query = new KDQuery();
                    List<int> results = new List<int>();
                    query.ClosestPoint(tree, position, results);

                    float3 point = points[results[0]];
                    float3 dircetion = point - position;

                    commands[i] = new RaycastCommand(position, dircetion, _injectData.NewGravitySourceScanRadiuce, _gravityLayer);
                }

                RaycastCommand
                    .ScheduleBatch(commands, hits, 1)
                    .Complete();

                foreach (var i in _filter)
                {
                    EcsEntity entity = _filter.GetEntity(i);
                    ref NGravityAttractor attractor = ref entity.Get<NGravityAttractor>();
                    attractor.NormalToGround = hits[i].normal;
                }

                hits.Dispose();
                commands.Dispose();
            }
        }
    }
}