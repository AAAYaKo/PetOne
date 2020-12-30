using Unity.Mathematics;

namespace Client
{
    struct NGravityAttractor
    {
        public float GravityFactor;
        public float3 NormalToGround;
        public float DistanceToGround;
        public float Time;
    }
}