using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Linkers;
using PetOne.Services;
using PetOne.Systems;
using PetOne.Ui.ViewModel;
using UnityEngine;

namespace PetOne.Startups
{
    sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] private InjectData _injectData;
        [SerializeField] private LayerMask _gravityLayer = 7;

        private EcsWorld _world;
        private EcsSystems _update;
        private EcsSystems _fixedUpdate;

        private PlayerConfig _player;
        private Inputs _inputs;
        private NGravitySourceConfig[] _sources;
        private PlayerStaminaModel _staminaModel;
        private PlayerHealthModel _healthModel;


        private void Awake()
        {
            _inputs = new Inputs();
            _player = FindObjectOfType<PlayerConfig>();
            _sources = FindObjectsOfType<NGravitySourceConfig>();
            _staminaModel = new PlayerStaminaModel();
            _healthModel = new PlayerHealthModel();
        }

        private void Start()
        {
            _world = new EcsWorld();
            _update = new EcsSystems(_world);
            _fixedUpdate = new EcsSystems(_world);

            FindObjectOfType<StaminaViewModel>().Bind(_staminaModel);
            FindObjectOfType<HealthViewModel>().Bind(_healthModel);

#if UNITY_EDITOR
            CreateInspector();
#endif
            _update
                // register systems
                .Add(new PlayerInitSystem())
                .Add(new InputSystem())
                .Add(new JumpSystem())
                .Add(new SlerpRotateSystem())
                .Add(new AnimatorLandingSystem())
                .Add(new AnimatorWalkingSystem())
                .Add(new StaminaSpendSystem())
                .Add(new StaminaRestorationSystem())
                .Add(new StaminaHideSystem())
                .Add(new TargetSpeedPercentChangeSystem())

                // register one-frame components
                .OneFrame<JumpQueryTag>()
                .OneFrame<LandedTag>()
                .OneFrame<TargetSpeedPercentChangedTag>()

                // inject
                .Inject(_gravityLayer)
                .Inject(_staminaModel)
                .Inject(_healthModel)
                .Inject(_injectData)
                .Inject(_player)
                .Inject(_inputs);

            _fixedUpdate
                // register systems
                .AddNGravity()
                .Add(new TranslationCalculateSystem())
                .Add(new PhysicTranslationSystem())
                .Add(new ImpulseAttractSystem())
                .Add(new LandingSystem())
                .Add(new MarkFactorReset())
                .Add(new ResetNGravityFactor())
                .Add(new StaminaViewTranslateSystem())

                // register one-frame components
                .OneFrame<ForceImpulse>()
                .OneFrame<FactorResetTag>()
                .OneFrame<ChangeSourceTag>()

                // inject service instances
                .Inject(_gravityLayer)
                .Inject(_staminaModel)
                .Inject(_injectData)
                .Inject(_sources)
                .Inject(_player);

            _update.ProcessInjects();
            _fixedUpdate.ProcessInjects();

            _fixedUpdate.Init();
            _update.Init();
        }

#if UNITY_EDITOR
        private void CreateInspector()
        {
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_update);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_fixedUpdate);
        }
#endif

        private void Update()
        {
            _update?.Run();
        }

        private void FixedUpdate()
        {
            _fixedUpdate?.Run();
        }

        private void OnEnable()
        {
            _inputs.Enable();
        }

        private void OnDisable()
        {
            _inputs.Disable();
        }

        private void OnDestroy()
        {
            if (_update != null)
            {
                _update.Destroy();
                _update = null;
            }
            if (_fixedUpdate != null)
            {
                _fixedUpdate.Destroy();
                _fixedUpdate = null;
            }
            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }

    }

    internal static class SystemsExtentions
    {
        public static EcsSystems AddNGravity(this EcsSystems systems)
        {
            systems
                .Add(new NGravitySourcesInit())
                .Add(new NGravitySleep())
                .Add(new NGravityScanGroundSystem())
                .Add(new NGravityAttractForce())
                .Add(new ChangeNGravitySource())
                .Add(new NGravityRotateToNewSource());
            return systems;
        }
    }
}