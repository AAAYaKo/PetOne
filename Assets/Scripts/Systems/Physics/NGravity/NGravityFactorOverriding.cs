using Leopotam.Ecs;

namespace Client
{
    sealed class NGravityFactorOverriding : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<FactorOverridedTag, NGravityAttractor> _filter = null;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                ref var attractor = ref _filter.Get2(i);
                ref var factor = ref _filter.GetEntity(i).Get<FactorOverrided>();
                factor.Old = attractor.GravityFactor;
                attractor.GravityFactor = _injectData.JumpFactor;
            }
        }
    }
}