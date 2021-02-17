using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class StaminaRestorationSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<Stamina>.Exclude<StaminaHideQuery> _filter = null;
        private readonly InjectData _injectData = null;

        [EcsIgnoreInject] private readonly UiRepository repository = UiRepository.Instance;


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
                            repository.IsStaminaTired = false;
                            ref var hide = ref entity.Get<StaminaHideQuery>();
                            hide.TimeToHide = _injectData.HideStaminaTime;
                        }

                        repository.StaminaAmount = stamina.Amount;
                    }
                }
            }
        }
    }
}