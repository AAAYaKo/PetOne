using Leopotam.Ecs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Client
{
    sealed class SlerpRotateSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<TargetRotation, RealTransform> _filter = null;

        private readonly float _slerpSpeed = 0;

        void IEcsRunSystem.Run()
        {
            float time = Time.deltaTime * _slerpSpeed;
            DoJob(time);
        }

        private void DoJob(float time)
        {
            PrepareJobData(out var to, out var transformAccess);
            var job = new SlerpRotateJob
            {
                To = to,
                Time = time
            };
            job
                .Schedule(transformAccess)
                .Complete();

            to.Dispose();
            transformAccess.Dispose();
        }

        private void PrepareJobData(out NativeArray<quaternion> to, out TransformAccessArray transformAccess)
        {
            int count = _filter.GetEntitiesCount();

            to = new NativeArray<quaternion>(count, Allocator.Persistent);
            Transform[] transforms = new Transform[count];
            foreach (var i in _filter)
            {
                to[i] = _filter.Get1(i).Value;
                transforms[i] = _filter.Get2(i).Value;
            }
            transformAccess = new TransformAccessArray(transforms);
        }

        private struct SlerpRotateJob : IJobParallelForTransform
        {
            [ReadOnly] public NativeArray<quaternion> To;
            [ReadOnly] public float Time;

            public void Execute(int index, TransformAccess transform)
            {
                var from = transform.rotation;
                var to = To[index];
                transform.rotation = math.slerp(from, to, Time);
            }
        }
    }
}