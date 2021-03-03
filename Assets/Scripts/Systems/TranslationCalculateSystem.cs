using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Calculate float3 translation and quaternion rotation from float 2 input and local up vector
    /// </summary>
    internal sealed class TranslationCalculateSystem : IEcsRunSystem, IEcsInitSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<InputDirection, RealTransform, ViewComponent>.Exclude<JumpData, BlockMoveTag> _filter = null;
        private readonly InjectData _injectData = null;

        [EcsIgnoreInject] private Transform camera;
        [EcsIgnoreInject] private float3 right;
        [EcsIgnoreInject] private float3 forward;
        [EcsIgnoreInject] private float slowDeltaSpeed;
        [EcsIgnoreInject] private float fastDeltaSpeed;


        public void Init()
        {
            camera = _injectData.CameraTransform;
            slowDeltaSpeed = Time.fixedDeltaTime * _injectData.SlowRunSpeed;
            fastDeltaSpeed = Time.fixedDeltaTime * _injectData.FastRunSpeed;
        }

        public void Run()
        {
            right = camera.right;
            forward = camera.forward;


            foreach (var i in _filter)
            {
                //Get player Entity
                var entity = _filter.GetEntity(i);

                // Get data
                var direction = _filter.Get1(i).Value;
                var transform = _filter.Get2(i).Value;
                var up = transform.up;
                // Calculate and aply translatin
                ref var physicTranslation = ref entity.Get<PhysicTranslation>();
                var translation = CalculateTranslation(up, direction);
                physicTranslation.Value = translation * (entity.Has<RunTag>() && !entity.Has<TiredTag>() ? fastDeltaSpeed : slowDeltaSpeed);
                // Change Entity to View Entity
                entity = _filter.Get3(i).Entity;
                // Update target rotation
                var rotation = quaternion.LookRotation(translation, up);
                ref var target = ref entity.Get<TargetRotation>();
                target.Value = rotation;

            }
        }

        private float3 CalculateTranslation(float3 up, float2 direction)
        {
            var calculatedRight = ProjetOnPlain(right, up) * direction.x;
            var calculatedForward = ProjetOnPlain(forward, up) * direction.y;

            return calculatedRight + calculatedForward;
        }

        private float3 ProjetOnPlain(float3 vector, float3 normal) => vector - math.project(vector, normal);
    }
}