using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] private InjectData _injectData;
        [SerializeField] private LayerMask _gravityLayer = 7;

        private EcsWorld _world;
        private EcsSystems _update;
        private EcsSystems _fixedUpdate;

        private PlayerTag _player;
        private Inputs _inputs;
        private AnimationEventsProvider _provider;
        private NGravitySourceTag[] _sources;


        private void Awake()
        {
            _inputs = new Inputs();
            _player = FindObjectOfType<PlayerTag>();
            _provider = FindObjectOfType<AnimationEventsProvider>();
            _sources = FindObjectsOfType<NGravitySourceTag>();
        }

        private void Start()
        {
            _world = new EcsWorld();
            _update = new EcsSystems(_world);
            _fixedUpdate = new EcsSystems(_world);

#if UNITY_EDITOR
            CreateInspector();
#endif
            _update
                // register systems
                .Add(new PlayerInitSystem())
                .Add(new InputSystem())
                .Add(new SlerpRotateSystem())
                .Add(new AnimatorLandingSystem())
                .Add(new AnimatorWalkingSystem())

                // register one-frame components
                .OneFrame<LandedTag>()

                // inject
                .Inject(_gravityLayer)
                .Inject(_injectData)
                .Inject(_provider)
                .Inject(_player)
                .Inject(_inputs);

            _fixedUpdate
                // register systems
                .Add(new NGravitySourcesInit())
                .Add(new NGravitySleep())
                .Add(new NGravityScanGroundSystem())
                .Add(new NGravityAttractForce())
                .Add(new ChangeNGravitySource())
                .Add(new NGravityRotateToNewSource())

                .Add(new PhysicTranslationSystem())
                .Add(new ImpulseAttractSystem())
                .Add(new LandingSystem())
                .Add(new MarkFactorReset())
                .Add(new ResetNGravityFactor())

                // register one-frame components
                .OneFrame<ForceImpulse>()
                .OneFrame<FactorReset>()
                .OneFrame<ChangeSourceTag>()

                // inject service instances
                .Inject(_gravityLayer)
                .Inject(_injectData)
                .Inject(_sources);

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
}