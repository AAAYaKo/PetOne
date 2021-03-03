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
        // Ecs fields
        private EcsWorld _world;
        private EcsSystems _update;
        private EcsSystems _fixedUpdate;
        // Data for inject
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
            // New models
            _staminaModel = new PlayerStaminaModel();
            _healthModel = new PlayerHealthModel();
            // Creeate Ecs
            _world = new EcsWorld();
            _update = new EcsSystems(_world);
            _fixedUpdate = new EcsSystems(_world);
        }

        private void Start()
        {
            // Bind models to view models
            FindObjectOfType<StaminaViewModel>().Bind(_staminaModel);
            FindObjectOfType<HealthViewModel>().Bind(_healthModel);

#if UNITY_EDITOR
            CreateInspector();
#endif
            // Prepare systems
            PrepareUpdateSystems();
            PrepareFixedUpdateSystems();
            // Merge systems
            _update.ProcessInjects();
            _fixedUpdate.ProcessInjects();
            // Init systems
            _fixedUpdate.Init();
            _update.Init();
        }

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

        private void PrepareUpdateSystems()
        {
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
        }

        private void PrepareFixedUpdateSystems()
        {
            _fixedUpdate
                // register systems
                .AddNGravity()
                .Add(new TranslationCalculateSystem())
                .Add(new PhysicTranslationSystem())
                .Add(new AddImpulseSystem())
                .Add(new LandingSystem())
                .Add(new MarkFactorReset())
                .Add(new ResetNGravityFactor())
                .Add(new StaminaViewTranslateSystem())

                // register one-frame components
                .OneFrame<ForceImpulse>()
                .OneFrame<FactorResetTag>()

                // inject service instances
                .Inject(_gravityLayer)
                .Inject(_staminaModel)
                .Inject(_injectData)
                .Inject(_sources)
                .Inject(_player);
        }

#if UNITY_EDITOR
        private void CreateInspector()
        {
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_update);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_fixedUpdate);
        }
#endif
    }

    internal static class SystemsExtentions
    {
        /// <summary>
        /// Add Custom NGravity to systems
        /// </summary>
        /// <param name="systems"></param>
        /// <returns></returns>
        public static EcsSystems AddNGravity(this EcsSystems systems)
        {
            systems
                .Add(new NGravitySourcesInit())
                .Add(new NGravitySleep())
                .Add(new NGravityScanGroundSystem())
                .Add(new NGravityAttractForce())
                .Add(new ChangeNGravitySource())
                .Add(new NGravityRotateToNewSource())
                .OneFrame<ChangeSourceTag>();
            return systems;
        }
    }
}