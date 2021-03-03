using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Spends stamina
    /// </summary>
    internal sealed class StaminaSpendSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<Stamina, RunTag>.Exclude<TiredTag, JumpData> _filter = null;
        private readonly InjectData _injectData = null;
        private readonly PlayerStaminaModel _model = null;


        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime * _injectData.SpeedOfStaminaSpendOnRun;
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                if (!entity.Has<BlockMoveTag>())
                {
                    ref var stamina = ref _filter.Get1(i);
                    // Spend stamina
                    stamina.Amount -= delta;
                    if (stamina.Amount <= 0)
                    {
                        // Stamina tired
                        stamina.Amount = 0;
                        entity.Del<RunTag>();
                        entity.Get<TiredTag>();
                        entity.Get<TargetSpeedPercentChangedTag>();
                        _model.IsTired = true;
                    }
                    _model.Amount = stamina.Amount;
                }
            }
        }
    }
}