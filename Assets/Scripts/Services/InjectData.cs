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
        public float SlerpRotateViewSpeed;
        public float PlayerSpeed;
        public float JumpForce;
    }
}