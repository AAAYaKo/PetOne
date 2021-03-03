using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using Unity.Mathematics;

namespace PetOne.Systems
{
    /// <summary>
    /// Jump by query
    /// </summary>
    internal sealed class JumpSystem : IEcsRunSystem
    {
        private const string JUMP_PROPERTY_NAME = "Jump Rising";

        // auto-injected fields.
        private readonly EcsFilter<ViewComponent, NGravityAttractor, JumpQueryTag>.Exclude<JumpData> _filter = null;
        private readonly InjectData _injectData = null;


        void IEcsRunSystem.Run()
        {
            float jumpForce = _injectData.JumpForce;

            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);

                _filter.Get1(i).Animator.SetBool(JUMP_PROPERTY_NAME, true);
                var up = _filter.Get2(i).NormalToGround;
                //Add Jump data
                ref var attractor = ref _filter.Get2(i);
                ref var jump = ref entity.Get<JumpData>();
                jump.IsInAir = false;
                jump.OldFactor = attractor.GravityFactor;
                attractor.GravityFactor = _injectData.JumpFactor;
                //Add force
                ref var force = ref entity.Get<ForceImpulse>();
                var forceVector = entity.Has<InputDirection>() ? 
                    GetForceWithMovement(entity, up, jumpForce) : GetForceWithoutMovement(up, jumpForce);
                force.Value = forceVector;

                entity.Del<PhysicTranslation>();
            }
        }

        private float3 GetForceWithoutMovement(float3 up, float force) => up * force;
        private float3 GetForceWithMovement(EcsEntity entity, float3 up, float force)
        {
            var translation = entity.Get<PhysicTranslation>();
            float3 forceVector = up + math.normalize(translation.Value);
            forceVector = forceVector * force;
            return forceVector;
        }
    }
}