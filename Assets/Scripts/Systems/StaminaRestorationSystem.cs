using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class StaminaRestorationSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<Stamina> _filter = null;
        private readonly InjectData _injectData = null;

        void IEcsRunSystem.Run()
        {
            float amount = _injectData.StaminaAmount;
            float delta = Time.deltaTime;
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                if(!entity.Has<RunTag>() || !entity.Has<InputDirection>())
                {
                    ref var stamina = ref _filter.Get1(i);
                    if(stamina.Amount != amount)
                    {
                        stamina.Amount += stamina.RecoverySpeed * delta;
                        if(stamina.Amount >= amount)
                        {
                            stamina.Amount = amount;
                            entity.Del<TiredTag>();
                        }
                    }
                }
            }
        }
    }
}