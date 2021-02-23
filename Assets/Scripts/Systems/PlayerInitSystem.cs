using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Linkers;
using PetOne.Services;

namespace PetOne.Systems
{
    internal sealed class PlayerInitSystem : IEcsInitSystem
    {
        private const int HEALTH_PER_HEART = 4;

        // auto-injected fields.
        private readonly EcsWorld _world = null;
        private readonly PlayerConfig _playerConfig = null;
        private readonly InjectData _injectData = null;
        private readonly PlayerStaminaModel _staminaModel = null;
        private readonly PlayerHealthModel _healthModel = null;


        public void Init()
        {
            var player = _world.NewEntity();
            
            player.Get<PlayerTag>();
            ViewInit(player);
            RealTransformInit(player);
            PhysicBodyInit(player);
            GravityAttractorInit(player);
            StaminaInit(player);
            HealthInit(player);
            ColliderInit(player);
        }

        private void StaminaInit(EcsEntity player)
        {
            ref var stamina = ref player.Get<Stamina>();
            stamina.Amount = _injectData.StaminaAmount;
            stamina.RecoverySpeed = _injectData.StaminaRecoverySpeed;
            _staminaModel.Amount = stamina.Amount;
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
            viewComponent.EventsProvider = _playerConfig.AnimationEventsProvider;
        }

        private void PhysicBodyInit(EcsEntity player)
        {
            var playerRigidbody = _playerConfig.SelfRigidbody;
            ref var body = ref player.Get<PhysicBody>();
            body.Value = playerRigidbody;
        }

        private void RealTransformInit(EcsEntity player)
        {
            var playerTransform = _playerConfig.SelfTransform;
            ref var realTransform = ref player.Get<RealTransform>();
            realTransform.Value = playerTransform;
        }

        private void HealthInit(EcsEntity player)
        {
            ref var health = ref player.Get<Health>();
            health.Max = _injectData.HeartCount * HEALTH_PER_HEART;
            health.Current = health.Max;
            _healthModel.Max = health.Max;
            _healthModel.Current = health.Current;
        }

        private void ColliderInit(EcsEntity player)
        {
            ref var collider = ref player.Get<Components.Collider>();
            collider.Provider = _playerConfig.CollisionProvider;
            collider.Reference = _playerConfig.Collider;
        }
    }
}