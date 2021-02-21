using Unity.Mathematics;

namespace PetOne.Components
{
    internal struct NGravityAttractor
    {
        public float GravityFactor;
        public float3 NormalToGround;
        public float DistanceToGround;
        public float TimeToSleep;
    }
}