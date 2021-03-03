using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Scans ground colliders and writes data to Attractors
    /// </summary>
    internal sealed class NGravityScanGroundSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<RealTransform, NGravityAttractor>.Exclude<NGravityRotateToTag, WannaSleepTag> _filter = null;
        private readonly LayerMask _gravityLayer = default;
        private readonly InjectData _injectData = null;

        [EcsIgnoreInject] private JobHandle handle;
        [EcsIgnoreInject] private NativeArray<RaycastHit> hits;
        [EcsIgnoreInject] private NativeArray<SpherecastCommand> commands;


        void IEcsRunSystem.Run()
        {
            int count = _filter.GetEntitiesCount();
            if (count != 0)
            {
                // New Arrays
                hits = new NativeArray<RaycastHit>(count, Allocator.TempJob);
                commands = new NativeArray<SpherecastCommand>(count, Allocator.TempJob);
                // Fill input Array
                foreach (var i in _filter)
                    commands[i] = NewComand(i);
                // Schedule
                handle = SpherecastCommand.ScheduleBatch(commands, hits, 4);
                // Finalize
                FinalizeJob();
            }
        }

        private SpherecastCommand NewComand(int i)
        {
            var transform = _filter.Get1(i).Value;
            var position = transform.position;
            var direction = -transform.up;
            return new SpherecastCommand(position, _injectData.RadiusOfGroundScan, direction, layerMask: _gravityLayer);
        }

        private async void FinalizeJob()
        {
            // Await complete of the job
            while (!handle.IsCompleted)
            {
                await Task.Delay((int)math.round(Time.deltaTime));
            }
            handle.Complete();
            // Write data to attractors
            foreach (var i in _filter)
            {
                var attractor = _filter.Get2(i);
                attractor.NormalToGround = hits[i].normal;
                attractor.DistanceToGround = hits[i].distance;
                _filter.GetEntity(i).Replace(attractor);
            }
            // Dispose Arrays
            hits.Dispose();
            commands.Dispose();
        }
    }
}