using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class PlayerInitSystem : IEcsInitSystem
    {
        // auto-injected fields.
        private readonly EcsWorld _world = null;
        private readonly PlayerTag _player = null;
        private readonly InjectData _injectData = null;

        public void Init()
        {
            EcsEntity player = _world.NewEntity();
            Transform playerTransform = _player.PlayerTransform;
            Rigidbody playerRigidbody = _player.PlayerRigidbody;
            player.Get<InputTag>();
            ViewTransformInit(player);
            RealTransformInit(player, playerTransform);
            PhysicBodyInit(player, playerRigidbody);
            GravityAttractorInit(player);
        }

        private void GravityAttractorInit(EcsEntity player)
        {
            ref NGravityAttractor attractor = ref player.Get<NGravityAttractor>();
            attractor.GravityFactor = _injectData.DefaulFactor;
            player.Get<ChangeSourceTag>();
        }

        private void ViewTransformInit(EcsEntity player)
        {
            EcsEntity viewEntity = _world.NewEntity();
            ref RealTransform transform = ref viewEntity.Get<RealTransform>();
            transform.Value = _player.ViewTransform;

            ref ViewComponent viewComponent = ref player.Get<ViewComponent>();
            viewComponent.Entity = viewEntity;
            viewComponent.Animator = _player.ViewAnimator;
        }

        private void PhysicBodyInit(EcsEntity player, Rigidbody playerRigidbody)
        {
            ref PhysicBody body = ref player.Get<PhysicBody>();
            body.Value = playerRigidbody;
        }

        private void RealTransformInit(EcsEntity player, Transform playerTransform)
        {
            ref RealTransform realTransform = ref player.Get<RealTransform>();
            realTransform.Value = playerTransform;
        }
    }
}