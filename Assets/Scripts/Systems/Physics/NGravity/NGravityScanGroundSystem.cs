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
                hits = new NativeArray<RaycastHit>(count, Allocator.TempJob);
                commands = new NativeArray<SpherecastCommand>(count, Allocator.TempJob);
                foreach (var i in _filter)
                    commands[i] = NewComand(i);

                handle = SpherecastCommand.ScheduleBatch(commands, hits, 4);
                handle.Complete();

                WriteToAttractors();

                hits.Dispose();
                commands.Dispose();
            }
        }

        private SpherecastCommand NewComand(int i)
        {
            Transform transform = _filter.Get1(i).Value;
            float3 position = transform.position;
            float3 direction = -transform.up;
            return new SpherecastCommand(position, _injectData.RadiusOfGroundScan, direction, layerMask: _gravityLayer);
        }

        private async void WriteToAttractors()
        {
            while (!handle.IsCompleted)
            {
                await Task.Delay((int)math.round(Time.deltaTime));
            }

            foreach (var i in _filter)
            {
                NGravityAttractor attractor = _filter.Get2(i);
                attractor.NormalToGround = hits[i].normal;
                attractor.DistanceToGround = hits[i].distance;
                _filter.GetEntity(i).Replace(attractor);
            }
        }
    }
}