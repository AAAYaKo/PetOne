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
        //Const
        private const float SLOW_SPEED_PERCENT = 0.75f;
        private const string JUMP_PROPERTY_NAME = "Jump Rising";
        private const string ATTACK_PROPERTY_NAME = "Attack";

        // auto-injected fields.
        //Ecs-data
        private readonly EcsFilter<InputTag, RealTransform, ViewComponent>.Exclude<JumpData, AttackTag> _filter1 = null;
        private readonly EcsFilter<InputTag, RealTransform, ViewComponent>.Exclude<AttackTag> _filter2 = null;
        private readonly EcsFilter<AttackTag> _filter3 = null;
        //Config-data
        private readonly Inputs _inputs = null;
        private readonly PlayerTag _player = null;
        private readonly AnimationEventsProvider _provider = null;
        private readonly InjectData _injectData = null;

        //EcsIgnore
        [EcsIgnoreInject] private float2 _direction = float2.zero;
        [EcsIgnoreInject] private bool _needMove = false;


        public void Init()
        {
            float jumpForce = _injectData.JumpForce;

            _inputs.Player.Move.performed += x =>
            {
                _direction = x.ReadValue<Vector2>();
                _needMove = true;
                foreach (var i in _filter2)
                {
                    ref ViewComponent view = ref _filter2.Get3(i);

                    ref TargetSpeedPercent targetSpeed = ref _filter2.GetEntity(i).Get<TargetSpeedPercent>();
                    targetSpeed.Value = math.length(_direction) * SLOW_SPEED_PERCENT;
                }

            };
            _inputs.Player.Move.canceled += _ =>
            {
                _direction = float2.zero;
                _needMove = false;

                foreach (var i in _filter2)
                {
                    ref EcsEntity entity = ref _filter2.GetEntity(i);
                    ref ViewComponent view = ref _filter2.Get3(i);

                    ref TargetSpeedPercent targetSpeed = ref entity.Get<TargetSpeedPercent>();
                    targetSpeed.Value = 0;

                    view.Entity.Del<TargetRotation>();
                    entity.Del<PhysicTranslation>();
                }
            };
            _inputs.Player.Jump.performed += _ =>
            {
                foreach (var i in _filter2)
                {
                    EcsEntity entity = _filter2.GetEntity(i);

                    ViewComponent view = _filter2.Get3(i);
                    view.Animator.SetBool(JUMP_PROPERTY_NAME, true);

                    ref JumpData inAir = ref entity.Get<JumpData>();
                    inAir.IsInAir = false;

                    ref ForceImpulse force = ref entity.Get<ForceImpulse>();
                    float3 up = _filter2.Get2(i).Value.up;

                    float3 forceVector = _needMove ? GetForceVectorWithMovement(entity, up, jumpForce) : GetForceVectorWithoutMovement(up, jumpForce);
                    force.Value = forceVector;

                    entity.Del<PhysicTranslation>();
                    entity.Get<FactorOverridedTag>();
                }
            };
            _inputs.Player.Jump.canceled += _ =>
            {
                foreach (var i in _filter2)
                {
                    ref EcsEntity entity = ref _filter2.GetEntity(i);
                    if (entity.Has<FactorOverrided>())
                        entity.Get<FactorReset>();
                }
            };
            _inputs.Player.Attack.performed += _ =>
            {
                foreach (var i in _filter1)
                {
                    EcsEntity entity = _filter1.GetEntity(i);
                    ViewComponent view = _filter1.Get3(i);
                    entity.Del<PhysicTranslation>();
                    entity.Get<AttackTag>();
                    view.Animator.SetTrigger(ATTACK_PROPERTY_NAME);
                }
            };

            _provider.AttackEnded += () =>
            {
                foreach (var i in _filter3)
                {
                    EcsEntity entity = _filter3.GetEntity(i);
                    entity.Del<AttackTag>();
                }
            };
        }


        public void Run()
        {
            if (_needMove)
            {
                int count = _filter1.GetEntitiesCount();
                if (count != 0)
                {
                    //Prepare Data For Job
                    //Inputs
                    float delta = Time.fixedDeltaTime;
                    float speed = _injectData.SlowRunSpeed;
                    var combinedInputs = new NativeArray<CombinedInput>(count, Allocator.Persistent);
                    //OutPuts
                    var results = new NativeArray<RigidTransform>(count, Allocator.Persistent);
                    FillJobInputArray(combinedInputs);

                    DoJob(count, combinedInputs, results);
                    Affect(delta, speed, results);

                    results.Dispose();
                    combinedInputs.Dispose();
                }
            }
        }

        private void Affect(float delta, float speed, NativeArray<RigidTransform> results)
        {
            foreach (var i in _filter1)
            {
                ref PhysicTranslation physicTranslation = ref _filter1.GetEntity(i).Get<PhysicTranslation>();
                ref EcsEntity view = ref _filter1.Get3(i).Entity;
                ref TargetRotation targetRotaion = ref view.Get<TargetRotation>();

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
            foreach (var i in _filter1)
            {
                Transform transform = _filter1.Get2(i).Value;
                CombinedInput combinedInput = new CombinedInput
                {
                    Up = transform.up,
                    Rotation = _filter1.Get3(i).Entity.Get<RealTransform>().Value.rotation
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