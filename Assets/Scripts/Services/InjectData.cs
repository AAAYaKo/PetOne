using System;
using UnityEngine;

namespace Client
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
        public float ToFootDistance;
        public float LerpAnimatorSpeed;
        public float JumpForce;
    }
}