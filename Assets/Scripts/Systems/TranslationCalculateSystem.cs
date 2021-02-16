using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class TranslationCalculateSystem : IEcsRunSystem, IEcsInitSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<InputDirection, RealTransform, ViewComponent>.Exclude<JumpData> _filter = null;
        private readonly PlayerConfig _player = null;
        private readonly InjectData _injectData = null;

        [EcsIgnoreInject] private Transform camera;
        [EcsIgnoreInject] private float3 right;
        [EcsIgnoreInject] private float3 forward;
        [EcsIgnoreInject] private float slowDeltaSpeed;
        [EcsIgnoreInject] private float fastDeltaSpeed;


        public void Init()
        {
            camera = _player.CameraTransform;
            slowDeltaSpeed = Time.fixedDeltaTime * _injectData.SlowRunSpeed;
            fastDeltaSpeed = Time.fixedDeltaTime * _injectData.FastRunSpeed;
        }

        public void Run()
        {
            right = camera.right;
            forward = camera.forward;


            foreach (var i in _filter)
            {
                EcsEntity entity = _filter.GetEntity(i);

                if (entity.Has<AttackTag>())
                    entity.Del<PhysicTranslation>();
                else
                {
                    var direction = _filter.Get1(i).Value;
                    var transform = _filter.Get2(i).Value;
                    var up = transform.up;

                    float3 translation = CalculateTranslation(up, direction);
                    ref var physicTranslation = ref entity.Get<PhysicTranslation>();

                    physicTranslation.Value = translation * (entity.Has<RunTag>() && !entity.Has<TiredTag>() ? fastDeltaSpeed : slowDeltaSpeed);

                    entity = _filter.Get3(i).Entity;
                    quaternion rotation = quaternion.LookRotation(translation, up);
                    ref var target = ref entity.Get<TargetRotation>();
                    target.Value = rotation;
                }
            }
        }

        private float3 CalculateTranslation(float3 up, float2 direction)
        {
            float3 calculatedRight = ProjetOnPlain(right, up) * direction.x;
            float3 calculatedForward = ProjetOnPlain(forward, up) * direction.y;

            return calculatedRight + calculatedForward;
        }

        private float3 ProjetOnPlain(float3 vector, float3 normal) => vector - math.project(vector, normal);
    }
}