using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class StaminaViewTranslateSystem : IEcsRunSystem, IEcsInitSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<RealTransform, Stamina> _filter = null;
        private readonly InjectData _injectData = null;
        [EcsIgnoreInject] private readonly UiRepository repository = UiRepository.Instance;

        [EcsIgnoreInject] private Camera camera;
        [EcsIgnoreInject] private Transform cameraTransform;
        [EcsIgnoreInject] private Vector3 oldEntityPosition;
        [EcsIgnoreInject] private Vector3 oldCameraPosition;


        public void Init()
        {
            camera = _injectData.Camera;
            cameraTransform = _injectData.CameraTransform;
        }

        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                Vector3 entityPosition = _filter.Get1(i).Value.position;
                Vector3 cameraPosition = cameraTransform.position;
                if(entityPosition != oldEntityPosition || cameraPosition != oldCameraPosition)
                {
                    repository.StaminaViewPosition = camera.WorldToScreenPoint(entityPosition);
                    oldEntityPosition = entityPosition;
                    oldCameraPosition = cameraPosition;
                }
            }
        }
    }
}