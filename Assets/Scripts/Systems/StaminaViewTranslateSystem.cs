using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Translate view to new position related to player position and camera view
    /// </summary>
    internal sealed class StaminaViewTranslateSystem : IEcsRunSystem, IEcsInitSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<RealTransform, Stamina> _filter = null;
        private readonly InjectData _injectData = null;
        private readonly PlayerStaminaModel _model = null;

        [EcsIgnoreInject] private Camera camera;
        [EcsIgnoreInject] private Transform cameraTransform;
        [EcsIgnoreInject] private Vector3 oldEntityPosition;
        [EcsIgnoreInject] private Vector3 oldCameraPosition;


        public void Init()
        {
            // Cash links
            camera = _injectData.Camera;
            cameraTransform = _injectData.CameraTransform;
        }

        void IEcsRunSystem.Run()
        {
            var cameraPosition = cameraTransform.position;
            foreach (var i in _filter)
            {
                var entityPosition = _filter.Get1(i).Value.position;
                if(entityPosition != oldEntityPosition || cameraPosition != oldCameraPosition)
                {
                    // Update positions
                    oldEntityPosition = entityPosition;
                    oldCameraPosition = cameraPosition;
                    _model.Position = camera.WorldToScreenPoint(entityPosition);
                }
            }
        }
    }
}