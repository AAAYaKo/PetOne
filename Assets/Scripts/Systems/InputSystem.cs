using Leopotam.Ecs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class InputSystem : IEcsInitSystem, IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<InputTag, RealTransform, ViewComponent> _filter = null;
        private readonly Inputs _inputs = null;
        private readonly PlayerTag _player = null;
        private readonly InjectData _injectData = null;

        [EcsIgnoreInject] private float2 _direction = float2.zero;
        [EcsIgnoreInject] private bool _needMove = false;


        public void Init()
        {
            float jumpForce = _injectData.JumpForce;

            _inputs.Player.Move.performed += x =>
            {
                _direction = x.ReadValue<Vector2>();
                _needMove = true;
            };
            _inputs.Player.Move.canceled += _ =>
            {
                _direction = float2.zero;
                _needMove = false;

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

                    entity.Get<FactorOverridedTag>();
                }
            };

            _inputs.Player.Jump.canceled += _ =>
            {
                foreach (var i in _filter)
                {
                    ref EcsEntity entity = ref _filter.GetEntity(i);
                    entity.Get<FactorReset>();
                }
            };
        }


        public void Run()
        {
            if (_needMove)
            {
                int count = _filter.GetEntitiesCount();
                if (count != 0)
                {
                    //Prepare Data For Job
                    //Inputs
                    float delta = Time.fixedDeltaTime;
                    float speed = _injectData.PlayerSpeed;
                    var combinedInputs = new NativeArray<CombinedInput>(count, Allocator.Persistent);
                    FillJobInputArray(combinedInputs);
                    //OutPuts
                    var results = new NativeArray<RigidTransform>(count, Allocator.Persistent);

                    DoJob(count, combinedInputs, results);
                    Affect(delta, speed, results);

                    results.Dispose();
                    combinedInputs.Dispose();
                }
            }
        }

        private void Affect(float delta, float speed, NativeArray<RigidTransform> results)
        {
            foreach (var i in _filter)
            {
                ref PhysicTranslation physicTranslation = ref _filter.GetEntity(i).Get<PhysicTranslation>();
                ref EcsEntity entity = ref _filter.Get3(i).Entity;
                ref TargetRotation targetRotaion = ref entity.Get<TargetRotation>();

                physicTranslation.Value = delta * speed * results[i].pos;
                targetRotaion.Value = results[i].rot;
            }
        }

        private void DoJob(int count, NativeArray<CombinedInput> combinedInputs, NativeArray<RigidTransform> results)
        {
            var job = new CalculateJob
            {
                Direction = _direction,
                CombinedInput = combinedInputs,
                CameraRight = _player.CameraTransform.right,
                CameraForward = _player.CameraTransform.forward,
                Result = results
            };
            job
            .Schedule(count, 1)
            .Complete();
        }

        private void FillJobInputArray(NativeArray<CombinedInput> combinedInputs)
        {
            foreach (var i in _filter)
            {
                Transform transform = _filter.Get2(i).Value;
                CombinedInput combinedInput = new CombinedInput
                {
                    Up = transform.up,
                    Rotation = _filter.Get3(i).Entity.Get<RealTransform>().Value.rotation
                };
                combinedInputs[i] = combinedInput;
            }
        }

        private float3 GetForceVectorWithoutMovement(float3 up, float force) => up * force;

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
            [ReadOnly] public float3 CameraForward;
            [ReadOnly] public NativeArray<CombinedInput> CombinedInput;
            //Outputs
            ///<summary>
            /// Position = Translation
            ///</summary>
            public NativeArray<RigidTransform> Result;

            public void Execute(int index)
            {
                CalculateTranslation(index);
                CalculateRotation(index);
            }

            private void CalculateRotation(int index)
            {
                bool2 isZero = Direction == float2.zero;
                if (isZero.x && isZero.y)
                {
                    RigidTransform outPut = Result[index];
                    outPut.rot = CombinedInput[index].Rotation;
                    Result[index] = outPut;
                }
                else
                {
                    RigidTransform outPut = Result[index];
                    outPut.rot = quaternion.LookRotation(outPut.pos, CombinedInput[index].Up);
                    Result[index] = outPut;
                }
            }

            private void CalculateTranslation(int index)
            {
                float3 forward = ProjetOnPlain(CameraForward, CombinedInput[index].Up) * Direction.y;
                float3 right = ProjetOnPlain(CameraRight, CombinedInput[index].Up) * Direction.x;

                float3 translation = right + forward;

                RigidTransform outPut = Result[index];
                outPut.pos = translation;
                Result[index] = outPut;
            }

            private float3 ProjetOnPlain(float3 vector, float3 normal)
            {
                return vector - math.project(vector, normal);
            }
        }

        private struct CombinedInput
        {
            public float3 Up;
            public quaternion Rotation;
        }
    }
}