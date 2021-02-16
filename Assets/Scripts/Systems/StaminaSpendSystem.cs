using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    sealed class StaminaSpendSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<Stamina, InputDirection, RunTag>.Exclude<TiredTag> _filter = null;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime * _injectData.SpeedOfStaminaSpendOnRun;
            foreach (var i in _filter)
            {
                ref var stamina = ref _filter.Get1(i);
                if (stamina.Amount <= 0)
                {
                    stamina.Amount = 0;
                    var entity = _filter.GetEntity(i);
                    entity.Get<TiredTag>();
                    entity.Get<TargetSpeedPercentChangedTag>();
                }
                else
                    stamina.Amount -= delta;
            }
        }
    }
}