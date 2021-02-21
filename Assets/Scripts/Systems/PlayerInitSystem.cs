using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Linkers;
using PetOne.Services;
using PetOne.Ui;
using UnityEngine;

namespace PetOne.Systems
{
    internal sealed class PlayerInitSystem : IEcsInitSystem
    {
        // auto-injected fields.
        private readonly EcsWorld _world = null;
        private readonly PlayerConfig _playerConfig = null;
        private readonly InjectData _injectData = null;

        [EcsIgnoreInject] private readonly UiRepository repository = UiRepository.Instance;


        public void Init()
        {
            var player = _world.NewEntity();
            var playerTransform = _playerConfig.SelfTransform;
            var playerRigidbody = _playerConfig.SelfRigidbody;
            player.Get<InputTag>();
            ViewInit(player);
            RealTransformInit(player, playerTransform);
            PhysicBodyInit(player, playerRigidbody);
            GravityAttractorInit(player);
            StaminaInit(player);
        }

        private void StaminaInit(EcsEntity player)
        {
            ref var stamina = ref player.Get<Stamina>();
            stamina.Amount = _injectData.StaminaAmount;
            stamina.RecoverySpeed = _injectData.StaminaRecoverySpeed;
            repository.StaminaAmount = stamina.Amount;
        }

        private void GravityAttractorInit(EcsEntity player)
        {
            ref var attractor = ref player.Get<NGravityAttractor>();
            attractor.GravityFactor = _injectData.DefaulFactor;
            player.Get<ChangeSourceTag>();
        }

        private void ViewInit(EcsEntity player)
        {
            var viewEntity = _world.NewEntity();
            ref var transform = ref viewEntity.Get<RealTransform>();
            transform.Value = _playerConfig.ViewTransform;

            ref var viewComponent = ref player.Get<ViewComponent>();
            viewComponent.Entity = viewEntity;
            viewComponent.Animator = _playerConfig.ViewAnimator;
        }

        private void PhysicBodyInit(EcsEntity player, Rigidbody playerRigidbody)
        {
            ref var body = ref player.Get<PhysicBody>();
            body.Value = playerRigidbody;
        }

        private void RealTransformInit(EcsEntity player, Transform playerTransform)
        {
            ref var realTransform = ref player.Get<RealTransform>();
            realTransform.Value = playerTransform;
        }
    }
}