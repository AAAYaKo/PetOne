using DataStructures.ViliWonka.KDTree;
using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    internal sealed class ChangeNGravitySource : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<ChangeSourceTag, RealTransform> _filter = null;
        private readonly LayerMask _gravityLayer = default;
        private readonly InjectData _injectData = null;

        [EcsIgnoreInject] private JobHandle handle;
        [EcsIgnoreInject] private NativeArray<RaycastCommand> commands;
        [EcsIgnoreInject] private NativeArray<RaycastHit> hits;


        void IEcsRunSystem.Run()
        {
            int count = _filter.GetEntitiesCount();
            if (count != 0)
            {
                hits = new NativeArray<RaycastHit>(count, Allocator.TempJob);
                commands = new NativeArray<RaycastCommand>(count, Allocator.TempJob);
                foreach (var i in _filter)
                    commands[i] = NewComand(i);

                handle = RaycastCommand.ScheduleBatch(commands, hits, 6);

                FinalizeJob();
            }
        }

        private RaycastCommand NewComand(int index)
        {
            var position = _filter.Get2(index).Value.position;
            Vector3[] points = GetClosestPoints(position);

            var point = GetClosestPoint(position, points);
            var dircetion = point - position;

            return new RaycastCommand(position, dircetion, _injectData.NewGravitySourceScanRadiuce, _gravityLayer);
        }

        private Vector3[] GetClosestPoints(Vector3 position)
        {
            var colliders = Physics.OverlapSphere(position, _injectData.NewGravitySourceScanRadiuce, _gravityLayer);
            var points = colliders.Select(x => x.ClosestPoint(position)).ToArray();
            return points;
        }

        private Vector3 GetClosestPoint(Vector3 position, Vector3[] points)
        {
            KDTree tree = new KDTree(points, 8);
            KDQuery query = new KDQuery();
            List<int> results = new List<int>();
            query.ClosestPoint(tree, position, results);

            return points[results.First()];
        }

        private async void FinalizeJob()
        {
            while (!handle.IsCompleted)
            {
                await Task.Delay((int)math.round(Time.deltaTime));
            }
            handle.Complete();

            foreach (var i in _filter)
            {
                EcsEntity entity = _filter.GetEntity(i);
                entity.Get<NGravityRotateToTag>();
                NGravityAttractor attractor = entity.Get<NGravityAttractor>();
                attractor.NormalToGround = hits[i].normal;
                entity.Replace(attractor);
            }

            hits.Dispose();
            commands.Dispose();
        }
    }
}