using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class EcsStartupGravitySample : MonoBehaviour
    {
        [SerializeField] private int _attractorsCount = 32;
        [SerializeField]
        private InjectData _injectData = new InjectData
        {
            RadiusOfGroundScan = 0.5f,
            NewGravitySourceScanRadiuce = 100,
            SlerpToGravitySourceSpeed = 10
        };
        [SerializeField] private GameObject _attractor;
        [SerializeField] private LayerMask _gravityLayer = 7;
        private EcsWorld _world;
        private EcsSystems _fixedSystems;
        private NGravitySourceTag[] _sources;


        private void Awake()
        {
            _sources = FindObjectsOfType<NGravitySourceTag>();
        }

        private void Start()
        {
            _world = new EcsWorld();
            _fixedSystems = new EcsSystems(_world);
#if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_fixedSystems);
#endif
            _fixedSystems
                // register your systems
                .Add(new TestNGravityAttractorsInit())
                .Add(new NGravitySourcesInit())
                .Add(new ChangeNGravitySource())
                .Add(new NGravityAffectSystem())
                .Add(new NGravityAttractForce())
                .Add(new NGravityRotate())

                // register one-frame components
                .OneFrame<ChangeSourceTag>()
                // .OneFrame<TestComponent2> ()

                // inject service instances
                .Inject(_injectData)
                .Inject(_attractorsCount)
                .Inject(_gravityLayer)
                .Inject(_attractor)
                .Inject(_sources)
                .Init();
        }

        private void FixedUpdate()
        {
            _fixedSystems?.Run();
        }

        private void OnDestroy()
        {
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