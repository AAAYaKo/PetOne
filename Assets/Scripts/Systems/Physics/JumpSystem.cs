using Leopotam.Ecs;
using Unity.Mathematics;

namespace Client
{
    sealed class JumpSystem : IEcsRunSystem
    {
        private const string JUMP_PROPERTY_NAME = "Jump Rising";
        private const int FAST_MULTIPLIER = 2;
        private const int SLOW_MULTIPLIER = 1;

        // auto-injected fields.
        private readonly EcsFilter<ViewComponent, NGravityAttractor, JumpQueryTag>.Exclude<JumpData> _filter = null;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            float jumpForce = _injectData.JumpForce;

            foreach (var i in _filter)
            {
                EcsEntity entity = _filter.GetEntity(i);

                _filter.Get1(i).Animator.SetBool(JUMP_PROPERTY_NAME, true);
                float3 up = _filter.Get2(i).NormalToGround;

                ref var attractor = ref _filter.Get2(i);
                ref var jump = ref entity.Get<JumpData>();
                jump.IsInAir = false;
                jump.OldFactor = attractor.GravityFactor;
                attractor.GravityFactor = _injectData.JumpFactor;

                ref var force = ref entity.Get<ForceImpulse>();
                float3 forceVector = entity.Has<InputDirection>() ? 
                    GetForceVectorWithMovement(entity, up, jumpForce) : GetForceVectorWithoutMovement(up, jumpForce);
                force.Value = forceVector;

                entity.Del<PhysicTranslation>();
            }
        }

        private float3 GetForceVectorWithoutMovement(float3 up, float force) => up * force;
        private float3 GetForceVectorWithMovement(EcsEntity entity, float3 up, float force)
        {
            var translation = entity.Get<PhysicTranslation>();
            float3 forceVector = up + math.normalize(translation.Value) * (entity.Has<RunTag>() ? FAST_MULTIPLIER : SLOW_MULTIPLIER);
            forceVector = forceVector * force;
            return forceVector;
        }
    }
}