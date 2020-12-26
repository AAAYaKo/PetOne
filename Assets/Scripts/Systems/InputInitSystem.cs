using Leopotam.Ecs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class InputInitSystem : IEcsInitSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<InputTag, RealTransform, ViewComponent> _filter = null;
        private readonly Inputs _inputs = null;
        private readonly PlayerTag _player = null;
        private readonly InjectData _injectData = null;


        public void Init()
        {
            float delta = Time.fixedDeltaTime;
            float speed = _injectData.PlayerSpeed;
            float jumpForce = _injectData.JumpForce;

            _inputs.Player.Move.performed += x =>
            {
                int count = _filter.GetEntitiesCount();
                if (count != 0)
                {
                    //Prepare Data For Job
                    //Inputs
                    Vector2 input = x.ReadValue<Vector2>();
                    var up = new NativeArray<float3>(count, Allocator.Persistent);
                    var view = new NativeArray<quaternion>(count, Allocator.Persistent);
                    foreach (var i in _filter)
                    {
                        up[i] = _filter.Get2(i).Value.up;
                        view[i] = _filter.Get3(i).Entity.Get<RealTransform>().Value.rotation;
                    }
                    //OutPuts
                    var rotation = new NativeArray<quaternion>(count, Allocator.Persistent);
                    var translation = new NativeArray<float3>(1, Allocator.Persistent);

                    //Do Job
                    var job = new CalculateJob
                    {
                        Direction = input,
                        PlayerUp = up,
                        CameraRight = _player.CameraTransform.right,
                        ViewRotation = view,
                        Translation = translation,
                        Rotation = rotation
                    };
                    job
                    .Schedule(view.Length, 1)
                    .Complete();

                    //Affect
                    foreach (var i in _filter)
                    {
                        ref PhysicTranslation physicTranslation = ref _filter.GetEntity(i).Get<PhysicTranslation>();
                        ref EcsEntity entity = ref _filter.Get3(i).Entity;
                        ref TargetRotation targetRotaion = ref entity.Get<TargetRotation>();

                        physicTranslation.Value = delta * speed * translation[0];
                        targetRotaion.Value = rotation[0];
                    }

                    translation.Dispose();
                    rotation.Dispose();
                    view.Dispose();
                    up.Dispose();
                }
            };
            _inputs.Player.Move.canceled += _ =>
            {
                foreach (var i in _filter)
                {
                    ref EcsEntity entity = ref _filter.GetEntity(i);
                    ref ViewComponent view = ref _filter.Get3(i);
                    view.Entity.Del<TargetRotation>();
                    entity.Del<PhysicTranslation>();
                }
            };
            _inputs.Player.Jump.performed += _ =>
            {
                foreach (var i in _filter)
                {
                    ref EcsEntity entity = ref _filter.GetEntity(i);
                    ref ForceImpulse force = ref entity.Get<ForceImpulse>();
                    float3 up = _filter.Get2(i).Value.up;
                    bool isTranslating = entity.Has<PhysicTranslation>();

                    float3 forceVector = isTranslating ? GetForceVectorWithMovement(entity, up, jumpForce) : GetForceVectorWithoutMovement(up, jumpForce);
                    force.Value = forceVector;
                }
            };
        }

        private float3 GetForceVectorWithoutMovement(float3 up, float force)
        {
            return up * force;
        }

        private float3 GetForceVectorWithMovement(EcsEntity entity, float3 up, float force)
        {
            float3 forceVector;
            PhysicTranslation translation = entity.Get<PhysicTranslation>();
            forceVector = math.normalize(translation.Value) + up;
            forceVector = math.normalize(forceVector) * force;
            return forceVector;
        }

        [BurstCompile]
        private struct CalculateJob : IJobParallelFor
        {
            //Inputs
            [ReadOnly] public float2 Direction; //Player Input
            [ReadOnly] public float3 CameraRight;
            [ReadOnly] public NativeArray<float3> PlayerUp;
            [ReadOnly] public NativeArray<quaternion> ViewRotation;
            //Outputs
            public NativeArray<float3> Translation;
            public NativeArray<quaternion> Rotation;

            public void Execute(int index)
            {
                CalculateTranslation(index);
                CalculateRotation(index);
            }

            private void CalculateRotation(int index)
            {
                bool2 isZero = Direction == float2.zero;
                if (isZero.x && isZero.y)
                    Rotation[index] = ViewRotation[index];

                Rotation[index] = quaternion.LookRotation(Translation[index], PlayerUp[index]);
            }

            private void CalculateTranslation(int index)
            {
                float3 forward = math.cross(CameraRight, PlayerUp[index]);
                quaternion rotationAngle = Calculate.FromToRotation(math.forward(), forward);

                float3 translation = new float3(Direction.x, 0, Direction.y);

                Translation[0] = math.rotate(rotationAngle, translation);
            }
        }
    }
}