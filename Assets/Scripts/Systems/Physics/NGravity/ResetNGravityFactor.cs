using Leopotam.Ecs;

namespace Client
{
    sealed class ResetNGravityFactor : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<FactorReset, FactorOverrided, NGravityAttractor> _filter = null;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                float old = _filter.Get2(i).Old;
                ref var attractor = ref _filter.Get3(i);
                attractor.GravityFactor = old;
                _filter.GetEntity(i).Del<FactorOverrided>();
            }
        }
    }
}