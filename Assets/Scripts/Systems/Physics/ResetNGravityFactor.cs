using Leopotam.Ecs;
using PetOne.Components;

namespace PetOne.Systems
{
    internal sealed class ResetNGravityFactor : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<FactorResetTag, JumpData, NGravityAttractor> _filter = null;


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