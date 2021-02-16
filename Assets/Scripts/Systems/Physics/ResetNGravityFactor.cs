using Leopotam.Ecs;

namespace Client
{
    sealed class ResetNGravityFactor : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<FactorReset, JumpData, NGravityAttractor> _filter = null;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                float old = _filter.Get2(i).OldFactor;
                ref var attractor = ref _filter.Get3(i);
                attractor.GravityFactor = old;
            }
        }
    }
}