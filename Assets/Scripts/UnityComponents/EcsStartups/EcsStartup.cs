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
        private NGravitySourceTag[] _sources;


        private void Awake()
        {
            _inputs = new Inputs();
            _player = FindObjectOfType<PlayerTag>();
            _sources = FindObjectsOfType<NGravitySourceTag>();
        }

        private void Start()
        {
            _world = new EcsWorld();
            _update = new EcsSystems(_world);
            _fixedUpdate = new EcsSystems(_world);
#if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_update);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_fixedUpdate);
#endif
            _update
                // register systems
                //Init
                .Add(new PlayerInitSystem())
                .Add(new InputInitSystem())
                //Run
                .Add(new SlerpRotateSystem())
                .Add(new ImpulseAttractSystem())

                // register one-frame components
                .OneFrame<ForceImpulse>()

                // inject
                .Inject(_gravityLayer)
                .Inject(_injectData)
                .Inject(_player)
                .Inject(_inputs);

            _fixedUpdate
                // register systems
                .Add(new NGravitySleep())
                .Add(new NGravitySourcesInit())
                .Add(new NGravityAttractForce())
                .Add(new ChangeNGravitySource())
                .Add(new NGravityAffectSystem())
                .Add(new PhysicTranslationSystem())
                .Add(new NGravityRotateToNewSource())

                // register one-frame components
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