using DataStructures.ViliWonka.KDTree;
using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PetOne.Systems
{
    // TODO: Reduse allocation
    /// <summary>
    /// Change Custom Gravity Source by tag
    /// </summary>
    internal sealed class ChangeNGravitySource : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<RealTransform, NGravityAttractor, ChangeSourceTag> _filter = null;
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
                // New Arrays
                hits = new NativeArray<RaycastHit>(count, Allocator.TempJob);
                commands = new NativeArray<RaycastCommand>(count, Allocator.TempJob);
                // Fill input Array
                foreach (var i in _filter)
                    commands[i] = NewComand(i);
                // Schedule
                handle = RaycastCommand.ScheduleBatch(commands, hits, 2);
                // Finalize
                FinalizeJob();
            }
        }

        private RaycastCommand NewComand(int index)
        {
            // Get Direction
            var position = _filter.Get1(index).Value.position;
            var points = GetClosestPoints(position);
            var point = GetClosestPoint(position, points);
            var dircetion = point - position;
            // New Command
            return new RaycastCommand(position, dircetion, _injectData.NewGravitySourceScanRadiuce, _gravityLayer);
        }

        /// <summary>
        /// Get Closest points from closest colliders
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector3[] GetClosestPoints(Vector3 position)
        {
            var colliders = Physics.OverlapSphere(position, _injectData.NewGravitySourceScanRadiuce, _gravityLayer);
            var points = colliders.Select(x => x.ClosestPoint(position)).ToArray();
            return points;
        }

        /// <summary>
        /// Returns closest to position point from array
        /// </summary>
        /// <param name="position"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private Vector3 GetClosestPoint(Vector3 position, Vector3[] points)
        {
            var tree = new KDTree(points, 8);
            var query = new KDQuery();
            var results = new List<int>();
            query.ClosestPoint(tree, position, results);

            return points[results.First()];
        }

        private async void FinalizeJob()
        {
            // Await complet of the job
            while (!handle.IsCompleted)
            {
                await Task.Delay(Mathf.FloorToInt(Time.deltaTime));
            }
            handle.Complete();
            // Update Entities
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                entity.Get<NGravityRotateToTag>();
                // Update normal
                var attractor = _filter.Get2(i);
                attractor.NormalToGround = hits[i].normal;
                entity.Replace(attractor);
            }
            // Dispose Arrays
            hits.Dispose();
            commands.Dispose();
        }
    }
}