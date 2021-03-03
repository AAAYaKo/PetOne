using UnityEngine;

namespace PetOne.Services
{
    [CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/Config")]
    public class InjectData : ScriptableObject
    {
        public float RadiusOfGroundScan;
        public float NewGravitySourceScanRadiuce;
        public float SlerpToGravitySourceSpeed;
        public float DefaulFactor = 9.81f;
        public float JumpFactor = 9.81f / 5;
        public float SlerpRotateViewSpeed;
        public float SlowRunSpeed;
        public float FastRunSpeed;
        public float SlowRunPercent = 0.75f;
        public float FastRunPercent = 1;
        public int HeartCount;
        public float ToFootDistance;
        public float LerpAnimatorSpeed;
        public float JumpForce;
        public float StaminaAmount;
        public float StaminaRecoverySpeed;
        public float SpeedOfStaminaSpendOnRun;
        public float HideStaminaTime;
        public Camera Camera
        {
            get
            {
                if(_camera == null)
                    _camera = Camera.main;
                return _camera;
            }
        }
        public Transform CameraTransform
        {
            get
            {
                if (_cameraTransform == null)
                    _cameraTransform = Camera.transform;
                return _cameraTransform;
            }
        }

        private Camera _camera;
        private Transform _cameraTransform;
    }
}