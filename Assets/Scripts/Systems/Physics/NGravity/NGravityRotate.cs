using Leopotam.Ecs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Client
{
    sealed class NGravityRotate : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<NGravityAttractor, RealTransform, NGravityRotateToTag> _filter = null;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            int count = _filter.GetEntitiesCount();
            if(count != 0)
            {
                float delta = Time.deltaTime;
                NativeArray<float3> up = new NativeArray<float3>(count, Allocator.Persistent);
                NativeArray<float3> upTarget = new NativeArray<float3>(count, Allocator.Persistent);
                Transform[] transforms = new Transform[count];
                foreach (var i in _filter)
                {
                    ref var attractor = ref _filter.Get1(i);
                    transforms[i] = _filter.Get2(i).Value;
                    up[i] = transforms[i].up;
                    upTarget[i] = attractor.NormalToGround;
                    if (math.abs(math.dot(up[i], upTarget[i])) > 0.99f)
                        _filter.GetEntity(i).Del<NGravityRotateToTag>();
                }
                TransformAccessArray accessArray = new TransformAccessArray(transforms);

                var job = new RotateJob
                {
                    Up = up,
                    UpTarget = upTarget,
                    Time = _injectData.SlerpToGravitySourceSpeed * delta
                };


                job.Schedule(accessArray).Complete();

                accessArray.Dispose();
                up.Dispose();
                upTarget.Dispose();
            }
        }

        [BurstCompile]
        private struct RotateJob : IJobParallelForTransform
        {
            [ReadOnly] public NativeArray<float3> Up;
            [ReadOnly] public NativeArray<float3> UpTarget;
            [ReadOnly] public float Time;

            public void Execute(int index, TransformAccess transform)
            {
                quaternion angle = Calculate.FromToRotation(Up[index], UpTarget[index]);
                transform.rotation = math.slerp(transform.rotation, angle * transform.rotation, Time);
            }
        }
    }
}