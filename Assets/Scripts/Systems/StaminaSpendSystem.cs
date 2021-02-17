using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class StaminaSpendSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<Stamina, InputDirection, RunTag>.Exclude<TiredTag, JumpData> _filter = null;
        private readonly InjectData _injectData = null;

        [EcsIgnoreInject] private readonly UiRepository repository = UiRepository.Instance;

        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime * _injectData.SpeedOfStaminaSpendOnRun;
            foreach (var i in _filter)
            {
                ref var stamina = ref _filter.Get1(i);
                stamina.Amount -= delta;
                if (stamina.Amount <= 0)
                {
                    stamina.Amount = 0;
                    var entity = _filter.GetEntity(i);
                    entity.Get<TiredTag>();
                    entity.Get<TargetSpeedPercentChangedTag>();
                    ref var hide = ref entity.Get<StaminaHideQuery>();
                    hide.TimeToHide = _injectData.HideStaminaTime;
                    repository.IsStaminaTired = true;
                }

                repository.StaminaAmount = stamina.Amount;
            }
        }
    }
}