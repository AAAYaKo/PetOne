using Leopotam.Ecs;
using PetOne.Components;

namespace PetOne.Systems
{
    /// <summary>
    /// Reset Gravity Factor by tag
    /// </summary>
    internal sealed class ResetNGravityFactor : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<JumpData, NGravityAttractor, FactorResetTag> _filter = null;


        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                float old = _filter.Get1(i).OldFactor;
                ref var attractor = ref _filter.Get2(i);
                attractor.GravityFactor = old;
            }
        }
    }
}