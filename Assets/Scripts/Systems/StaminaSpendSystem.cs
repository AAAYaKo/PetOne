using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using PetOne.Ui;
using UnityEngine;

namespace PetOne.Systems
{
    internal sealed class StaminaSpendSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<Stamina, InputDirection, RunTag>.Exclude<TiredTag, JumpData> _filter = null;
        private readonly InjectData _injectData = null;
        private readonly PlayerStaminaModel _model = null;


        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime * _injectData.SpeedOfStaminaSpendOnRun;
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                if (!entity.Has<BlockMoveTag>() && !entity.Has<AttackTag>())
                {
                    ref var stamina = ref _filter.Get1(i);
                    stamina.Amount -= delta;
                    if (stamina.Amount <= 0)
                    {
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