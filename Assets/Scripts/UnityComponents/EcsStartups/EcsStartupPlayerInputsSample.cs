using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class EcsStartupPlayerInputsSample : MonoBehaviour
    {
        [SerializeField]
        private InjectData _injectData = new InjectData
        {
            PlayerSpeed = 7,
            JumpForce = 5,
            SlerpRotateViewSpeed = 7
        };
        private EcsWorld _world;
        private EcsSystems _systems;
        private EcsSystems _fixedSystems;
        private Inputs _inputs;
        private PlayerTag _player;

        private void Awake()
        {
            _inputs = new Inputs();
            _player = FindObjectOfType<PlayerTag>();
        }

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _fixedSystems = new EcsSystems(_world);
#if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_systems);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_fixedSystems);
#endif
            _systems
                // register your systems
                .Add(new PlayerInitSystem())
                .Add(new InputSystem())
                .Add(new SlerpRotateSystem())

                // inject service instances
                .Inject(_inputs)
                .Inject(_injectData)
                .Inject(_player);


            _fixedSystems
                // register your systems
                .Add(new PhysicTranslationSystem());

            _systems.ProcessInjects();
            _fixedSystems.ProcessInjects();

            _systems.Init();
            _fixedSystems.Init();
        }

        private void OnEnable()
        {
            _inputs.Enable();
        }

        private void OnDisable()
        {
            _inputs.Disable();
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void FixedUpdate()
        {
            _fixedSystems?.Run();
        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
            }
            if (_fixedSystems != null)
            {
                _fixedSystems.Destroy();
                _fixedSystems = null;
                _world.Destroy();
                _world = null;
            }
        }
    }
}