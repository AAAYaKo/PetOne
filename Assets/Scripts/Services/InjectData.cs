using System;

namespace Client
{
    [Serializable]
    public class InjectData
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