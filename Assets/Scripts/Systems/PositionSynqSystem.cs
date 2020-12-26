using Leopotam.Ecs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Client
{
    //sealed class PositionSynqSystem : IEcsRunSystem
    //{
        //// auto-injected fields.
        //private readonly EcsFilter<RealTransform, VirtualTransform> _filter = null;

        //void IEcsRunSystem.Run()
        //{
        //    InitJobData(out var realTransforms, out var virtualTransforms);

        //    var job = new SynqJob
        //    {
        //        VirtualTransforms = virtualTransforms
        //    };

        //    job
        //        .Schedule(realTransforms)
        //        .Complete();

        //    foreach (int i in _filter)
        //    {
        //        ref VirtualTransform virtualT = ref _filter.Get2(i);
        //        virtualT = virtualTransforms[i];
        //    }

        //    realTransforms.Dispose();
        //    virtualTransforms.Dispose();
        //}

        //private void InitJobData(out TransformAccessArray realTransforms, out NativeArray<VirtualTransform> virtualTransforms)
        //{
        //    int count = _filter.GetEntitiesCount();
        //    Transform[] realTs = new Transform[count];
        //    virtualTransforms = new NativeArray<VirtualTransform>(count, Allocator.Persistent);

        //    foreach (int i in _filter)
        //    {
        //        ref RealTransform realT = ref _filter.Get1(i);
        //        realTs[i] = realT.Value;
        //    }

        //    realTransforms = new TransformAccessArray(realTs);
        //}

        //[BurstCompile]
        //private struct SynqJob : IJobParallelForTransform
        //{
        //    public NativeArray<VirtualTransform> VirtualTransforms;

        //    public void Execute(int index, TransformAccess transform)
        //    {
        //        quaternion rotation = transform.rotation;
        //        float3 up = math.mul(rotation, math.up());
        //        float3 right = math.mul(rotation, math.right());
        //        float3 forward = math.mul(rotation, math.forward()); 
        //        VirtualTransform virtualTransform = new VirtualTransform
        //        {
        //            Position = transform.position,
        //            Rotation = transform.rotation,
        //            Up = up,
        //            Right = right,
        //            Forward = forward
        //        };
        //        VirtualTransforms[index] = virtualTransform;
        //    }
        //}
    //}
}